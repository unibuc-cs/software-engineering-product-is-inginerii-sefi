using FreeMusicInstantly.Models;
using System.ComponentModel.DataAnnotations;

namespace FreeMusicInstantly.Models
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the playlist is mendatory")]
        public string PlaylistName { get; set; }

        public string? Description { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<SongPlaylist>? SongPlaylists { get; set; }
    }
}
