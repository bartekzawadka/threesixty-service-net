using System;
using System.ComponentModel.DataAnnotations;
using Threesixty.Common.Contracts.Interfaces;

namespace Threesixty.Common.Contracts.Models
{
    public class User : IIdentifier<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? LastLogin { get; set; }
    }
}
