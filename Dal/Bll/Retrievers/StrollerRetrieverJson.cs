using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Threesixty.Common.Contracts.Dto.Stroller;

namespace Threesixty.Dal.Bll.Retrievers
{
    public class StrollerRetrieverJson : IStrollerRetriever
    {
        public string GetFile(StrollerFileInfo strollerFileInfo)
        {
            var json = new JObject();
            if (!string.IsNullOrEmpty(strollerFileInfo.Name))
                json["name"] = strollerFileInfo.Name;
            if (!string.IsNullOrEmpty(strollerFileInfo.Thumbnail))
                json["thumbnail"] = strollerFileInfo.Thumbnail;
            if (strollerFileInfo.CreatedAt != null)
                json["createdAt"] = strollerFileInfo.CreatedAt.Value.ToUniversalTime()
                    .ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

            if (strollerFileInfo.Chunks != null && strollerFileInfo.Chunks.Count > 0)
            {
                var chunks = new JArray();
                foreach (var strollerChunkItem in strollerFileInfo.Chunks)
                {
                    var obj = new JObject
                    {
                        ["index"] = strollerChunkItem.Index,
                        ["image"] = strollerChunkItem.Data
                    };
                    chunks.Add(obj);
                }
                json["chunks"] = chunks;
            }

            var tmpFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
            var jsonStrin = JsonConvert.SerializeObject(json);

            File.WriteAllText(tmpFilePath, jsonStrin);

            return tmpFilePath;
        }
    }
}
