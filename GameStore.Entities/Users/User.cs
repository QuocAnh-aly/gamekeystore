using System;
using System.Collections.Generic;
using GameStore.Entities.Store;

namespace GameStore.Entities.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; } = "user";
        public bool? IsVerified { get; set; } = false;
        public string? GoogleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<UserWishlist> UserWishlists { get; set; } = new HashSet<UserWishlist>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
