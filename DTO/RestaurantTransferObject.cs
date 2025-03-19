namespace Feedbacks.DTO
{
    public class RestaurantTransferObject
    {
        public string Name { get; set; }
        public int CityId { get; set; }
        public int RestorantCategoryId { get; set; }
        public IFormFile RestaurantImage { get; set; }
    }
}
