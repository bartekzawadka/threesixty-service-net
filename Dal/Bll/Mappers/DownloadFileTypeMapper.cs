using System;
using System.Collections.Generic;
using System.Text;
using Threesixty.Common.Contracts.Enums;

namespace Threesixty.Dal.Bll.Mappers
{
    public class DownloadFileTypeMapper
    {
        public static DownloadFileType GetFileType(string type)
        {
            if (string.IsNullOrEmpty(type))
                type = "";

            switch (type.ToLower())
            {
                default:
                    return DownloadFileType.Json;
                case "zip":
                    return DownloadFileType.Zip;
            }
        }
    }
}
