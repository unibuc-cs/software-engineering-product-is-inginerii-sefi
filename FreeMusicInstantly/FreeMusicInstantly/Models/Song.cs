using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FreeMusicInstantly.Models
{
    public class Song
    {
        [Key]
        [JsonRequired]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is mandatory")]
        [StringLength(50, ErrorMessage = "The title can't be more than 50 characters")]

        public string? Title { get; set; }
        [Required(ErrorMessage = "Description is mandatory")]
        [StringLength(100, ErrorMessage = "The description can't have more than 100 characters")]
        public string? Description { get; set; }

        public string ? SongFile { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<SongAlbum>? SongAlbums { get; set; }
        public virtual ICollection<SongPlaylist>? SongPlaylists { get; set; }

        public virtual ICollection<Play>? Plays { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }

    }
}