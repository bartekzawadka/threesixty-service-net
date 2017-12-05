using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto.Stroller;

namespace Threesixty.Dal.Bll
{
    public class StrollerProcessor
    {
        public static StrollerFileInfo ParseStrollerFile(Stream fileStream, string fileName)
        {
            if (fileStream == null)
                throw new ApiException("Provided file stream is empty", HttpStatusCode.BadRequest);

            string jsonString;
            try
            {
                using (var sr = new StreamReader(fileStream))
                {
                    jsonString = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new ApiException("Error occurred while reading input file stream: " + ex.Message, ex,
                    HttpStatusCode.BadRequest);
            }

            JObject json;
            try
            {
                json = JsonConvert.DeserializeObject(jsonString) as JObject;
            }
            catch (Exception exception)
            {
                throw new ApiException("Error occurred while converting input file to JSON object: " + exception.Message, exception,
                    HttpStatusCode.BadRequest);
            }

            if (json == null)
                throw new ApiException("Invalid JSON file format", HttpStatusCode.BadRequest);

            if (json["chunks"] == null || json["chunks"].GetType() != typeof(JArray))
            {
                throw new ApiException("Invalid Stroller file format. No chunks collection found", HttpStatusCode.BadRequest);
            }

            var chunks = (JArray) json["chunks"];
            int indexNum;

            foreach (var job in chunks)
            {
                if (job == null || job.GetType() != typeof(JObject))
                    throw new ApiException(
                        "Invalid Stroller file structure. Chunks array does not contain proper fields",
                        HttpStatusCode.BadRequest);

                if(job["index"] == null)
                    throw new ApiException(
                        "Invalid Stroller file structure. Some chunks do not contain index number",
                        HttpStatusCode.BadRequest);

                if (!int.TryParse(job["index"].ToString(), out indexNum))
                {
                    throw new ApiException(
                        "Invalid Stroller file structure. Invalid index number for some chunks. Could not extract index value",
                        HttpStatusCode.BadRequest);
                }

                if(job["image"] == null)
                    throw new ApiException(
                        "Invalid Stroller file structure. Some chunks do not contain image data",
                        HttpStatusCode.BadRequest);


            }

            var chunkItems = new List<StrollerChunkItem>();

            foreach (var chunk in chunks)
            {
                chunkItems.Add(new StrollerChunkItem
                {
                    Data = chunk["image"].ToString(),
                    Index = int.Parse(chunk["index"].ToString())
                });
            }

            var result = new StrollerFileInfo
            {
                Chunks = chunkItems
            };

            if (json["thumbnail"] != null)
            {
                result.Thumbnail = json["thumbnail"].ToString();
            }

            if (json["createdAt"] != null)
            {
                DateTime dt;
                if (DateTime.TryParse(json["createdAt"].ToString(), out dt))
                {
                    result.CreatedAt = dt;
                }
            }

            var name = string.Empty;
            if(string.IsNullOrEmpty(fileName))
                return result;

            fileName = Path.GetFileNameWithoutExtension(fileName);
            var parts = fileName.Split(' ', '-', '_');

            if (parts.Length > 0)
            {
                for (var i = 0; i < parts.Length; i++)
                {
                    if (i == 0)
                        parts[i] = parts[i].First().ToString().ToUpper() + parts[i].Substring(1);
                }

                name = String.Join(' ', parts);
                result.Name = name;
            }

            return result;
        }
    }
}
