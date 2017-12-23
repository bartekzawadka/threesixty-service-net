using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Models;

namespace Threesixty.Dal.Bll.Managers
{
    public class ChunkManager : Manager
    {
        public ChunkManager(DbContextOptions<ThreesixtyContext> options) : base(options)
        {
        }

        public int AddChunk(Chunk chunk, int imageId)
        {
            if (chunk == null)
            {
                throw new ApiException("Chunk data was not provided", HttpStatusCode.BadRequest);
            }

            if (imageId <= 0)
            {
                throw new ApiException("Image ID was not specified", HttpStatusCode.BadRequest);
            }

            chunk.ImageId = imageId;

            return ExecuteDb(db =>
            {
                db.Chunks.Add(chunk);
                db.SaveChanges();

                return chunk.Id;
            });
        }

        public Chunk GetChunk(int id)
        {
            if (id <= 0)
            {
                throw new ApiException("Chunk ID was not specified", HttpStatusCode.BadRequest);
            }

            return ExecuteDb(db => db.Chunks.SingleOrDefault(x => x.Id == id));
        }
    }
}
