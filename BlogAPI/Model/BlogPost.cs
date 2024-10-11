using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Model
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }

        public string Content { get; set; }

        [MaxLength(100)]
        public string Quote { get; set; }
    }
}
