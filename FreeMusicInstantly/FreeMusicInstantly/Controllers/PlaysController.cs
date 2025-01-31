using Azure;
using FreeMusicInstantly.Data;
using FreeMusicInstantly.Data.Migrations;
using FreeMusicInstantly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;
using System;


namespace FreeMusicInstantly.Controllers
{
    public class PlaysController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public PlaysController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpPost]
        public async Task<IActionResult> TrackPlay(int songId)
        {
            var song = await db.Songs.Include(s => s.Plays)
                                      .FirstOrDefaultAsync(s => s.Id == songId);

            if (song == null)
            {
                return Json(new { success = false, message = "Song not found" });
            }

            string userId = _userManager.GetUserId(User);
            DateTime now = DateTime.UtcNow;

        
            var recentPlay = await db.Plays
                                     .Where(p => p.SongId == songId && p.UserId == userId)
                                     .OrderByDescending(p => p.PlayTime)
                                     .FirstOrDefaultAsync();

            if (recentPlay != null && (now - recentPlay.PlayTime).TotalSeconds < 10)
            {
                return Json(new { success = false, message = "Play already counted too recently", totalPlays = song.Plays.Count });
            }

            var newPlay = new Play
            {
                SongId = songId,
                UserId = userId,
                PlayTime = now
            };

            db.Plays.Add(newPlay);
            await db.SaveChangesAsync();

            int totalPlays = await db.Plays.CountAsync(p => p.SongId == songId);

            return Json(new { success = true, totalPlays = totalPlays });
        }




    }
}

