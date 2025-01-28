using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace proiectDAW.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int Id)
        {
            Comment comm = db.Comments.Find(Id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                TempData["message"] = "The comment was deleted";
                TempData["messageType"] = "alert alert-success";
                return Redirect("/Songs/Show/" + comm.SongId);
            }

            else
            {
                TempData["message"] = "You do not have the right to delete the comment";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index", "Songs");
            }
        }
    }

}