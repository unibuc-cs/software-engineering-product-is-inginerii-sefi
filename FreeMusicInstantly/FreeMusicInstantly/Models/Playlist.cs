using FreeMusicInstantly.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FreeMusicInstantly.Models
{
    public class Playlist
    {
        [Key]
        [JsonRequired]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the playlist is mendatory")]
        public string? PlaylistName { get; set; }

        public string? Description { get; set; }
        //[Required(ErrorMessage = "Foto cover is mendatory")]
        public string? PhotoCover { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<SongPlaylist> SongPlaylists { get; set; } = new List<SongPlaylist>();
    }
}
