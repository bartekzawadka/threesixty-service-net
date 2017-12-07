using System;
using System.Collections.Generic;
using System.Net;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto.Stroller;

namespace Threesixty.Dal.Bll.Retrievers
{
    public abstract class StrollerRetriever
    {
        protected ImageManager ImageManager { get; }
        protected ChunkManager ChunkManager { get; }

        protected StrollerRetriever(ImageManager imageManager, ChunkManager chunkManager)
        {
            ChunkManager = chunkManager ?? throw new ApiException("Chunk manager cannot be null", HttpStatusCode.InternalServerError);
            ImageManager = imageManager ?? throw new ApiException("Image manager cannot be null", HttpStatusCode.InternalServerError);
        }

        public string GetFile(int id)
        {
            var image = ImageManager.GetImage(id);
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

            try
            {
                foreach (var imageChunk in image.Chunks)
                {
                    var chunkRecord = ChunkManager.GetChunk(imageChunk.Id);
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

            return BuildFile(strollerImage);
        }

        protected abstract string BuildFile(StrollerFileInfo strollerFileInfo);
    }
}
