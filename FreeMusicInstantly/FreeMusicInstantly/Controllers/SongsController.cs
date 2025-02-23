using FreeMusicInstantly.Data;
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

        private const string TempDataMessageKey = "message";
        private const string TempDataMessageTypeKey = "messageType";
        private const string pathShowString = "/Songs/Show/";
        private const string indexString = "Index";
        private const string alertString = "alert alert-danger";
        private const string dangerAlertString = "alert-danger";
        public SongsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
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
                if (!string.IsNullOrEmpty(search))
                {
                    List<int> SongIds = db.Songs
                                           .Where(song => song.Title != null && song.Title.Contains(search))
                                           .Select(song => song.Id)
                                           .ToList();

                    songs = db.Songs
                          .Where(song => SongIds.Contains(song.Id))
                          .Include("User")
                          .OrderBy(song => song.Title);
                }
              
            }

            ViewBag.Songs = songs;
            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                TempData.TryGetValue(TempDataMessageTypeKey, out var msgType);
                ViewBag.MsgType = msgType; 
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

            ViewBag.PaginationBaseUrl = search != "" ? $"/Songs/Index/?search={search}&page" : "/Songs/Index/?page";

            return View();

        }

        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Show(int id)
        {

            if (!ModelState.IsValid)
            {
                return View("Error"); 
            }

            var song = db.Songs
                    .Include(s => s.User)
                    .Include(s => s.Comments)
                        .ThenInclude(c => c.User) // No cast needed; ensure User is defined in Comment
                    .Include(s => s.Plays)
                    .Include(s => s.Likes)
                    .FirstOrDefault(s => s.Id == id);


            if (song == null)
            {
                return NotFound();
            }

            ViewBag.SongGroups = null;
            ViewBag.TotalPlays = song.Plays?.Count ?? 0;
            ViewBag.LikesCount = song.Likes?.Count ?? 0;

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

            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                TempData.TryGetValue(TempDataMessageTypeKey, out var msgType);
                ViewBag.MsgType = msgType;
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
                if (comment.Text != null)
                {
                    comment.Text = new HtmlSanitizer().Sanitize(comment.Text);
                }
                else
                {
                    comment.Text = string.Empty;
                }

                db.Comments.Add(comment);
                db.SaveChanges();
                TempData[TempDataMessageKey] = "The comment was successfully added";
                TempData[TempDataMessageTypeKey] = " alert alert-success";
                return Redirect(pathShowString + comment.SongId);
            }

            else
            {
                Song bmk = db.Songs.Include("User")
                                   .Include("Comments")
                                   .Include("Comments.User")
                                   .Where(b => b.Id == comment.SongId)
                                   .First();


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
                cat.Title = sanitizer.Sanitize(cat.Title ?? string.Empty);

                cat.Description = sanitizer.Sanitize(cat.Description ?? string.Empty);


                db.Songs.Add(cat);
                await db.SaveChangesAsync();
                TempData[TempDataMessageKey] = "The song was added";
                TempData[TempDataMessageTypeKey] = "alert alert-success";
                return RedirectToAction(indexString);
            }
            else
            {
                TempData[TempDataMessageKey] = "The song was not added successfully";
                TempData[TempDataMessageTypeKey] = " alert alert-danger";
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
                TempData[TempDataMessageKey] = "You don't have the right to edit a song that doesn't belong to you";
                TempData[TempDataMessageTypeKey] = alertString;
                return RedirectToAction("Show", new { id = id });
            }

        }
        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Song reqcat, IFormFile? SongFile)
        {
            Song? cat = await db.Songs.Where(a => a.Id == id).FirstOrDefaultAsync();


            if (cat == null)
            {
                TempData[TempDataMessageKey] = "The song does not exist.";
                TempData[TempDataMessageTypeKey] = alertString;
                return RedirectToAction(indexString);
            }


            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                cat.Title = sanitizer.Sanitize(cat.Title ?? string.Empty);

                cat.Description = sanitizer.Sanitize(cat.Description ?? string.Empty);


                if (cat.UserId == _userManager.GetUserId(User))
                {
                    
                    cat.Title = reqcat.Title;
                    cat.Description = reqcat.Description;
                   

                    if (SongFile != null)
                    {

                        if (string.Equals(Path.GetExtension(SongFile.FileName), ".mp3", StringComparison.OrdinalIgnoreCase))
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
                            TempData[TempDataMessageKey] = "Invalid file type. Please upload an MP3 file.";
                            TempData[TempDataMessageTypeKey] = alertString;
                            return View(reqcat);
                        }
                    }

                    await db.SaveChangesAsync();

                    TempData[TempDataMessageKey] = "The song was edited successfully.";
                    TempData[TempDataMessageTypeKey] = "alert alert-success";
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    TempData[TempDataMessageKey] = "You don't have the right to edit a song that doesn't belong to you.";
                    TempData[TempDataMessageTypeKey] = alertString;
                    return RedirectToAction("Show", new { id = id });
                }
            }
            else
            {
                TempData[TempDataMessageKey] = "The song was not edited successfully. Please correct the errors.";
                TempData[TempDataMessageTypeKey] = alertString;
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
                TempData[TempDataMessageKey] = "The song was deleted";
                TempData[TempDataMessageTypeKey] = " alert alert-success";
                return RedirectToAction(indexString);
            }
            else
            {
                TempData[TempDataMessageKey] = "You don't have the right to delete a song that doesn't belong to you";
                TempData[TempDataMessageTypeKey] = alertString;
                return RedirectToAction(indexString);
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
                    TempData[TempDataMessageKey] = "Song already added to playlist!";
                    TempData[TempDataMessageTypeKey] = dangerAlertString;
                }
                else
                {
                    db.SongPlaylists.Add(songPlaylist);
                    db.SaveChanges();
                    TempData[TempDataMessageKey] = "Song added to playlist!";
                    TempData[TempDataMessageTypeKey] = "alert-success";
                }
            }
            else
            {
                TempData[TempDataMessageKey] = "Could not add song to playlist!";
                TempData[TempDataMessageTypeKey] = dangerAlertString;
            }
            return Redirect(pathShowString + songPlaylist.SongId);
        }

        [Authorize(Roles = "Admin,Artist")]
        [HttpPost]
        public IActionResult AddToAlbum([FromForm] SongAlbum songAlbum)
        {
            SetAccessRights(); 

            if (ModelState.IsValid)
            {
                string userId = ViewBag.UserCurent as string ?? string.Empty;


                bool isUserSongOwner = db.Songs.Any(s => s.Id == songAlbum.SongId && s.UserId == userId);

                if (!isUserSongOwner)
                {
                    TempData[TempDataMessageKey] = "You can only add your own songs to an album!";
                    TempData[TempDataMessageTypeKey] = dangerAlertString;
                    return Redirect(pathShowString + songAlbum.SongId);
                }

                if (db.SongAlbums.Any(sa => sa.SongId == songAlbum.SongId && sa.AlbumId == songAlbum.AlbumId))
                {
                    TempData[TempDataMessageKey] = "Song already added to album!";
                    TempData[TempDataMessageTypeKey] = dangerAlertString;
                }
                else
                {
                    db.SongAlbums.Add(songAlbum);
                    db.SaveChanges();
                    TempData[TempDataMessageKey] = "Song added to album!";
                    TempData[TempDataMessageTypeKey] = "alert-success";
                }
            }
            else
            {
                TempData[TempDataMessageKey] = "Could not add song to album!";
                TempData[TempDataMessageTypeKey] = dangerAlertString;
            }
            return Redirect(pathShowString + songAlbum.SongId);
        }

       

        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


    
}
}
