using System;
using System.Collections.Generic;

namespace EcommerceApp.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } 

        public ICollection<Order> Orders { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public Cart Cart { get; set; }
    }
}