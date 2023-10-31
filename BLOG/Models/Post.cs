using System.ComponentModel.DataAnnotations;

namespace BLOG.Models
{
    public class Post
    {
        public Post() { Comments = new List<Comment>(); }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PostDate { get; set; }

        /*public string Author { get; set; }*/
        public ICollection<Comment> Comments { get; set; }
    }
}
