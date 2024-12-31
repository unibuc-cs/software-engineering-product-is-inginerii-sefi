using System.ComponentModel.DataAnnotations.Schema;

namespace FreeMusicInstantly.Models
{
    public class SongAlbum
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? SongId { get; set; }
        public int? AlbumId { get; set; }
        public virtual Song? Song { get; set; }
        public virtual Album? Album { get; set; }
        
    }
}
