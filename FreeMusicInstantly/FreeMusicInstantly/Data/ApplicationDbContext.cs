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

        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<SongPlaylist> SongPlaylists { get; set; }

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


            modelBuilder.Entity<SongPlaylist>()
                .HasKey(pl => new
                {
                    pl.Id,
                    pl.SongId,
                    pl.PlaylistId
                });

            modelBuilder.Entity<SongPlaylist>()
                .HasOne(pl => pl.Song)
                .WithMany(pl => pl.SongPlaylists)
                .HasForeignKey(pl => pl.SongId);

            modelBuilder.Entity<SongPlaylist>()
                .HasOne(pl => pl.Playlist)
                .WithMany(pl => pl.SongPlaylists)
                .HasForeignKey(pl => pl.PlaylistId);
        }

    }
}
