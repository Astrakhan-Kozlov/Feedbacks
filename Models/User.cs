﻿namespace Feedbacks.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int CityId { get; set; }
        public int RoleId { get; set; }
    }
}
