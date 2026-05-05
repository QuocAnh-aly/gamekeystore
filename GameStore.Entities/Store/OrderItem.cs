using System;
using GameStore.Entities.Games;

namespace GameStore.Entities.Store
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? PlatformId { get; set; }
        public decimal? PriceAtPurchase { get; set; }
        public int? PurchasedKeyId { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Platform? Platform { get; set; }
        public virtual Inventory? PurchasedKey { get; set; }
    }
}
