using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Threesixty.Common.Contracts;
using Threesixty.Common.Contracts.Dto.Stroller;
using Threesixty.Dal.Bll.Helpers;

namespace Threesixty.Dal.Bll.Retrievers
{
    public class StrollerRetrieverZip : IStrollerRetriever
    {
        public string GetFile(StrollerFileInfo strollerFileInfo)
        {
            var tmpDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            try
            {
                if (strollerFileInfo.Chunks.Count == 0)
                    return null;

                var extension = StringHelper.ExtractFileExtensionFromBase64(strollerFileInfo.Chunks[0].Data);

                try
                {
                    Parallel.ForEach(strollerFileInfo.Chunks, (item, state, arg3) =>
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
    }
}
