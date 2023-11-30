using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.Utilities.Extension;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PeskitAdmin")]
    public class DepartmentController : Controller
	{
		private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DepartmentController(AppDbContext context,IWebHostEnvironment env)
		{
			_context = context;
            _env = env;
        }
		public async Task<IActionResult> Index() 
        {
			List<Department> departmentList = await _context.Departments.Include(d=>d.Employees).ToListAsync();
            return View(departmentList);
        }

		public async Task<IActionResult> Create()
		{

			return View();
		}

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Departments.FirstOrDefaultAsync(d=>d.Id==id);
            if (existed is null) return NotFound();
            
            existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");

            _context.Departments.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
