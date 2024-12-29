using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace proiectDAW.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<IActionResult> MyProfile()
        {
            var currentUserId = _userManager.GetUserId(User); 
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            ViewBag.CurrentUserId = currentUserId;
            if (currentUser == null)
            {
                TempData["message"] = "Your profile was not found";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
            ViewBag.EsteArtist = User.IsInRole("Artist");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }

            return View(currentUser); 
        }

   
        [Authorize(Roles = "Admin,User")]
        public IActionResult ViewUsers()
        {
            var users = db.Users
                          .Where(u => _userManager.IsInRoleAsync((ApplicationUser)u, "User").Result) // Filter by "User" role
                          .OrderBy(u => u.UserName)
                          .ToList();

            return View(users); 
        }

       
        [Authorize(Roles = "Admin,Artist")]
        public IActionResult ViewArtists()
        {
            var artists = db.Users
                            .Where(u => _userManager.IsInRoleAsync((ApplicationUser)u, "Artist").Result) // Filter by "Artist" role
                            .OrderBy(u => u.UserName)
                            .ToList();

            return View(artists); 
        }



        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }
            return View();
        }



        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<ActionResult> Show(string id)
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ApplicationUser user = (ApplicationUser)db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            return View(user);
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<ActionResult> Edit()
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);


            ApplicationUser user = (ApplicationUser)db.Users.Find(currentUserId);
            ViewBag.CurrentUserId = currentUserId;

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); 

           
            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); 
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [Authorize(Roles = "User,Admin,Artist")]
        [HttpPost]
        public async Task<ActionResult> Edit( ApplicationUser newData)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            ApplicationUser user = (ApplicationUser)db.Users.Find(currentUserId);

            ViewBag.CurrentUserId = currentUserId;
            if (user == null)
            {
                TempData["message"] = "User not found.";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.PhoneNumber = newData.PhoneNumber;
                user.MyMusicDescription = newData.MyMusicDescription;
                user.Biography = newData.Biography;

              
                db.SaveChanges();

                TempData["message"] = "The user was edited!";
                TempData["messageType"] = "alert alert-success";
            }
            else
            {
                TempData["message"] = "Error when editing the user";
                TempData["messageType"] = "alert alert-danger";
            }

            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Where(u => u.Id == id)
                         .First();

           

            db.ApplicationUsers.Remove((ApplicationUser)user);

            db.SaveChanges();

            return RedirectToAction("MyProfile");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
         /* to be implemented
         -delete method
         -show songs and albums for artist
         -show playlists for users
         */

    }
}