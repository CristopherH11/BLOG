using System.ComponentModel.DataAnnotations;

namespace BLOG.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PostDate { get; set; }
        public string AuthorId { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public AppUser User { get; set; }
    }
}
