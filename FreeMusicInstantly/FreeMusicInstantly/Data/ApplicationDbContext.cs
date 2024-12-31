using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreeMusicInstantly.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Song> Songs { get; set; }
        public DbSet<SongAlbum> SongAlbums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SongAlbum>()
            .HasKey(ab => new
            {
                ab.Id,
                ab.SongId,
                ab.AlbumId
            });

            modelBuilder.Entity<SongAlbum>()
               .HasOne(ab => ab.Song)
               .WithMany(ab => ab.SongAlbums)
               .HasForeignKey(ab => ab.SongId);

            modelBuilder.Entity<SongAlbum>()
                .HasOne(ab => ab.Album)
                .WithMany(ab => ab.SongAlbums)
                .HasForeignKey(ab => ab.AlbumId);


        }

    }
}
