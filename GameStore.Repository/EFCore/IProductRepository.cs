using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Games;

namespace GameStore.Repository.EFCore;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetWithDetailsAsync(int id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<List<Product>> GetFeaturedAsync(int count);
    Task<List<Product>> GetByGenreAsync(int genreId);
    Task<(List<Product> Products, int TotalCount)> SearchAsync(string? keyword, int[]? genreIds, int[]? platformIds,
        decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize);
}
