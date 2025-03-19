namespace Feedbacks.DTO
{
    public class RestaurantTransferObject
    {
        public string Name { get; set; }
        public int CityId { get; set; }
        public IFormFile RestaurantImage { get; set; }
    }
}
