using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.Utilities.Extension;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitAdmin")]
    public class ProjectController : Controller
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
            List<Project> projects = await _context.Projects.Include(p => p.ProjectImages).ToListAsync();
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
            if (projectVM.Photos is not null)
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

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Project project = await _context.Projects
                .Include(p => p.ProjectImages)
                .FirstOrDefaultAsync(p => p.Id == id);


            if (project is null) return NotFound();

            UpdateProjectVM projectVM = new()
            {

                Name = project.Name,
                ProjectImages = project.ProjectImages
            };
            return View(projectVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateProjectVM projectVM)
        {
            Project existed = await _context.Projects
                .Include(p => p.ProjectImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            projectVM.ProjectImages = existed.ProjectImages;

            if (!ModelState.IsValid)
            {
                return View(projectVM);
            }

            if (existed is null) return NotFound();




            if (projectVM.MainPhoto is not null)
            {
                if (!projectVM.MainPhoto.ValidateType())
                {
                    ModelState.AddModelError("MainPhoto", "File type is not valid");
                    return View(projectVM);
                }
                if (!projectVM.MainPhoto.ValidateSize(600))
                {
                    ModelState.AddModelError("MainPhoto", "File size is not valid");
                    return View(projectVM);
                }
            }


            if (projectVM.MainPhoto is not null)
            {
                string fileName = await projectVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img");
                ProjectImage mainImage = existed.ProjectImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
                _context.ProjectImages.Remove(mainImage);
                existed.ProjectImages.Add(new ProjectImage
                {
                    Alternative = projectVM.Name,
                    IsPrimary = true,
                    ImageURL = fileName
                });
            }
            if (projectVM.ImageIds is null)
            {
                projectVM.ImageIds = new();
            }
            var removeable = existed.ProjectImages.Where(pi => !projectVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProjectImage pi in removeable)
            {
                pi.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ProjectImages.Remove(pi);
            }




            bool result = _context.Projects.Any(c => c.Name == projectVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele product movcuddur");
                return View(projectVM);
            }



            TempData["Message"] = "";
            if (projectVM.Photos is not null)
            {
                foreach (IFormFile photo in projectVM.Photos)
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
                        continue;
                    }

                    existed.ProjectImages.Add(new ProjectImage
                    {
                        Alternative = projectVM.Name,
                        IsPrimary = null,
                        ImageURL = await photo.CreateFile(_env.WebRootPath, "assets", "img")
                    });
                }
            }


            existed.Name = projectVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0) return BadRequest();
			var existed = await _context.Projects.Include(p => p.ProjectImages).FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) return NotFound();
			foreach (ProjectImage image in existed.ProjectImages)
			{
				image.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
			}
			_context.Projects.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	}
}
