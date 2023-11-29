using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PesKitTask.Models;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class CreateBlogVM
    {

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Max length is 50")]

        public string Title { get; set; }

        public int CommentCount { get; set; }

        public string Description { get; set; }

        public int AuthorId { get; set; }

        public List<Author>? Authors { get; set; }

        public List<int> TagIds { get; set; }

        public List<Tag>? Tags { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
