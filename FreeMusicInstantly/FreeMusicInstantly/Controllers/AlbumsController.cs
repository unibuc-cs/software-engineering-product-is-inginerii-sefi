using FreeMusicInstantly.Data;
using FreeMusicInstantly.Data.Migrations;
using FreeMusicInstantly.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FreeMusicInstantly.Controllers
{

    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Index()
        {
            SetAccessRights();
            var cat = db.Albums.Include("User").Where(x => x.UserId == _userManager.GetUserId(User)).Include("User");
            var search = "";
            foreach (Album c in cat)
            {
                c.NrSongs = db.SongAlbums.Where(a => a.AlbumId == c.Id).Count();
            }

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                var sanitizer = new HtmlSanitizer();

                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                search = sanitizer.Sanitize(search);

                List<int> albumIds = db.Albums
                                       .Where(album => album.AlbumName.Contains(search))
                                       .Select(album => album.Id)
                                       .ToList();
                cat = db.Albums
                            .Where(album => albumIds.Contains(album.Id))
                            .Include("User")
                            .OrderBy(album => album.AlbumName);
            }

            ViewBag.Albums = cat;
            ViewBag.SearchString = search;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }


            return View();
        }
        [Authorize(Roles = "Admin,Artist")]
        public IActionResult New()
        {
            Album cat = new Album();
            return View(cat);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Artist")]
        public async Task<IActionResult> New(Album cat, IFormFile? PhotoCover)
        {

            cat.NrSongs = 0;

            cat.UserId = _userManager.GetUserId(User);

            cat.Date = DateTime.Now;

            if (PhotoCover != null)
            {
                var fileName = Path.GetFileName(PhotoCover.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoCover.CopyToAsync(fileSteam);
                }
                cat.PhotoCover = fileName;
            }
            else
            {
                cat.PhotoCover = "default.jpg"; // default image as a black photo
            }

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
            var cat = db.Albums.Include("SongAlbums")
                                   .Include("SongAlbums.Song")
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
        public async Task<IActionResult> Edit(int id, Album reqcat, IFormFile? PhotoCover)
        {
            Album cat = db.Albums.Where(a => a.Id == id).First();

            if (ModelState.IsValid)
            {
                if (cat.UserId == _userManager.GetUserId(User))
                {
                    cat.AlbumName = reqcat.AlbumName;
                    cat.Description = reqcat.Description;
                    cat.NrSongs = db.SongAlbums.Where(a => a.AlbumId == cat.Id).Count();
                    cat.Date = DateTime.Now; // no need to take it from reqcat

                    if (PhotoCover != null)
                    {
                        var fileName = Path.GetFileName(PhotoCover.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await PhotoCover.CopyToAsync(fileSteam);
                        }
                        cat.PhotoCover = fileName;
                    }
                    else
                    {
                        if (reqcat.PhotoCover != null)
                        {
                            cat.PhotoCover = reqcat.PhotoCover;
                        }
                        else
                        {
                            if (cat.PhotoCover == null)
                            {
                                cat.PhotoCover = "default.jpg";
                            }
                        }
                    }

                    db.SongAlbums.RemoveRange(db.SongAlbums.Where(a => a.AlbumId == cat.Id));
                    if (reqcat.SongAlbums == null)
                    {
                        reqcat.SongAlbums = new List<SongAlbum>();
                    }
                    if (cat.SongAlbums == null)
                    {
                        cat.SongAlbums = new List<SongAlbum>();
                    }
                    cat.SongAlbums.Clear();
                    if (reqcat.SongAlbums != null)
                    {
                        foreach (var song in reqcat.SongAlbums)
                        {
                            SongAlbum sa = new SongAlbum();
                            sa.AlbumId = cat.Id;
                            sa.SongId = song.SongId;
                            db.SongAlbums.Add(sa);
                            cat.SongAlbums.Add(sa);
                        }
                    }
                    db.SaveChanges();

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

        //[Authorize(Roles = "Admin,Artist")]
        //[HttpPost]
        //public IActionResult RemoveSong(int AlbumId, int SongId)
        //{
        //    Album cat = db.Albums.Include("SongAlbums")
        //                              .Where(a => a.Id == AlbumId).First();

        //    if (_userManager.GetUserId(User) == cat.UserId)
        //    {

        //        foreach (var bmkcat in cat.SongAlbums)
        //        {
        //            if (bmkcat.SongId == SongId)
        //            {
        //                db.SongAlbums.Remove(bmkcat);
        //            }

        //        }


        //        db.SaveChanges();
        //        TempData["message"] = "The song was deleted from the album";
        //        TempData["messageType"] = " alert alert-success";
        //        return Redirect("/Albums/Show/" + cat.Id);
        //    }
        //    else
        //    {
        //        TempData["message"] = "You don't have the right to remove the song from this album";
        //        return Redirect("/Albums/Show/" + cat.Id);
        //    }

        //}

        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult RemoveSong([FromForm] SongAlbum songAlbum)
        {
            Album album = db.Albums.Include("SongAlbums")
                                      .Where(a => a.Id == songAlbum.AlbumId).First();
            if (_userManager.GetUserId(User) == album.UserId)
            {
                var songAlbumToRemove = db.SongAlbums
                                        .Where(sa => sa.AlbumId == songAlbum.AlbumId && sa.SongId == songAlbum.SongId)
                                        .FirstOrDefault();
                if (songAlbumToRemove != null)
                {
                    db.SongAlbums.Remove(songAlbumToRemove);
                    db.SaveChanges();
                    TempData["message"] = "The song was deleted from the album";
                    TempData["messageType"] = " alert alert-success";
                    return Redirect("/Albums/Show/" + album.Id);
                }
                else
                {
                    TempData["message"] = "The song was not found in the album";
                    TempData["messageType"] = " alert alert-danger";
                    return Redirect("/Albums/Show/" + album.Id);
                }
            }
            else
            {
                TempData["message"] = "You don't have the right to remove the song from this album";
                return Redirect("/Albums/Show/" + album.Id);
            }
        }

        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


    }



}
