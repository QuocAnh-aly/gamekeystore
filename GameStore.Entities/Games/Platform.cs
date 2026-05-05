using System;
using System.Collections.Generic;

namespace GameStore.Entities.Games
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public virtual ICollection<ProductPlatform> ProductPlatforms { get; set; } = new HashSet<ProductPlatform>();
    }
}
