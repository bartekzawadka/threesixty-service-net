using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts;
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
                    Chunks = a.Chunks.Select(x => new Chunk
                    {
                        Id = x.Id,
                        Index = x.Index,
                        MimeType = x.MimeType
                    }).ToList()
                }).SingleOrDefault();
            });
        }

        public List<Image> GetImages(int skip, int limit)
        {
            if (skip < 0)
            {
                skip = 0;
            }
            if (limit <= 0)
            {
                limit = 50;
            }

            return ExecuteDb(db => db.Images.OrderByDescending(x => x.CreatedAt).Select(x => new Image
            {
                CreatedAt = x.CreatedAt,
                ChunkWidth = x.ChunkWidth,
                ChunkHeight = x.ChunkHeight,
                Name = x.Name,
                Id = x.Id
            }).Skip(skip).Take(limit).ToList());
        }
    }
}
