using System;

namespace GameStore.Entities.Games
{
    public class ProductPlatform
    {
        public int ProductId { get; set; }
        public int PlatformId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Platform Platform { get; set; } = null!;
    }
}
