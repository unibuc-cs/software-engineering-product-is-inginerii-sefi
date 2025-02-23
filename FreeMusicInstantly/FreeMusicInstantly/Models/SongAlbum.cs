using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FreeMusicInstantly.Models
{
    public class SongAlbum
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonRequired]
        public int Id { get; set; }
        public int? SongId { get; set; }
        public int? AlbumId { get; set; }
        public virtual Song? Song { get; set; }
        public virtual Album? Album { get; set; }
        
    }
}
