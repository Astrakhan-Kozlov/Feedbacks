namespace Feedbacks.DTO
{
    public class RestaurantDTO
    {
        public string Name { get; set; }
        public int CityId { get; set; }
        public int RestorantCategoryId { get; set; }
        public IFormFile RestaurantImage { get; set; }
    }
}
