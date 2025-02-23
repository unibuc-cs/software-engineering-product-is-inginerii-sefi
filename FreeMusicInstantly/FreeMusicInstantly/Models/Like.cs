using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace FreeMusicInstantly.Models
{
    public class Like
    {
        [Key]
        [JsonRequired]
        public int LikeId { get; set; }
  
        [Required]
        public string? UserId { get; set; }
        public int SongId { get; set; }
        public DateTime LikeDate { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Song? Song { get; set; }
    }
}
