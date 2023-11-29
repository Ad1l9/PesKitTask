using PesKitTask.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Areas.PesKitAdmin.ViewModel
{
    public class UpdateBlogVM
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Max length is 25")]
        public string Title { get; set; }
        public int CommentCount { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public List<int> TagIds { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Author>? Authors { get; set; }
    }
}
