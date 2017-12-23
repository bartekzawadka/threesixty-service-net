using Microsoft.EntityFrameworkCore;
using Threesixty.Common.Contracts.Models;

namespace Threesixty.Dal.Bll
{
    public class ThreesixtyContext : DbContext
    {
        public ThreesixtyContext(DbContextOptions<ThreesixtyContext> options) : base (options)
        {
        }

        public DbSet<Chunk> Chunks { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chunk>().ToTable("Chunk");
            modelBuilder.Entity<Image>().ToTable("Image");
            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}
