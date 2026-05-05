using System;
using GameStore.Entities.Games;

namespace GameStore.Entities.Store
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PlatformId { get; set; }
        public string? GameKey { get; set; }
        public bool? IsSold { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; } = null!;
        public virtual Platform Platform { get; set; } = null!;
    }
}
