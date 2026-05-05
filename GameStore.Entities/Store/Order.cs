using System;
using System.Collections.Generic;
using GameStore.Entities.Users;

namespace GameStore.Entities.Store
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual User? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }
}
