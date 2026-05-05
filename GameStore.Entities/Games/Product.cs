using System;
using System.Collections.Generic;
using GameStore.Entities.Store;
using GameStore.Entities.Users;

namespace GameStore.Entities.Games
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? CoverImage { get; set; }
        public bool IsNew { get; set; } = false;
        public bool IsHot { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<ProductGenre> ProductGenres { get; set; } = new HashSet<ProductGenre>();
        public virtual ICollection<ProductPlatform> ProductPlatforms { get; set; } = new HashSet<ProductPlatform>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();
        public virtual ICollection<UserWishlist> UserWishlists { get; set; } = new HashSet<UserWishlist>();
    }
}
