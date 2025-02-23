using FreeMusicInstantly.Data;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;



namespace proiectDAW.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private const string TempDataMessageKey = "message";
        private const string TempDataMessageTypeKey = "messageType";

        private const string viewUserString = "ViewUsers";
        private const string adminString = "Admin";
        private const string alertSuccessString = "alert alert-success";
        private const string alertDangerString = "alert alert-danger";

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

        [HttpPost]
        public IActionResult CSRFProtectionTest()
        {
            // this method is used only in testing to check if the CSRF protection is working
            return base.Content("Request was allowed");
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<IActionResult> MyProfile()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            ViewBag.CurrentUserId = currentUserId;
            if (currentUser == null)
            {
                TempData[TempDataMessageKey] = "Your profile was not found";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction("Index");
            }
            ViewBag.EsteArtist = User.IsInRole("Artist");
            if (TempData.TryGetValue(TempDataMessageKey, out var msg))
            {
                ViewBag.Msg = msg;
            }

            if (TempData.TryGetValue(TempDataMessageTypeKey, out var msgType))
            {
                ViewBag.MsgType = msgType;
            }

            return View(currentUser);
        }


        [Authorize(Roles = "Admin,User,Artist")]
        public IActionResult ViewUsers(string? search)
        {
            var artistRoleName = "User";
            var userId = _userManager.GetUserId(User);
            var users = from user in db.Users
                        join userRole in db.UserRoles on user.Id equals userRole.UserId
                        join role in db.Roles on userRole.RoleId equals role.Id
                        join friendship in db.Friendships on new { A = userId, B = user.Id } equals new { A = friendship.User1Id, B = friendship.User2Id } into userFriendships1
                        from friendship1 in userFriendships1.DefaultIfEmpty()
                        join friendship in db.Friendships on new { A = userId, B = user.Id } equals new { A = friendship.User2Id, B = friendship.User1Id } into userFriendships2
                        from friendship2 in userFriendships2.DefaultIfEmpty()
                        join sentRequest in db.FriendRequests on new { A = userId, B = user.Id } equals new { A = sentRequest.SenderId, B = sentRequest.ReceiverId } into userSentRequests
                        from sentRequest in userSentRequests.DefaultIfEmpty()
                        join receivedRequest in db.FriendRequests on new { A = userId, B = user.Id } equals new { A = receivedRequest.ReceiverId, B = receivedRequest.SenderId } into userReceivedRequests
                        from receivedRequest in userReceivedRequests.DefaultIfEmpty()
                        where role.Name == artistRoleName && user.Id != userId
                        orderby user.UserName
                        let friendshipId = friendship1 != null ? friendship1.Id : friendship2 != null ? friendship2.Id : (int?)null // Extracted statement
                        select new
                        {
                            User = user,
                            FriendshipId = friendshipId, // Use the extracted variable here
                            SentRequestId = sentRequest != null ? sentRequest.Id : (int?)null,
                            ReceivedRequestId = receivedRequest != null ? receivedRequest.Id : (int?)null,
                        };


            if (!string.IsNullOrWhiteSpace(search)) 
            {
                search = search.Trim();

                if (users != null)
                {
                    users = users.Where(u => u.User != null && u.User.UserName != null && u.User.UserName.Contains(search));
                }
            }





            ViewBag.SearchString = search ?? "";
            ViewBag.UsersList = users;
            ViewBag.EsteAdmin = User.IsInRole(adminString);
            ViewBag.FromFriendsList = false;
            if (TempData.TryGetValue(TempDataMessageKey, out var msg))
            {
                ViewBag.Msg = msg;
            }

            if (TempData.TryGetValue(TempDataMessageTypeKey, out var msgType))
            {
                ViewBag.MsgType = msgType;
            }

            return View(users);
        }

        [Authorize(Roles = "Admin,User,Artist")]
        public IActionResult ViewFriends(string? search)
        {
            var userId = _userManager.GetUserId(User);
            var friends_ = from friendship in db.Friendships
                          join user1 in db.Users on friendship.User1Id equals user1.Id
                          join user2 in db.Users on friendship.User2Id equals user2.Id
                          where friendship.User1Id == userId || friendship.User2Id == userId
                          select new
                          {
                              User1 = user1,
                              User2 = user2,
                              FriendshipId = friendship.Id,
                          };
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                friends_ = friends_.Where(f =>
                    (f.User1 != null && f.User1.Id == userId && f.User2 != null && f.User2.UserName != null && f.User2.UserName.Contains(search)) ||
                    (f.User1 != null && f.User1.UserName != null && f.User1.UserName.Contains(search))
                );
            }


            var friends = friends_.Select(f => new {
                User = f.User1.Id == userId ? f.User2 : f.User1,
                f.FriendshipId,
                SentRequestId = (int?)null,
                ReceivedRequestId = (int?)null,
            });
            ViewBag.SearchString = search ?? "";
            ViewBag.UsersList = friends;
            ViewBag.EsteAdmin = User.IsInRole(adminString);
            ViewBag.CurrentUserId = userId;
            ViewBag.FromFriendsList = true;
            if (TempData.TryGetValue(TempDataMessageKey, out var msg))
            {
                ViewBag.Msg = msg;
            }

            if (TempData.TryGetValue(TempDataMessageTypeKey, out var msgType))
            {
                ViewBag.MsgType = msgType;
            }

            return View(viewUserString, friends);
        }

        [Authorize(Roles = "Admin,User,Artist")]
        public IActionResult ViewFriendRequests()
        {
            var userId = _userManager.GetUserId(User);
            var friendRequests = from request in db.FriendRequests
                                 join user in db.Users on request.SenderId equals user.Id
                                 where request.ReceiverId == userId
                                 select new
                                 {
                                     User = user,
                                     RequestId = request.Id,
                                 };
            ViewBag.FriendRequests = friendRequests;
            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                ViewBag.MsgType = TempData.TryGetValue(TempDataMessageTypeKey, out var msgType) ? msgType : null;
            }

            return View();
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult SendFriendRequest(string userId) {
            var currentUserId = _userManager.GetUserId(User);

            var friendRequest = new FriendRequest
            {
                SenderId = currentUserId,
                ReceiverId = userId,
            };
            db.FriendRequests.Add(friendRequest);
            db.SaveChanges();

            TempData[TempDataMessageKey] = "Friend request sent!";
            TempData[TempDataMessageTypeKey] = alertSuccessString;
            return RedirectToAction(viewUserString);
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult AcceptFriendRequest(int requestId, bool? fromFriendRequests) {
            var request = db.FriendRequests.Find(requestId);
            var redirect = (fromFriendRequests ?? false) ? "ViewFriendRequests" : viewUserString;

            if (request == null || request.ReceiverId == null || request.ReceiverId != _userManager.GetUserId(User))
            {
                TempData[TempDataMessageKey] = "Not authorized to accept this friend request!";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction(redirect);
            }



            db.FriendRequests.Remove(request);

            var friendship = new Friendship
            {
                User1Id = request.SenderId,
                User2Id = request.ReceiverId,
            };
            db.Friendships.Add(friendship);
            db.SaveChanges();

            TempData[TempDataMessageKey] = "Friend request accepted!";
            TempData[TempDataMessageTypeKey] = alertSuccessString;
            return RedirectToAction(redirect);
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult CancelFriendRequest([FromBody] int requestId)
        {
            if (!ModelState.IsValid)
            {
                TempData[TempDataMessageKey] = "Invalid request!";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction(viewUserString);
            }

            var request = db.FriendRequests.Find(requestId);

            if (request == null)
            {
                TempData[TempDataMessageKey] = "Friend request not found!";
                TempData[TempDataMessageTypeKey] = "alert alert-warning";
                return RedirectToAction(viewUserString);
            }

            if (request.SenderId != _userManager.GetUserId(User))
            {
                TempData[TempDataMessageKey] = "Not authorized to cancel this friend request!";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction(viewUserString);
            }

            db.FriendRequests.Remove(request);
            db.SaveChanges();

            TempData[TempDataMessageKey] = "Friend request canceled!";
            TempData[TempDataMessageTypeKey] = alertSuccessString;
            return RedirectToAction(viewUserString);
        }


        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult DeclineFriendRequest(int requestId) {
            var request = db.FriendRequests.Find(requestId);

            if (request == null || request.ReceiverId != _userManager.GetUserId(User))
            {
                TempData[TempDataMessageKey] = "Not authorized to decline this friend request!";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction("ViewFriendRequests");
            }


            db.FriendRequests.Remove(request);
            db.SaveChanges();

            TempData[TempDataMessageKey] = "Friend request declined!";
            TempData[TempDataMessageTypeKey] = alertSuccessString;
            return RedirectToAction("ViewFriendRequests");
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult RemoveFriend(int friendshipId, bool fromFriendsList) {
            var redirect = fromFriendsList ? "ViewFriends" : viewUserString;
            var friendship = db.Friendships.Find(friendshipId);
            var userId = _userManager.GetUserId(User);

            if (friendship == null || (friendship.User1Id != userId && friendship.User2Id != userId))
{
    TempData[TempDataMessageKey] = "Not authorized to delete this friendship!";
    TempData[TempDataMessageTypeKey] = alertDangerString;
    return RedirectToAction(redirect);
}


            db.Friendships.Remove(friendship);
            db.SaveChanges();

            TempData[TempDataMessageKey] = "Friend removed!";
            TempData[TempDataMessageTypeKey] = alertSuccessString;
            return RedirectToAction(redirect);
        }

        [Authorize(Roles = "Admin,Artist,User")]
        public IActionResult ViewArtists()
        {
            var artistRoleName = "Artist";
            var search = "";
            var currentUserId = _userManager.GetUserId(User);
            var artists = from user in db.Users
                                    join userRole in db.UserRoles on user.Id equals userRole.UserId
                                    join role in db.Roles on userRole.RoleId equals role.Id
                                    where role.Name == artistRoleName && user.Id != currentUserId
                                    orderby user.UserName
                                    select user;
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                artists = from user in db.Users
                          join userRole in db.UserRoles on user.Id equals userRole.UserId
                          join role in db.Roles on userRole.RoleId equals role.Id
                          where role.Name == artistRoleName && user.UserName != null && user.UserName.Contains(search)
                          orderby user.UserName
                          select user;


            }
            ViewBag.SearchString = search;
            ViewBag.UsersList = artists;
            ViewBag.EsteAdmin = User.IsInRole(adminString);
            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;

                ViewBag.MsgType = TempData.TryGetValue(TempDataMessageTypeKey, out var msgType) ? msgType : null;
            }


            return View(artists);
        }



        [Authorize(Roles = "User,Admin,Artist")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;
            ViewBag.EsteAdmin = User.IsInRole(adminString);

            if (TempData.TryGetValue(TempDataMessageKey, out var message))
            {
                ViewBag.Msg = message;
                TempData.TryGetValue(TempDataMessageTypeKey, out var msgType);
                ViewBag.MsgType = msgType; 
            }

            return View();
        }




        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<ActionResult> Show(string id)
        {
            ViewBag.EsteAdmin = User.IsInRole(adminString);
            ViewBag.UserCurent = _userManager.GetUserId(User);

            var user = await db.Users.FindAsync(id) as ApplicationUser;

            if (user == null)
            {
                return NotFound(); 
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }


        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<ActionResult> ShowArtist(string id)
        {

            ViewBag.EsteAdmin = User.IsInRole(adminString);
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteArtist = User.IsInRole("Artist");

            var user = await db.Users.FindAsync(id) as ApplicationUser;

            if (user == null)
            {
                return NotFound();
            }
            var artistSongs = db.Songs.Where(s => s.UserId == id);
            var artistAlbums = db.Albums.Where(a => a.UserId == id);
            foreach (Album c in artistAlbums)
            {
                c.NrSongs = await db.SongAlbums.Where(a => a.AlbumId == c.Id).CountAsync();

            }
            ViewBag.artistSongs = artistSongs;
            ViewBag.artistAlbums = artistAlbums;

            return View(user);
        }

        [Authorize(Roles = "User,Admin,Artist")]
        public async Task<ActionResult> Edit()
        {
            var currentUserId = _userManager.GetUserId(User);

            var user = await db.Users.FindAsync(currentUserId) as ApplicationUser;

            if (user == null)
            {
                return NotFound(); 
            }

            ViewBag.CurrentUserId = currentUserId;

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user);

            var currentUserRole = await _roleManager.Roles
                .Where(r => r.Name != null && (roleNames ?? new List<string>()).Contains(r.Name))
                .Select(r => r.Id)
                .FirstOrDefaultAsync();




            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [Authorize(Roles = "User,Admin,Artist")]
        [HttpPost]
        public async Task<ActionResult> Edit( ApplicationUser newData)
        {
            var currentUserId = _userManager.GetUserId(User);

            var user = await db.Users.FindAsync(currentUserId) as ApplicationUser;

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.CurrentUserId = currentUserId;
            if (user == null)
            {
                TempData[TempDataMessageKey] = "User not found.";
                TempData[TempDataMessageTypeKey] = alertDangerString;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.PhoneNumber = newData.PhoneNumber;
                user.MyMusicDescription = newData.MyMusicDescription;
                user.Biography = newData.Biography;


                await db.SaveChangesAsync();

                TempData[TempDataMessageKey] = "The user was edited!";
                TempData[TempDataMessageTypeKey] = alertSuccessString;
            }
            else
            {
                TempData[TempDataMessageKey] = "Error when editing the user";
                TempData[TempDataMessageTypeKey] = alertSuccessString;
            }

            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            // Associated SongAlbum and SongPlaylist entries are deleted by cascade
            db.Songs.RemoveRange(db.Songs.Where(s => s.UserId == id));
            db.Albums.RemoveRange(db.Albums.Where(a => a.UserId == id));
            db.Playlists.RemoveRange(db.Playlists.Where(p => p.UserId == id));
            db.FriendRequests.RemoveRange(db.FriendRequests.Where(fr => fr.SenderId == id || fr.ReceiverId == id));
            db.Friendships.RemoveRange(db.Friendships.Where(f => f.User1Id == id || f.User2Id == id));
            db.Comments.RemoveRange(db.Comments.Where(c => c.UserId == id));

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
                if (role != null) // Check if role is not null
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = role.Id?.ToString() ?? string.Empty, // Safely handle potential null value
                        Text = role.Name ?? string.Empty // Handle potential null value
                    });
                }
            }

            return selectList;
        }
        /* to be implemented
        -delete method to be completed
        -show songs and albums for artist
        -show playlists for users
        */


        [Authorize(Roles = "Artist, Admin")]
        public async Task<IActionResult> ArtistSongStatistics()
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentDate = DateTime.UtcNow;

            // Get artist's songs
            var artistSongs = await db.Songs
                .Where(s => s.UserId == currentUserId)
                .ToListAsync();

            if (artistSongs.Count == 0)
            {
                TempData[TempDataMessageKey] = "You have no songs.";
                TempData[TempDataMessageTypeKey] = "alert alert-warning";
                return View(new List<object>());
            }

            var songIds = artistSongs.Select(s => s.Id).ToList();

            // Get the total number of plays & likes in the last year & month
            var lastYear = currentDate.AddYears(-1);
            var lastMonth = currentDate.AddMonths(-1);

            var totalPlaysLastYear = await db.Plays
                .Where(p => songIds.Contains(p.SongId) && p.PlayTime >= lastYear)
                .CountAsync();

            var totalPlaysLastMonth = await db.Plays
                .Where(p => songIds.Contains(p.SongId) && p.PlayTime >= lastMonth)
                .CountAsync();

            var totalLikesLastYear = await db.Likes
                .Where(l => songIds.Contains(l.SongId) && l.LikeDate >= lastYear)
                .CountAsync();

            var totalLikesLastMonth = await db.Likes
                .Where(l => songIds.Contains(l.SongId) && l.LikeDate >= lastMonth)
                .CountAsync();

            // Fetch individual song statistics
            var playCounts = await db.Plays
                .Where(p => songIds.Contains(p.SongId))
                .GroupBy(p => p.SongId)
                .Select(g => new { SongId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SongId, x => x.Count);

            var likeCounts = await db.Likes
                .Where(l => songIds.Contains(l.SongId))
                .GroupBy(l => l.SongId)
                .Select(g => new { SongId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SongId, x => x.Count);

            var playlistCounts = await db.SongPlaylists
                .Where(sp => sp.SongId.HasValue && songIds.Contains(sp.SongId.Value))
                .GroupBy(sp => sp.SongId.Value) // Use sp.SongId.Value to ensure it's non-nullable
                .Select(g => new { SongId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SongId, x => x.Count);


            var songStatistics = artistSongs.Select(song => new
            {
                SongTitle = song.Title,
                PlaylistCount = playlistCounts.TryGetValue(song.Id, out int playlistCount) ? playlistCount : 0,
                PlayCount = playCounts.TryGetValue(song.Id, out int playCount) ? playCount : 0,
                LikeCount = likeCounts.TryGetValue(song.Id, out int likeCount) ? likeCount : 0
            }).ToList();

            // Pass data to ViewBag for visualization
            ViewBag.TotalPlaysLastYear = totalPlaysLastYear;
            ViewBag.TotalPlaysLastMonth = totalPlaysLastMonth;
            ViewBag.TotalLikesLastYear = totalLikesLastYear;
            ViewBag.TotalLikesLastMonth = totalLikesLastMonth;
            ViewBag.SongStatistics = songStatistics;

            return View(songStatistics);
        }










    }
}