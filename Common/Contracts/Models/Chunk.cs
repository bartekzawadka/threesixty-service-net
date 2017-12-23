using System.ComponentModel.DataAnnotations.Schema;

namespace Threesixty.Common.Contracts.Models
{
    public class Chunk
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int Index { get; set; }
        public string MimeType { get; set; }
        public string Data { get; set; }

        [ForeignKey("ImageId")]
        public Image Image { get; set; }
    }
}
