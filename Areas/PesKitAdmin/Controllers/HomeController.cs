using Microsoft.AspNetCore.Mvc;

namespace PesKitTask.Areas.Areas.Controllers
{
    [Area("PesKitAdmin")]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
