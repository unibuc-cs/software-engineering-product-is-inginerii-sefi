using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreeMusicInstantly.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string indexString = "Index";
        private const string TempDataMessageKey = "message";
        private const string TempDataMessageTypeKey = "messageType";
        private const string alertDangerString = "alert alert-danger";
        public PlaylistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Index()
        {
            SetAccessRights();

            var playlists = db.Playlists
                .Include(p => p.SongPlaylists)
                    .ThenInclude(sp => sp.Song)
                .Include(p => p.User)
                .Where(x => x.UserId == _userManager.GetUserId(User))
                .ToList(); // Call ToList to execute the query

            ViewBag.Playlists = playlists;
            ViewBag.IsOwner = true;

            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                TempData.TryGetValue(TempDataMessageTypeKey, out var msgType);
                ViewBag.MsgType = msgType;
            }

            return View();
        }



        public IActionResult IndexFriend(string friendId)
        {
            SetAccessRights();
            var currentUserId = _userManager.GetUserId(User);

            var friendship = db.Friendships
                .FirstOrDefault(f => (f.User1Id == currentUserId && f.User2Id == friendId) ||
                                     (f.User1Id == friendId && f.User2Id == currentUserId));

            if (friendship == null)
            {
                TempData[TempDataMessageKey] = "You are not friends with this user!";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction("ViewUsers", "ApplicationUsers");
            }

            var playlists = db.Playlists.Include(p => p.SongPlaylists)
                                         .ThenInclude(sp => sp.Song)
                                         .Include(p => p.User)
                                         .Where(x => x.UserId == friendId);

            ViewBag.Playlists = playlists;
            ViewBag.IsOwner = false;

            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                TempData.TryGetValue(TempDataMessageTypeKey, out var msgType);
                ViewBag.MsgType = msgType;
            }

            return View(indexString);
        }


        [Authorize(Roles = "Admin,Artist,User")]
        public IActionResult New()
        {
            Playlist p = new Playlist();
            return View(p);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Artist,User")]
        public async Task<IActionResult> New(Playlist p, IFormFile? PhotoCover)
        {
            p.UserId = _userManager.GetUserId(User);

            if (PhotoCover != null)
            {
                var fileName = Path.GetFileName(PhotoCover.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoCover.CopyToAsync(fileSteam);
                }
                p.PhotoCover = fileName;
            }
            else
            {
                p.PhotoCover = "default.jpg"; // default image as a black photo
            }

            if (ModelState.IsValid)
            {
                db.Playlists.Add(p);
                await db.SaveChangesAsync();
                TempData[TempDataMessageKey] = "The playlist was added";
                TempData[TempDataMessageTypeKey] = "alert alert-success";
                return RedirectToAction(indexString);
            }
            else
            {
                TempData[TempDataMessageKey] = "The playlist was not added successfully";
                TempData[TempDataMessageTypeKey] = " alert alert-danger";
                return View(p);
            }
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            var userId = _userManager.GetUserId(User);

            var p = db.Playlists
                .Include("SongPlaylists.Song")
                .Include("SongPlaylists.Song.User")
                .Include("User")
                .FirstOrDefault(a => a.Id == id); 

            if (p == null)
            {
                return NotFound(); 
            }

            if (p.UserId != userId)
            {
                var ownerFriendship = db.Friendships
                    .Where(f => (f.User1Id == userId && f.User2Id == p.UserId) ||
                                 (f.User1Id == p.UserId && f.User2Id == userId));

                if (!ownerFriendship.Any())
                {
                    TempData[TempDataMessageKey] = "You don't have access to this playlist!";
                    TempData[TempDataMessageTypeKey] = alertDangerString;
                    return RedirectToAction(indexString);
                }
            }

            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                TempData.TryGetValue(TempDataMessageTypeKey, out var msgType);
                ViewBag.MsgType = msgType; 
            }

            return View(p);
        }


        [Authorize(Roles = "Admin,Artist,User")]
        public IActionResult Edit(int id)
        {
            Playlist p = db.Playlists.Where(a => a.Id == id).First();

            SetAccessRights();

            if (p.UserId == _userManager.GetUserId(User))
            {
                return View(p);
            }

            else
            {
                TempData[TempDataMessageKey] = "You don't have the right to edit a playlist that doesn't belong to you";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction(indexString);
            }   

        }

        [Authorize(Roles = "Admin,Artist,User")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Playlist reqp, IFormFile? PhotoCover)
        {
            Playlist p = await db.Playlists.Where(a => a.Id == id).FirstAsync();

            
            if (ModelState.IsValid)
            {
                if (p.UserId == _userManager.GetUserId(User))
                {
                    p.PlaylistName = reqp.PlaylistName;
                    p.Description = reqp.Description;

                    if (PhotoCover != null)
                    {
                        var fileName = Path.GetFileName(PhotoCover.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", fileName);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await PhotoCover.CopyToAsync(fileSteam);
                        }
                        p.PhotoCover = fileName;
                    }
                    else
                    {
                        if (reqp.PhotoCover != null)
                        {
                            p.PhotoCover = reqp.PhotoCover;
                        }
                        else
                        {
                            if (p.PhotoCover == null)
                            {
                                p.PhotoCover = "default.jpg";
                            }
                        }
                    }

                    db.SongPlaylists.RemoveRange(db.SongPlaylists.Where(a => a.PlaylistId == p.Id));
                    if (reqp.SongPlaylists == null)
                    {
                        reqp.SongPlaylists = new List<SongPlaylist>();
                    }
                    if (p.SongPlaylists == null)
                    {
                        p.SongPlaylists = new List<SongPlaylist>();
                    }
                    p.SongPlaylists.Clear();
                    foreach (var sp in reqp.SongPlaylists)
                    {
                        SongPlaylist songPlaylist = new SongPlaylist();
                        songPlaylist.SongId = sp.SongId;
                        songPlaylist.PlaylistId = p.Id;
                        db.SongPlaylists.Add(songPlaylist);
                        p.SongPlaylists.Add(songPlaylist);
                    }
                    await db.SaveChangesAsync();

                    TempData[TempDataMessageKey] = "The playlist was edited successfully";
                    TempData[TempDataMessageTypeKey] = "alert alert-success";
                    await db.SaveChangesAsync();
                    return RedirectToAction(indexString);
                }
                else
                {
                    TempData[TempDataMessageKey] = "You don't have the right to edit a playlist that doesn't belong to you";
                    TempData[TempDataMessageTypeKey] = alertDangerString;
                    return RedirectToAction(indexString);
                }
            }
            else
            {
                return View(reqp);
            }


        }

        [Authorize(Roles = "Admin,Artist,User")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Playlist p = db.Playlists.Include("SongPlaylists")
                                        .Where(a => a.Id == id).First();
            if (_userManager.GetUserId(User) == p.UserId || User.IsInRole("Admin"))
            {
                if (p.SongPlaylists != null && p.SongPlaylists.Count > 0)
                {
                    foreach (var sp in p.SongPlaylists)
                    {
                        db.SongPlaylists.Remove(sp);
                    }
                }


                db.Playlists.Remove(p);
                db.SaveChanges();
                TempData[TempDataMessageKey] = "The playlist was deleted";
                TempData[TempDataMessageTypeKey] = " alert alert-success";
                return RedirectToAction(indexString);
            }
            else
            {
                TempData[TempDataMessageKey] = "You don't have the right to delete a playlist that doesn't belong to you";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction(indexString);
            }
        }

       
        [Authorize(Roles = "Admin,Artist,User")]
        [HttpPost]
        public IActionResult RemoveSong([FromForm] SongPlaylist songPlaylist)
        {
            Playlist playlist = db.Playlists.Include("SongPlaylists")
                              .Where(a => a.Id == songPlaylist.PlaylistId).First();
            if (_userManager.GetUserId(User) == playlist.UserId)
            {
                var songPlaylistToRemove = db.SongPlaylists
                    .Where(sp => sp.PlaylistId == songPlaylist.PlaylistId && sp.SongId == songPlaylist.SongId)
                    .FirstOrDefault();
                if (songPlaylistToRemove != null)
                {
                    db.SongPlaylists.Remove(songPlaylistToRemove);
                    db.SaveChanges();
                    TempData[TempDataMessageKey] = "The song was deleted from the playlist";
                    TempData[TempDataMessageTypeKey] = " alert alert-success";
                    return Redirect("/Playlists/Show/" + playlist.Id);
                }
                else
                {
                    TempData[TempDataMessageKey] = "The song was not found in the playlist";
                    TempData[TempDataMessageTypeKey] = " alert alert-danger";
                    return Redirect("/Playlists/Show/" + playlist.Id);
                }
            }
            else
            {
                TempData[TempDataMessageKey] = "You don't have the right to remove the song from this playlist";
                return Redirect("/Playlists/Show/" + playlist.Id);
            }
        }


        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
