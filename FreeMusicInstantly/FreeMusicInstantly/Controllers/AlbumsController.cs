using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace proiectDAW.Controllers
{

    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Index()
        {
            SetAccessRights();
            var cat = db.Albums.Where(x => x.UserId == _userManager.GetUserId(User)).Include("User");
            foreach (Album c in cat)
            {
                c.NrSongs = db.SongAlbums.Where(a => a.AlbumId == c.Id).Count();
            }
            ViewBag.Albums = cat;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }


            return View();
        }
        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult New()
        {
            Album cat = new Album();
            return View(cat);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Artist")]
        public IActionResult New(Album cat)
        {

            cat.NrSongs = 0;

            cat.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Albums.Add(cat);
                db.SaveChanges();
                TempData["message"] = "The album was added";
                TempData["messageType"] = "alert alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "The album was not added successfully";
                TempData["messageType"] = " alert alert-danger";
                return View(cat);
            }
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            var cat = db.Albums.Include("SongAlbums.Song")
                                   .Include("SongAlbums.Song.User")
                                   .Include("User")
                                   .Where(a => a.Id == id).First();

            cat.NrSongs = db.SongAlbums.Where(a => a.AlbumId == cat.Id).Count();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }
            return View(cat);


        }

        [Authorize(Roles = "Admin,Artist")]
        public IActionResult Edit(int id)
        {
            Album cat = db.Albums.Where(a => a.Id == id).First();

            SetAccessRights();

            if (cat.UserId == _userManager.GetUserId(User))
            {
                return View(cat);
            }

            else
            {
                TempData["message"] = "You don't have the right to edit a album that doesn't belong to you";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult Edit(int id, Album reqcat)
        {
            Album cat = db.Albums.Where(a => a.Id == id).First();

            if (ModelState.IsValid)
            {
                if (cat.UserId == _userManager.GetUserId(User))
                {
                    cat.AlbumName = reqcat.AlbumName;
                    cat.Description = reqcat.Description;
                    cat.NrSongs = db.SongAlbums.Where(a => a.AlbumId == cat.Id).Count();

                    TempData["message"] = "The album was edited successfully";
                    TempData["messageType"] = "alert alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You don't have the right to edit a album that doesn't belong to you";
                    TempData["messageType"] = "alert alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(reqcat);
            }


        }

        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Album cat = db.Albums.Include("SongAlbums")
                                        .Where(a => a.Id == id).First();
            if (_userManager.GetUserId(User) == cat.UserId || User.IsInRole("Admin"))
            {
                if (cat.SongAlbums.Count > 0)
                {
                    foreach (var bmkcat in cat.SongAlbums)
                    {
                        db.SongAlbums.Remove(bmkcat);
                    }
                }


                db.Albums.Remove(cat);
                db.SaveChanges();
                TempData["message"] = "The album was deleted";
                TempData["messageType"] = " alert alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You don't have the right to delete a album that doesn't belong to you";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult RemoveSong(int CategoryId, int BookmarkId)
        {
            Album cat = db.Albums.Include("SongAlbums")
                                      .Where(a => a.Id == CategoryId).First();

            if (_userManager.GetUserId(User) == cat.UserId)
            {

                foreach (var bmkcat in cat.SongAlbums)
                {
                    if (bmkcat.SongId == BookmarkId)
                    {
                        db.SongAlbums.Remove(bmkcat);
                    }

                }


                db.SaveChanges();
                TempData["message"] = "The song was deleted from the album";
                TempData["messageType"] = " alert alert-success";
                return Redirect("/Albums/Show/" + cat.Id);
            }
            else
            {
                TempData["message"] = "You don't have the right to remove the song from this album";
                return Redirect("/Albums/Show/" + cat.Id);
            }

        }
        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


    }



}
