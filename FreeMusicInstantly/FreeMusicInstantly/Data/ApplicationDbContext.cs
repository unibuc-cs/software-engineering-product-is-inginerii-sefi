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

        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

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

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.User1Id);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fr => fr.SenderId);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.ReceiverId);

        }

    }
}
