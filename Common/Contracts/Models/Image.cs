using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Threesixty.Common.Contracts.Interfaces;

namespace Threesixty.Common.Contracts.Models
{
    public class Image : IIdentifier<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public int ChunkWidth { get; set; }

        [Required]
        public int ChunkHeight { get; set; }

        public string Thumbnail { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<Chunk> Chunks { get; set; }
    }
}
