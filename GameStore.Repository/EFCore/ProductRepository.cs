using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameStore.Entities.Games;

namespace GameStore.Repository.EFCore;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(GameStoreDbContext context) : base(context) { }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.ProductGenres).ThenInclude(pg => pg.Genre)
            .Include(p => p.ProductPlatforms).ThenInclude(pp => pp.Platform)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<Product?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.ProductGenres).ThenInclude(pg => pg.Genre)
            .Include(p => p.ProductPlatforms).ThenInclude(pp => pp.Platform)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetFeaturedAsync(int count)
    {
        return await _dbSet
            .Where(p => p.IsFeatured)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Product>> GetByGenreAsync(int genreId)
    {
        return await _dbSet
            .Where(p => p.ProductGenres.Any(pg => pg.GenreId == genreId))
            .ToListAsync();
    }

    public async Task<(List<Product> Products, int TotalCount)> SearchAsync(string? keyword, int[]? genreIds, int[]? platformIds,
        decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword));
        }

        if (genreIds != null && genreIds.Length > 0)
        {
            query = query.Where(p => p.ProductGenres.Any(pg => genreIds.Contains(pg.GenreId)));
        }

        if (platformIds != null && platformIds.Length > 0)
        {
            query = query.Where(p => p.ProductPlatforms.Any(pp => platformIds.Contains(pp.PlatformId)));
        }

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice);

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "newest" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (products, totalCount);
    }
}
