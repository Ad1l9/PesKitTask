using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
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


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index() 
        {
			List<Department> departmentList = await _context.Departments.Include(d=>d.Employees).ToListAsync();
            return View(departmentList);
        }



        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
		public async Task<IActionResult> Create(CreateDepartmentVM depVM)
		{
            if (!ModelState.IsValid) return View(depVM);

            if (!depVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil");
                return View();
            }
            if (!depVM.Photo.ValidateSize(600))
            {
                ModelState.AddModelError("Photo", "File olcusu uygun deyil");
                return View();
            }

            Department dep = new Department()
            {
                Name = depVM.Name,
                ImageUrl= await depVM.Photo.CreateFile(_env.WebRootPath, "assets", "img"),
            };



            await _context.Departments.AddAsync(dep);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Department department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);


            if (department is null) return NotFound();

            UpdateDepartmentVM depVM = new()
            {
                Name = department.Name,
                ImageUrl= department.ImageUrl,
            };
            return View(depVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateDepartmentVM depVM,int id)
        {

            Department existed = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);

            if (!ModelState.IsValid) return View();
            
            
            if (depVM.Photo is not null)
            {
                if (!depVM.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "File type is not valid");
                    return View(depVM);
                }
                if (!depVM.Photo.ValidateSize(600))
                {
                    ModelState.AddModelError("Photo", "File size is not valid");
                    return View(depVM);
                }
            }


            if (depVM.Photo is not null)
            {
                string fileName = await depVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
                existed.ImageUrl = fileName;
            }




            bool result = _context.Departments.Any(c => c.Name == depVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele Department artiq movcuddur");
                return View(depVM);
            }




            existed.Name = depVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
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



        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);
            if (existed is null) return NotFound();
            return View(existed);

        }
    }
}
