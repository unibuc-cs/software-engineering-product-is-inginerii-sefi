using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeMusicInstantly.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is mendatory")]
        [StringLength(50, ErrorMessage = "The title can't be more than 50 characters")]

        public string Title { get; set; }
        [Required(ErrorMessage = "Description is mendatory")]
        [StringLength(100, ErrorMessage = "The description can't have more than 100 characters")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Foto cover is mendatory")]
        //public string Photo_Cover { get; set; }
        public string SongFile { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
       
        public virtual ICollection<SongAlbum>? SongAlbums { get; set; }
        public virtual ICollection<SongPlaylist>? SongPlaylists { get; set; }

    }
}