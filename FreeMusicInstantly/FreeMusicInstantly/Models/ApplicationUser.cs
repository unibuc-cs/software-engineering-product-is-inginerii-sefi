using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;




namespace FreeMusicInstantly.Models
{
    public class ApplicationUser : IdentityUser
    {
        
       
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MyMusicDescription { get; set; }
        public string? Biography { get; set; }

        public virtual ICollection<FriendRequest>? SentFriendRequests { get; set; }
        public virtual ICollection<FriendRequest>? ReceivedFriendRequests { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
