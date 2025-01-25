using Microsoft.AspNetCore.Mvc;

namespace FreeMusicInstantly.Controllers
{
    public class PlaylistsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
