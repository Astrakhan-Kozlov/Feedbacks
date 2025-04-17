namespace Feedbacks.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public int AuthorId { get; set; }
        public Review Review { get; set; }
        public int ReviewId { get; set; }
        public string Text { get; set; }
    }
}
