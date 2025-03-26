namespace Feedbacks.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public int RestorantCategoryId { get; set; }
        public int CityId { get; set; }
        public List<Review> Reviwes { get; set; } = new();
        public byte[] RestaurantImage { get; set; }
    }
}
    