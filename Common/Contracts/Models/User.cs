using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Threesixty.Common.Contracts.Models
{
    public class User
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
    }
}
