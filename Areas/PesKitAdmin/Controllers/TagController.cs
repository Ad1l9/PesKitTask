using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
	[Area("PesKitAdmin")]
	public class TagController : Controller
	{
		private readonly AppDbContext _context;

		public TagController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			List<Tag> tags = await _context.Tags.ToListAsync();
			return View(tags);
		}

		public async Task<IActionResult> Create()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> Create(CreateTagVM vm)
        {
            Tag tag = new() { Name = vm.Name.Trim() };

            if (!ModelState.IsValid) return View(vm);

            bool isInclude = await _context.Authors.AnyAsync(a => a.Name == vm.Name);
            if (isInclude)
            {
                ModelState.AddModelError("Name", "Bu Tag movcuddur");
                return View(vm);
            }
            _context.Tags.AddAsync(tag);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


    }
}
