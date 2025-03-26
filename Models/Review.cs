namespace Feedbacks.Models
{
    public class Review
    {
        public int Id { get; set; }
        public required User User { get; set; }
        public int UserId { get; set; }
        public required Restaurant Restaurant { get; set; }
        public int Rating { get; set; }
        public int Status { get; set; }
        public required string Title { get; set; }        
        public required string Text { get; set; }

    }


}
