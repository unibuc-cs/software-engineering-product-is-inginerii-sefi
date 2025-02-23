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

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Play> Plays { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) // Renamed parameter to 'builder'
        {
            base.OnModelCreating(builder);

            builder.Entity<SongAlbum>()
                .HasKey(ab => new
                {
                    ab.Id,
                    ab.SongId,
                    ab.AlbumId
                });

            builder.Entity<SongAlbum>()
                .HasOne(ab => ab.Song)
                .WithMany(ab => ab.SongAlbums)
                .HasForeignKey(ab => ab.SongId);

            builder.Entity<SongAlbum>()
                .HasOne(ab => ab.Album)
                .WithMany(ab => ab.SongAlbums)
                .HasForeignKey(ab => ab.AlbumId);

            builder.Entity<SongPlaylist>()
                .HasKey(pl => new
                {
                    pl.Id,
                    pl.SongId,
                    pl.PlaylistId
                });

            builder.Entity<SongPlaylist>()
                .HasOne(pl => pl.Song)
                .WithMany(pl => pl.SongPlaylists)
                .HasForeignKey(pl => pl.SongId);

            builder.Entity<SongPlaylist>()
                .HasOne(pl => pl.Playlist)
                .WithMany(pl => pl.SongPlaylists)
                .HasForeignKey(pl => pl.PlaylistId);

            builder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.User1Id);

            builder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id);

            builder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fr => fr.SenderId);

            builder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.ReceiverId);

            builder.Entity<Song>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Song)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
