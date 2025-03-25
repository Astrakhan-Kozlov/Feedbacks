namespace Feedbacks.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public required string Title { get; set; }        
        public required string Text { get; set; }
    }
}
