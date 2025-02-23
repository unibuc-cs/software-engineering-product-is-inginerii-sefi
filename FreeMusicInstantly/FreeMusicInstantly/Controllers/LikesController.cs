using FreeMusicInstantly.Data;
using FreeMusicInstantly.Data.Migrations;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FreeMusicInstantly.Controllers
{
    public class LikesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;
        public LikesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }



        [HttpPost]
        public IActionResult ToggleLike(int songId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                Console.WriteLine("❌ ERROR: User is not logged in.");
                return Unauthorized();
            }

            Console.WriteLine($"✅ User {userId} is trying to like/unlike Song {songId}");

            var existingLike = db.Likes
                .FirstOrDefault(l => l.SongId == songId && l.UserId == userId);

            bool liked;

            if (existingLike != null)
            {
                Console.WriteLine("❌ User already liked the song. Removing like...");
                db.Likes.Remove(existingLike);
                liked = false;
            }
            else
            {
                Console.WriteLine("✅ Adding like to the database...");
                var like = new Like
                {
                    SongId = songId,
                    UserId = userId,
                    LikeDate = DateTime.Now
                };
                db.Likes.Add(like);
                liked = true;
            }

            Console.WriteLine("📝 Saving changes to database...");
            db.SaveChanges();

            var likeCount = db.Likes.Count(l => l.SongId == songId);
            Console.WriteLine($"👍 Updated like count: {likeCount}");

            return Json(new { success = true, likeCount, liked });
        }

    }
}

