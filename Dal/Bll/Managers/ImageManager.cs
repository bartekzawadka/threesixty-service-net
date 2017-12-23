using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto;
using Threesixty.Common.Contracts.Dto.Stroller;
using Threesixty.Common.Contracts.Enums;
using Threesixty.Dal.Bll.Retrievers;
using Threesixty.Common.Contracts.Models;

namespace Threesixty.Dal.Bll.Managers
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
            IStrollerRetriever ret;
            switch (fileType)
            {
                default:
                    return null;
                case DownloadFileType.Zip:
                    ret = new StrollerRetrieverZip();
                    break;
                case DownloadFileType.Json:
                    ret = new StrollerRetrieverJson();
                    break;
            }

            var image = GetImage(id);
            if (image == null)
                return null;

            var strollerImage = new StrollerFileInfo
            {
                CreatedAt = image.CreatedAt,
                Id = image.Id.ToString(),
                Name = image.Name,
                Thumbnail = image.Thumbnail
            };

            var chunks = new List<StrollerChunkItem>();
            var chunkManager = new ChunkManager(DbOptions);

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

            strollerImage.Chunks = chunks;

            return ret.GetFile(strollerImage);
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
        }
    }
}
