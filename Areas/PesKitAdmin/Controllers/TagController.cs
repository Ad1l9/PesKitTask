using Microsoft.AspNetCore.Authorization;
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


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
		{
			List<Tag> tags = await _context.Tags.ToListAsync();
			return View(tags);
		}



        [Authorize(Roles = "Admin,Moderator")]
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


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Position position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);

            if (position is null) return NotFound();

            UpdatePositionVM vm = new UpdatePositionVM()
            {
                Name = position.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdatePositionVM positionVM)
        {
            Position existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(positionVM);
            }

            bool result = _context.Projects.Any(c => c.Name == positionVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele position artiq movcuddur");
            }


            existed.Name = positionVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
