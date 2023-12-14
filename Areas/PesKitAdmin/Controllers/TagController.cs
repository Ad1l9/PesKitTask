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
            await _context.Tags.AddAsync(tag);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Tag? tag = await _context.Tags.FirstOrDefaultAsync(p => p.Id == id);

            if (tag is null) return NotFound();

            UpdateTagVM vm = new UpdateTagVM()
            {
                Name = tag.Name,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagvm)
        {
            Tag existed = await _context.Tags.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(tagvm);
            }

            bool result = _context.Tags.Any(c => c.Name == tagvm.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Bele tag artiq movcuddur");
            }


            existed.Name = tagvm.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            var tag = await _context.Tags.Include(c => c.BlogTags).ThenInclude(b=>b.Blog).FirstOrDefaultAsync(s => s.Id == id);
            if (tag == null) return NotFound();

            return View(tag);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
