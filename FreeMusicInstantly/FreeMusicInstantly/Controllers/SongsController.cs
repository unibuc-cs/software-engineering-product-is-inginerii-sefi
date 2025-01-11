using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreeMusicInstantly.Controllers
{
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public SongsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var songs = db.Songs.Include("User")
                                .OrderBy(p => p.Title)
                                .ToList();
            int perPage = 9;
            string search = "";
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {   search = Convert.ToString(HttpContext.Request.Query["search"]);
                songs = db.Songs.Include("User")
                                .Where(p => p.Title.Contains(search))
                                .OrderBy(p => p.Title)
                                .ToList();
            }
            ViewBag.SearchString = search;

            var totalItems = songs.Count();
            var page = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if(!page.Equals(0))
            {
                offset = (page - 1) * perPage;
            }
            var paginatedSongs = songs.Skip(offset).Take(perPage).ToList();
            ViewBag.Songs = paginatedSongs;
            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)perPage);

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Songs/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Songs/Index/?page";
            }
            return View();
        }

        public IActionResult Show(int id)
        {
            var song = db.Songs.Find(id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }
    }
}
