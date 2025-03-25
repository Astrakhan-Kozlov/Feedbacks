namespace Feedbacks.Models
{
    public class Review
    {
        public int Id { get; set; }
        public required User User { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }        
        public required string Text { get; set; }

    }


}
