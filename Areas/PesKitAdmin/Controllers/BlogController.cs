using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Areas.PesKitAdmin.ViewModel;
using PesKitTask.DAL;
using PesKitTask.Models;
using PesKitTask.Utilities.Extension;

namespace PesKitTask.Areas.PesKitAdmin.Controllers
{
    [Area("PesKitAdmin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _context.Blogs
                .Include(b=>b.BlogTags).ThenInclude(bt=>bt.Tag)
                .Include(b => b.Author).ToListAsync();
            return View(blogs);
        }

        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
        {
			var vm = new CreateBlogVM
			{
				Tags = await _context.Tags.ToListAsync(),
                Authors=await _context.Authors.ToListAsync()
			};
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBlogVM blogVM)
        {
            if (!ModelState.IsValid)
            {
                blogVM.Tags = await _context.Tags.ToListAsync();
                return View(blogVM);
            }

            foreach (int id in blogVM.TagIds)
            {
                bool tagresult = await _context.Tags.AnyAsync(t => t.Id == id);
                if (!tagresult)
                {
                    blogVM.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "Bele tag movcud deyil");
                    return View(blogVM);
                }
            }

            if (!blogVM.Photo.ValidateType())
            {
                blogVM.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Photo", "Yanlis fayl tipi");
                return View();
            }
            if (!blogVM.Photo.ValidateSize(2 * 1024))
            {
                blogVM.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Photo", "2mb dan cox olmamalidir");
                return View();
            }

            string fileName = await blogVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");

            Blog blog = new()
            {
                AuthorId = blogVM.AuthorId,
                ImageUrl = fileName,
                CommentCount = blogVM.CommentCount,
                Title = blogVM.Title,
                Description = blogVM.Description,
                BlogTags=new()
            };
            foreach (int id in blogVM.TagIds)
            {
                var bTag = new BlogTag
                {
                    TagId = id,
                    Blog = blog
                };
                blog.BlogTags.Add(bTag);
            }

            await _context.AddAsync(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Blog blog = await _context.Blogs
                .Include(b => b.BlogTags)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (blog is null) return NotFound();

            UpdateBlogVM blogVM = new()
            {
                Title = blog.Title,
                Description = blog.Description,
                CommentCount = blog.CommentCount,
                AuthorId = blog.AuthorId,
                ImageUrl=blog.ImageUrl,
                TagIds = blog.BlogTags.Select(pt => pt.TagId).ToList(),
                Tags = await _context.Tags.ToListAsync(),
                Authors=await _context.Authors.ToListAsync(),   
            };
            return View(blogVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateBlogVM vm)
        {
            Blog existed = await _context.Blogs
               .Include(b => b.BlogTags)
               .Include(b => b.Author)
               .FirstOrDefaultAsync(b => b.Id == id);

            if (!ModelState.IsValid)
            {
                vm.Authors = await _context.Authors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                return View(vm);
            }
            if (vm.Photo is not null)
            {
                if (!vm.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "Wrong file type");
                    return View();
                }
                if (!vm.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "It shouldn't exceed 2 mb");
                    return View();
                }

                existed.ImageUrl = await vm.Photo.CreateFile(_env.WebRootPath, "assets", "img");
            }



            if (existed == null) return NotFound();
            //bool result = await _context.Blogs.AnyAsync(c => c.AuthorId == vm.AuthorId);
            //if (!result)
            //{
            //    vm.Authors = await _context.Authors.ToListAsync();
            //    vm.Tags = await _context.Tags.ToListAsync();
            //    return View(vm);
            //}

            foreach (int idT in vm.TagIds)
            {
                bool TagResult = await _context.Tags.AnyAsync(t => t.Id == idT);
                if (!TagResult)
                {
                    vm.Authors = await _context.Authors.ToListAsync();
                    vm.Tags = await _context.Tags.ToListAsync();
                    ModelState.AddModelError("TagIds", "There is no such tag");
                    return View(vm);
                }
            }

            bool result = _context.Blogs.Any(c => c.Title == vm.Title && c.Id != id);
            if (result)
            {
                vm.Authors = await _context.Authors.ToListAsync();
                vm.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Name", "There is already such blog");
                return View(vm);
            }

            existed.BlogTags.RemoveAll(pTag => !vm.TagIds.Contains(pTag.Id));

            existed.BlogTags.AddRange(vm.TagIds.Where(tagId => !existed.BlogTags.Any(pt => pt.Id == tagId))
                                 .Select(tagId => new BlogTag { TagId = tagId }));

            existed.Title = vm.Title;
            existed.Description = vm.Description;

            existed.AuthorId = vm.AuthorId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var deleted = await _context.Blogs.FirstOrDefaultAsync(c => c.Id == id);
            if (deleted is null) return NotFound();
            _context.Blogs.Remove(deleted);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Detail(int id)
        {
			if (id <= 0) return BadRequest();
			Blog blog = await _context.Blogs
                .Include(blog => blog.BlogTags).ThenInclude(bt=>bt.Tag)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (blog is null) return NotFound();
			return View(blog);
		}
    }
}
