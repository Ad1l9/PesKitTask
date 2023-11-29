using Microsoft.AspNetCore.Mvc;
using PesKitTask.DAL;
using PesKitTask.Models;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PeskitAdmin")]
    public class DepartmentController : Controller
	{
		private readonly AppDbContext _context;

		public DepartmentController(AppDbContext context)
		{
			_context = context;
		}
		public IActionResult Index() 
        {
            return View();
        }
    }
}
