using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto;
using Threesixty.Common.Contracts.Dto.Stroller;
using Threesixty.Common.Contracts.Enums;
using Threesixty.Dal.Bll.Converters;
using Threesixty.Dal.Bll.Helpers;
using Threesixty.Dal.Bll.Retrievers;
using Threesixty.Dal.Dll;
using Threesixty.Dal.Dll.Models;

namespace Threesixty.Dal.Bll
{
    public class ImageManager : Manager
    {
        public ImageManager(DbContextOptions<ThreesixtyContext> options) : base(options)
        {
        }

        public Image AddImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Image data was not provided");

            image.CreatedAt = DateTime.Now;

            return ExecuteDb(db =>
            {
                db.Images.Add(image);
                db.SaveChanges();

                return image;
            });
        }

        public Image AddImage(StrollerFileInfo fInfo)
        {
            if (fInfo == null)
                throw new ArgumentNullException(nameof(fInfo), "Stroler file data was not provided");

            var image = new Image
            {
                ChunkWidth = 0,
                ChunkHeight = 0,
                Thumbnail = fInfo.Thumbnail,
                Name = fInfo.Name
            };

            image = AddImage(image);

            var chunksManager = new ChunkManager(DbOptions);

            try
            {
                Parallel.ForEach(fInfo.Chunks, (item, state, arg3) =>
                {
                    chunksManager.AddChunk(new Chunk
                    {
                        Data = item.Data,
                        Index = item.Index
                    }, image.Id);
                });
            }
            catch (Exception ex)
            {
                throw new ApiException("Unable to add one or more chunks: " + ex.Message, ex,
                    HttpStatusCode.BadRequest);
            }

            return image;
        }

        public Image GetImage(int id)
        {
            if (id <= 0)
                throw new ApiException("Image ID is invalid", HttpStatusCode.BadRequest);

            return ExecuteDb(db =>
            {
                return db.Images.Where(x => x.Id == id).Include(y => y.Chunks).Select(a => new Image
                {
                    Id = a.Id,
                    Name = a.Name,
                    ChunkWidth = a.ChunkWidth,
                    ChunkHeight = a.ChunkHeight,
                    CreatedAt = a.CreatedAt,
                    Thumbnail = a.Thumbnail,
                    Chunks = a.Chunks.Select(x => new Chunk
                    {
                        Id = x.Id,
                        Index = x.Index,
                        MimeType = x.MimeType
                    }).ToList()
                }).SingleOrDefault();
            });
        }

        public string GetStrollerImageFile(int id, DownloadFileType fileType)
        {
            StrollerRetriever ret;
            switch (fileType)
            {
                default:
                    return null;
                case DownloadFileType.Zip:
                    //return GetStrollerImageZip(id);
                    ret = new StrollerRetrieverZip(this, new ChunkManager(DbOptions));
                    break;
                case DownloadFileType.Json:
                    ret = new StrollerRetrieverJson(this, new ChunkManager(DbOptions));
                    break;
            }

            return ret.GetFile(id);
        }

        private string GetStrollerImageZip(int id)
        {
            var image = GetImage(id);
            if (image == null)
                return null;

            var chunkManager = new ChunkManager(DbOptions);
            var chunks = new List<StrollerChunkItem>();

            try
            {
                foreach (var imageChunk in image.Chunks)
                {
                    var chunkRecord = chunkManager.GetChunk(imageChunk.Id);
                    if (!string.IsNullOrEmpty(chunkRecord?.Data))
                    {
                        chunks.Add(new StrollerChunkItem
                        {
                            Index = chunkRecord.Index,
                            Data = chunkRecord.Data
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e.Message, HttpStatusCode.InternalServerError);
            }

            var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            try
            {
                if (chunks.Count == 0)
                    return null;

                var extension = StringHelper.ExtractFileExtensionFromBase64(chunks[0].Data);

                try
                {
                    Parallel.ForEach(chunks, (item, state, arg3) =>
                    {
                        var data = item.Data;
                        if (data.Contains(","))
                        {
                            var splits = data.Split(",");
                            if (splits != null && splits.Length > 0)
                            {
                                data = splits[1];
                            }
                        }

                        var buff = Convert.FromBase64String(data);
                        var fPath = Path.Combine(tmpDir.FullName,
                            item.Index + (string.IsNullOrEmpty(extension) ? string.Empty : "." + extension));
                        using (var fs = File.Open(fPath, FileMode.CreateNew, FileAccess.Write))
                        {
                            fs.Write(buff, 0, buff.Length);
                        }
                    });
                }
                catch (Exception e)
                {
                    throw new ApiException(e.Message, HttpStatusCode.InternalServerError);
                }

                var zipPath = Path.Combine(Path.GetTempPath(), tmpDir.Name + ".zip");

                using (var zipFile = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var file in Directory.GetFiles(tmpDir.FullName))
                    {
                        zipFile.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                }

                return zipPath;
            }
            finally
            {
                Directory.Delete(tmpDir.FullName, true);
            }
        }

        public PageableList<Image> GetImages(int skip, int limit)
        {
            if (skip < 0)
            {
                skip = 0;
            }
            if (limit <= 0)
            {
                limit = 50;
            }

            return ExecuteDb(db =>
            {
                var totalCount = db.Images.Count();
                var rows = db.Images.OrderByDescending(x => x.CreatedAt).Select(x => new Image
                {
                    CreatedAt = x.CreatedAt,
                    ChunkWidth = x.ChunkWidth,
                    ChunkHeight = x.ChunkHeight,
                    Thumbnail = x.Thumbnail,
                    Name = x.Name,
                    Id = x.Id
                }).Skip(skip).Take(limit).ToList();

                return new PageableList<Image>
                {
                    Rows = rows,
                    TotalCount = totalCount
                };
            });

            //            return ExecuteDb(db => db.Images.OrderByDescending(x => x.CreatedAt).Select(x => new Image
            //            {
            //                CreatedAt = x.CreatedAt,
            //                ChunkWidth = x.ChunkWidth,
            //                ChunkHeight = x.ChunkHeight,
            //                Name = x.Name,
            //                Id = x.Id
            //            }).Skip(skip).Take(limit).ToList());
        }
    }
}
