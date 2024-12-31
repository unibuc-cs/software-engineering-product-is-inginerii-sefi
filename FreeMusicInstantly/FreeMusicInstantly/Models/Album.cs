using FreeMusicInstantly.Models;
using System.ComponentModel.DataAnnotations;

namespace FreeMusicInstantly.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the album is mendatory")]
        public string AlbumName { get; set; }

        [Required(ErrorMessage = "The description of the album is mendatory")]
        public string Description { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? NrSongs { get; set; }

        public virtual ICollection<SongAlbum>? SongAlbums { get; set; }

    }
}