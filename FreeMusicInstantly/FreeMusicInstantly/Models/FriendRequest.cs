using FreeMusicInstantly.Models;
using System.ComponentModel.DataAnnotations;

namespace FreeMusicInstantly.Models
{
    public class FriendRequest
    {
        [Key]
        public int Id { get; set; }

        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }

        public virtual ApplicationUser? Sender { get; set; }
        public virtual ApplicationUser? Receiver { get; set; }
    }
}
