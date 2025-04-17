using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Feedbacks.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public City City { get; set; }
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }
        public int? RestaurantId { get; set; }
        public int CityId { get; set; }
        public int RoleId { get; set; }
        public List<Review> Reviews { get; set; } = new();
    }
}
