using System;
using System.Collections.Generic;
using System.Text;

namespace Threesixty.Common.Contracts.Dto.Stroller
{
    public class StrollerFileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<StrollerChunkItem> Chunks { get; set; }
    }
}
