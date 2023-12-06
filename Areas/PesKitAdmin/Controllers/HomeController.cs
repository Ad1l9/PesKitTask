using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PesKitTask.Areas.Areas.Controllers
{
    [Area("PesKitAdmin")]
    [Authorize(Roles ="Admin,Moderator")]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
