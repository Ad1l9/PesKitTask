namespace PesKitTask.Models
{
    public class ProjectImage
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string Alternative { get; set; }
        public bool? IsPrimary { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
