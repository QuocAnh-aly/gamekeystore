using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Games;
using GameStore.Repository.EFCore;

namespace GameStore.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetById(int id) => await _productRepository.GetByIdAsync(id);
    public async Task<Product?> GetBySlug(string slug) => await _productRepository.GetBySlugAsync(slug);
    public async Task<Product?> GetWithDetails(int id) => await _productRepository.GetWithDetailsAsync(id);
    public async Task<List<Product>> GetFeatured(int count = 10) => await _productRepository.GetFeaturedAsync(count);
    public async Task<List<Product>> GetByGenre(int genreId) => await _productRepository.GetByGenreAsync(genreId);

    public async Task<(List<Product> Products, int TotalCount)> Search(string? keyword, int[]? genreIds, int[]? platformIds,
        decimal? minPrice, decimal? maxPrice, string? sortBy, bool descending, int page, int pageSize) =>
        await _productRepository.SearchAsync(keyword, genreIds, platformIds, minPrice, maxPrice, sortBy, descending, page, pageSize);

    public async Task<Product> Create(Product product)
    {
        product.CreatedAt = DateTime.Now;
        await _productRepository.AddAsync(product);
        return product;
    }

    public async Task Update(Product product)
    {
        product.UpdatedAt = DateTime.Now;
        await _productRepository.UpdateAsync(product);
    }

    public async Task Delete(int id)
    {
        await _productRepository.DeleteByIdAsync(id);
    }
}
