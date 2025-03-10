﻿using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Index()
        {
            var sanitizer = new HtmlSanitizer();
            var search = "";

            var songs = db.Songs.Include("User");

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                search = sanitizer.Sanitize(search);
                List<int> SongIds = db.Songs
                                       .Where(song => song.Title.Contains(search))
                                       .Select(song => song.Id)
                                       .ToList();
                songs = db.Songs
                            .Where(song => SongIds.Contains(song.Id))
                            .Include("User")
                            .OrderBy(song => song.Title);
            }

            ViewBag.Songs = songs;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }
            ViewBag.SearchString = search;
            int _perPage = 6;
            int totalItems = songs.Count();


            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            ViewBag.PaginaCurenta = currentPage;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedSongs = songs.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Songs = paginatedSongs;

            SetAccessRights();

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Songs/Index/?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Songs/Index/?page";
            }

            return View();
            //var songs = db.Songs.Include("User")
            //                    .Include(s => s.SongAlbums)
            //                    .ThenInclude(sa => sa.Album)
            //                    .OrderBy(p => p.Title)
            //                    .ToList();
            //int perPage = 9;
            //string search = "";
            //if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            //{   search = Convert.ToString(HttpContext.Request.Query["search"]);
            //    songs = db.Songs.Include("User")
            //                    .Include(s => s.SongAlbums)
            //                    .ThenInclude(sa => sa.Album)
            //                    .Where(p => p.Title.Contains(search))
            //                    .OrderBy(p => p.Title)
            //                    .ToList();
            //}
            //ViewBag.SearchString = search;

            //var totalItems = songs.Count();
            //var page = Convert.ToInt32(HttpContext.Request.Query["page"]);
            //var offset = 0;
            //if(!page.Equals(0))
            //{
            //    offset = (page - 1) * perPage;
            //}
            //var paginatedSongs = songs.Skip(offset).Take(perPage).ToList();
            //for (int i = 0; i < paginatedSongs.Count(); i++)
            //{
            //    paginatedSongs[i].SongAlbums = paginatedSongs[i].SongAlbums.Take(1).ToList();
            //}
            //ViewBag.Songs = paginatedSongs;
            //ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)perPage);

            //if (search != "")
            //{
            //    ViewBag.PaginationBaseUrl = "/Songs/Index/?search=" + search + "&page";
            //}
            //else
            //{
            //    ViewBag.PaginationBaseUrl = "/Songs/Index/?page";
            //}
            //return View();
        }
        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Show(int id)
        {
            var song = db.Songs
                         .Include(s => s.User)
                         .Include(s => s.Comments)
                         .ThenInclude(c => c.User)
                         .Include(s => s.Plays)
                         .Include(s => s.Likes)
                         .FirstOrDefault(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            ViewBag.SongGroups = null;
            ViewBag.TotalPlays = song.Plays?.Count() ?? 0;
            ViewBag.LikesCount = song.Likes?.Count() ?? 0;

            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("User"))
            {
                ViewBag.SongGroups = db.Playlists.Include(p => p.User)
                                                 .Where(p => p.UserId == userId)
                                                 .ToList();
            }
            else if (User.IsInRole("Artist"))
            {
                ViewBag.SongGroups = db.Albums.Include(a => a.User)
                                              .Where(a => a.UserId == userId)
                                              .ToList();
            }
            else if (User.IsInRole("Admin"))
            {
                ViewBag.SongGroupsPlaylists = db.Playlists.Include(p => p.User).ToList();
                ViewBag.SongGroupsAlbums = db.Albums.Include(a => a.User).ToList();
            }

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }

            return View(song);
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                comment.Text = new HtmlSanitizer().Sanitize(comment.Text);
                db.Comments.Add(comment);
                db.SaveChanges();
                TempData["message"] = "The comment was successfully added";
                TempData["messageType"] = " alert alert-success";
                return Redirect("/Songs/Show/" + comment.SongId);
            }

            else
            {
                Song bmk = db.Songs.Include("User")
                                   .Include("Comments")
                                   .Include("Comments.User")
                                   .Where(b => b.Id == comment.SongId)
                                   .First();

                //ViewBag.UserCategories = db.Categories
                //                          .Where(b => b.UserId == _userManager.GetUserId(User))
                //                          .ToList();

                SetAccessRights();

                return View(bmk);
            }
        }


        [Authorize(Roles = "Admin,Artist")]
        public IActionResult New()
        {
            Song cat = new Song();
            return View(cat);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Artist")]
        public async Task<IActionResult> New(Song cat, IFormFile? SongFile)
        {
            
            cat.UserId = _userManager.GetUserId(User);


            if (SongFile != null)
            {
                var fileName = Path.GetFileName(SongFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await SongFile.CopyToAsync(fileSteam);
                }
                cat.SongFile = fileName;
            }
            else
            {
                cat.SongFile = "nice_file.mp3"; 
            }


            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                cat.Title = sanitizer.Sanitize(cat.Title);
                cat.Description = sanitizer.Sanitize(cat.Description);

                db.Songs.Add(cat);
                db.SaveChanges();
                TempData["message"] = "The song was added";
                TempData["messageType"] = "alert alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "The song was not added successfully";
                TempData["messageType"] = " alert alert-danger";
                return View(cat);
            }
        }


        [Authorize(Roles = "Admin,Artist")]
        public IActionResult Edit(int id)
        {
            Song cat = db.Songs.Where(a => a.Id == id).First();

            SetAccessRights();

            if (cat.UserId == _userManager.GetUserId(User))
            {
                return View(cat);
            }

            else
            {
                TempData["message"] = "You don't have the right to edit a song that doesn't belong to you";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Show", new { id = id });
            }

        }
        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Song reqcat, IFormFile? SongFile)
        {
            Song cat = db.Songs.Where(a => a.Id == id).FirstOrDefault();


            if (cat == null)
            {
                TempData["message"] = "The song does not exist.";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }


            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                cat.Title = sanitizer.Sanitize(cat.Title);
                cat.Description = sanitizer.Sanitize(cat.Description);

                if (cat.UserId == _userManager.GetUserId(User))
                {
                    
                    cat.Title = reqcat.Title;
                    cat.Description = reqcat.Description;
                   

                    if (SongFile != null)
                    {
                        
                        if (Path.GetExtension(SongFile.FileName).ToLower() == ".mp3")
                        {
                            var fileName = Path.GetFileName(SongFile.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);

                           
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await SongFile.CopyToAsync(fileStream);
                            }

                           
                            cat.SongFile = fileName;
                        }
                        else
                        {
                            TempData["message"] = "Invalid file type. Please upload an MP3 file.";
                            TempData["messageType"] = "alert alert-danger";
                            return View(reqcat);
                        }
                    }

                    db.SaveChanges();

                    TempData["message"] = "The song was edited successfully.";
                    TempData["messageType"] = "alert alert-success";
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    TempData["message"] = "You don't have the right to edit a song that doesn't belong to you.";
                    TempData["messageType"] = "alert alert-danger";
                    return RedirectToAction("Show", new { id = id });
                }
            }
            else
            {
                TempData["message"] = "The song was not edited successfully. Please correct the errors.";
                TempData["messageType"] = "alert alert-danger";
                return View(reqcat);
            }
        }


        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Song cat = db.Songs.Where(a => a.Id == id).First();
            if (_userManager.GetUserId(User) == cat.UserId || User.IsInRole("Admin"))
            {
                db.Songs.Remove(cat);
                db.SaveChanges();
                TempData["message"] = "The song was deleted";
                TempData["messageType"] = " alert alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You don't have the right to delete a song that doesn't belong to you";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public IActionResult AddToPlaylist([FromForm] SongPlaylist songPlaylist)
        {
            if (ModelState.IsValid)
            {
                if (db.SongPlaylists.Any(sp => sp.SongId == songPlaylist.SongId && sp.PlaylistId == songPlaylist.PlaylistId))
                {
                    TempData["message"] = "Song already added to playlist!";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.SongPlaylists.Add(songPlaylist);
                    db.SaveChanges();
                    TempData["message"] = "Song added to playlist!";
                    TempData["messageType"] = "alert-success";
                }
            }
            else
            {
                TempData["message"] = "Could not add song to playlist!";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Songs/Show/" + songPlaylist.SongId);
        }

        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult AddToAlbum([FromForm] SongAlbum songAlbum)
        {
            SetAccessRights(); 

            if (ModelState.IsValid)
            {
                string userId = ViewBag.UserCurent as string;

                bool isUserSongOwner = db.Songs.Any(s => s.Id == songAlbum.SongId && s.UserId == userId);

                if (!isUserSongOwner)
                {
                    TempData["message"] = "You can only add your own songs to an album!";
                    TempData["messageType"] = "alert-danger";
                    return Redirect("/Songs/Show/" + songAlbum.SongId);
                }

                if (db.SongAlbums.Any(sa => sa.SongId == songAlbum.SongId && sa.AlbumId == songAlbum.AlbumId))
                {
                    TempData["message"] = "Song already added to album!";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.SongAlbums.Add(songAlbum);
                    db.SaveChanges();
                    TempData["message"] = "Song added to album!";
                    TempData["messageType"] = "alert-success";
                }
            }
            else
            {
                TempData["message"] = "Could not add song to album!";
                TempData["messageType"] = "alert-danger";
            }
            return Redirect("/Songs/Show/" + songAlbum.SongId);
        }

       

        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


    
}
}
