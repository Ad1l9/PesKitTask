using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.Utilities.Extension;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitAdmin")]
    public class ProjectController:Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProjectController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Project> projects = await _context.Projects.Include(p=>p.ProjectImages).ToListAsync();
            return View(projects);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectVM projectVM)
        {
            if (!ModelState.IsValid) return View();

            if (!projectVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "File tipi uygun deyil");
                return View();
            }
            if (!projectVM.MainPhoto.ValidateSize(600))
            {
                ModelState.AddModelError("MainPhoto", "File olcusu uygun deyil");
                return View();
            }
            ProjectImage mainImage = new()
            {
                Alternative = projectVM.Name,
                IsPrimary = true,
                ImageURL = await projectVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img"),
            };

            Project project = new Project()
            {
                Name = projectVM.Name,
                ProjectImages = new List<ProjectImage> { mainImage },
            };


            TempData["Message"] = "";
            if(projectVM.Photos is not null)
            {
                foreach (IFormFile image in projectVM.Photos)
                {
                    if (!image.ValidateType("image/"))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{image.FileName} file tipi uygun deyil</p>";
                        continue;
                    }
                    if (!image.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{image.FileName} file olcusu uygun deyil</p>";
                        continue;
                    }
                    project.ProjectImages.Add(new ProjectImage
                    {
                        ImageURL = await image.CreateFile(_env.WebRootPath, "assets", "img"),
                        IsPrimary = null,
                        Alternative = projectVM.Name
                    });
                }
            }
            

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

    }
}
