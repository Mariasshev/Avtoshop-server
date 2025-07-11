﻿namespace Data_Access.Models
{
    public class User
    {
        public User()
        {
            this.Orders = new HashSet<Order>();
        }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool isAdmin { get; set; }

        // Новые поля
        public string? Surname { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        public string? PhotoUrl { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
