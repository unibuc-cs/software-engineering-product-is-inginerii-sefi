using FreeMusicInstantly.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace FreeMusicInstantly.Models
{
    public class Comment
    {
        [Key]
        [JsonRequired]
        public int Id { get; set; }
        [Required(ErrorMessage = "The content is mandatory")]
        [StringLength(100, ErrorMessage = "The text should not be larger than 100 characters.")]
        public string? Text { get; set; }

        public DateTime? Date { get; set; }
        public int? SongId { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual Song? Song { get; set; }
    }
}