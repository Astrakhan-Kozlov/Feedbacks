namespace Feedbacks.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public int CityId { get; set; }
        public byte[] RestaurantImage { get; set; }
    }
}
    