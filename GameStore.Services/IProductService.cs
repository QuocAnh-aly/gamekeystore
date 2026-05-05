using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Games;

namespace GameStore.Services;

public interface IProductService
{
    Task<Product?> GetById(int id);
    Task<Product?> GetBySlug(string slug);
    Task<Product?> GetWithDetails(int id);
    Task<List<Product>> GetFeatured(int count = 10);
    Task<List<Product>> GetByGenre(int genreId);
    Task<(List<Product> Products, int TotalCount)> Search(string? keyword, int[]? genreIds, int[]? platformIds,
        decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize);
    Task<Product> Create(Product product);
    Task Update(Product product);
    Task Delete(int id);
}
