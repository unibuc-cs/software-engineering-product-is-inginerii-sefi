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


        [Authorize(Roles = "Admin,User,Artist")]
        public IActionResult ViewUsers(string? search)
        {
            var artistRoleName = "User";
            var userId = _userManager.GetUserId(User);
            var users = from user in db.Users
                          join userRole in db.UserRoles on user.Id equals userRole.UserId
                          join role in db.Roles on userRole.RoleId equals role.Id
                          join friendship in db.Friendships on new { A=userId, B=user.Id } equals new { A=friendship.User1Id, B=friendship.User2Id } into userFriendships1
                          from friendship1 in userFriendships1.DefaultIfEmpty()
                          join friendship in db.Friendships on new { A=userId, B=user.Id } equals new { A=friendship.User2Id, B=friendship.User1Id } into userFriendships2
                          from friendship2 in userFriendships2.DefaultIfEmpty()
                          join sentRequest in db.FriendRequests on new { A=userId, B=user.Id } equals new { A=sentRequest.SenderId, B=sentRequest.ReceiverId } into userSentRequests
                          from sentRequest in userSentRequests.DefaultIfEmpty()
                          join receivedRequest in db.FriendRequests on new { A=userId, B=user.Id } equals new { A=receivedRequest.ReceiverId, B=receivedRequest.SenderId } into userReceivedRequests
                          from receivedRequest in userReceivedRequests.DefaultIfEmpty()
                          where role.Name == artistRoleName && user.Id != userId
                          orderby user.UserName
                          select new
                          {
                              User = user,
                              FriendshipId = friendship1 != null ? friendship1.Id : friendship2 != null ? friendship2.Id : (int?)null,
                              SentRequestId = sentRequest != null ? sentRequest.Id : (int?)null,
                              ReceivedRequestId = receivedRequest != null ? receivedRequest.Id : (int?)null,
                          };

            if (search != null)
            {
                search = search.Trim();
                users = users.Where(u => u.User.UserName.Contains(search));
            }

            ViewBag.SearchString = search ?? "";
            ViewBag.UsersList = users;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.FromFriendsList = false;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
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
            if(search != null) {
                search = search.Trim();
                friends_ = friends_.Where(f => f.User1.Id == userId ? f.User2.UserName.Contains(search) : f.User1.UserName.Contains(search));
            }
            var friends = friends_.Select(f => new {
                User = f.User1.Id == userId ? f.User2 : f.User1,
                f.FriendshipId,
                SentRequestId = (int?)null,
                ReceivedRequestId = (int?)null,
            });
            ViewBag.SearchString = search ?? "";
            ViewBag.UsersList = friends;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.CurrentUserId = userId;
            ViewBag.FromFriendsList = true;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }
            return View("ViewUsers", friends);
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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
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

            TempData["message"] = "Friend request sent!";
            TempData["messageType"] = "alert alert-success";
            return RedirectToAction("ViewUsers");
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult AcceptFriendRequest(int requestId, bool? fromFriendRequests) {
            var request = db.FriendRequests.Find(requestId);
            var redirect = (fromFriendRequests ?? false) ? "ViewFriendRequests" : "ViewUsers";

            if (request.ReceiverId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Not authorized to accept this friend request!";
                TempData["messageType"] = "alert alert-danger";
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

            TempData["message"] = "Friend request accepted!";
            TempData["messageType"] = "alert alert-success";
            return RedirectToAction(redirect);
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult CancelFriendRequest(int requestId) {
            var request = db.FriendRequests.Find(requestId);

            if (request.SenderId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Not authorized to cancel this friend request!";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("ViewUsers");
            }

            db.FriendRequests.Remove(request);
            db.SaveChanges();

            TempData["message"] = "Friend request canceled!";
            TempData["messageType"] = "alert alert-success";
            return RedirectToAction("ViewUsers");
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult DeclineFriendRequest(int requestId) {
            var request = db.FriendRequests.Find(requestId);

            if (request.ReceiverId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Not authorized to decline this friend request!";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("ViewFriendRequests");
            }

            db.FriendRequests.Remove(request);
            db.SaveChanges();

            TempData["message"] = "Friend request declined!";
            TempData["messageType"] = "alert alert-success";
            return RedirectToAction("ViewFriendRequests");
        }

        [Authorize(Roles = "Admin,User,Artist")]
        [AcceptVerbs("Post")]
        public IActionResult RemoveFriend(int friendshipId, bool fromFriendsList) {
            var redirect = fromFriendsList ? "ViewFriends" : "ViewUsers";
            var friendship = db.Friendships.Find(friendshipId);
            var userId = _userManager.GetUserId(User);

            if (friendship.User1Id != userId && friendship.User2Id != userId)
            {
                TempData["message"] = "Not authorized to delete this friendship!";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction(redirect);
            }

            db.Friendships.Remove(friendship);
            db.SaveChanges();

            TempData["message"] = "Friend removed!";
            TempData["messageType"] = "alert alert-success";
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
                artists = (from user in db.Users
                         join userRole in db.UserRoles on user.Id equals userRole.UserId
                         join role in db.Roles on userRole.RoleId equals role.Id
                         where role.Name == artistRoleName && user.UserName.Contains(search)
                         orderby user.UserName
                         select user);

            }
            ViewBag.SearchString = search;
            ViewBag.UsersList = artists;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
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
        public async Task<ActionResult> ShowArtist(string id)
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteArtist = User.IsInRole("Artist");

            ApplicationUser user = (ApplicationUser)db.Users.Find(id);

            var artistSongs = db.Songs.Where(s => s.UserId == id);
            var artistAlbums = db.Albums.Where(a => a.UserId == id);
            foreach (Album c in artistAlbums)
            {
                c.NrSongs = db.SongAlbums.Where(a => a.AlbumId == c.Id).Count();
            }
            ViewBag.artistSongs = artistSongs;
            ViewBag.artistAlbums = artistAlbums;

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
         -delete method to be completed
         -show songs and albums for artist
         -show playlists for users
         */

    }
}