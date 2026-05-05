using System;
using Microsoft.EntityFrameworkCore;
using GameStore.Entities.Users;
using GameStore.Entities.Games;
using GameStore.Entities.Store;

namespace GameStore.Repository
{
    public class GameStoreDbContext : DbContext
    {
        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : base(options) { }

        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Platform> Platforms => Set<Platform>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductGenre> ProductGenres => Set<ProductGenre>();
        public DbSet<ProductPlatform> ProductPlatforms => Set<ProductPlatform>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserWishlist> UserWishlists => Set<UserWishlist>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Genres
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            });

            // Platforms
            modelBuilder.Entity<Platform>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            });

            // Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsNew).HasDefaultValue(false);
                entity.Property(e => e.IsHot).HasDefaultValue(false);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            // ProductGenres
            modelBuilder.Entity<ProductGenre>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.GenreId });
                entity.HasOne(e => e.Product).WithMany(p => p.ProductGenres).HasForeignKey(e => e.ProductId);
                entity.HasOne(e => e.Genre).WithMany(g => g.ProductGenres).HasForeignKey(e => e.GenreId);
            });

            // ProductPlatforms
            modelBuilder.Entity<ProductPlatform>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.PlatformId });
                entity.HasOne(e => e.Product).WithMany(p => p.ProductPlatforms).HasForeignKey(e => e.ProductId);
                entity.HasOne(e => e.Platform).WithMany(p => p.ProductPlatforms).HasForeignKey(e => e.PlatformId);
            });

            // ProductImages
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Product).WithMany(p => p.ProductImages).HasForeignKey(e => e.ProductId);
            });

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.Role).HasDefaultValue("user").HasMaxLength(50);
                entity.Property(e => e.IsVerified).HasDefaultValue(false);
                entity.Property(e => e.GoogleId).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
            });

            // UserWishlists
            modelBuilder.Entity<UserWishlist>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ProductId });
                entity.HasOne(e => e.User).WithMany(u => u.UserWishlists).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Product).WithMany(p => p.UserWishlists).HasForeignKey(e => e.ProductId);
            });

            // Inventories
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.GameKey).IsUnique();
                entity.Property(e => e.GameKey).HasMaxLength(255);
                entity.Property(e => e.IsSold).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
                entity.HasOne(e => e.Platform).WithMany().HasForeignKey(e => e.PlatformId);
            });

            // Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaymentMethod).HasMaxLength(100);
                entity.Property(e => e.Status).HasDefaultValue("pending").HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User).WithMany(u => u.Orders).HasForeignKey(e => e.UserId);
            });

            // OrderItems
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PriceAtPurchase).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Order).WithMany(o => o.OrderItems).HasForeignKey(e => e.OrderId);
                entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
                entity.HasOne(e => e.Platform).WithMany().HasForeignKey(e => e.PlatformId);
                entity.HasOne(e => e.PurchasedKey).WithMany().HasForeignKey(e => e.PurchasedKeyId);
            });
        }
    }
}
