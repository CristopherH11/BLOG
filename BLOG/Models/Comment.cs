namespace BLOG.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string AuthorId { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public AppUser User { get; set; }
    }
}
