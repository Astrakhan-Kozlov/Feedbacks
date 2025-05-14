namespace Feedbacks.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        public int RestorantCategoryId { get; set; }
        public City City { get; set; }
        public int CityId { get; set; }
        public bool Activated { get; set; }
        public List<Review> Reviwes { get; set; } = new();
        public List<string> RestaurantImage { get; set; } = new List<string>();
    }
}
