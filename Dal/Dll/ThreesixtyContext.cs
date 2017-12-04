using Microsoft.EntityFrameworkCore;
using Threesixty.Dal.Dll.Models;

namespace Threesixty.Dal.Dll
{
    public class ThreesixtyContext : DbContext
    {
        public ThreesixtyContext(DbContextOptions<ThreesixtyContext> options) : base (options)
        {
        }

        public DbSet<Chunk> Chunks { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chunk>().ToTable("Chunk");
            modelBuilder.Entity<Image>().ToTable("Image");
        }
    }
}
