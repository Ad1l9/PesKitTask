using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PesKitTask.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Max length is 25")]
        public string Title { get; set; }
        public int CommentCount { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
		public List<BlogTag>? BlogTags { get; set; }


    }
}
