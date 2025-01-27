using FreeMusicInstantly.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FreeMusicInstantly.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        public string? User1Id { get; set; }
        public string? User2Id { get; set; }

        public virtual ApplicationUser? User1 { get; set; }
        public virtual ApplicationUser? User2 { get; set; }

        public DateTime? FriendshipDate { get; set; }
    }
}
