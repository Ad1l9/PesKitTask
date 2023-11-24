namespace PesKitTask.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CommentCount { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
