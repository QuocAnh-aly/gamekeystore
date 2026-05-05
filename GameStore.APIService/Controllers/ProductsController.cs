using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameStore.Entities.Games;
using GameStore.Services;

namespace GameStore.APIService.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] int[]? genres,
        [FromQuery] int[]? platforms,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] bool desc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] bool? isFeatured = null,
        [FromQuery] bool? isNew = null,
        [FromQuery] bool? isHot = null)
    {
        var (products, totalCount) = await _productService.Search(search, genres, platforms, minPrice, maxPrice, sortBy, desc, page, pageSize);
        
        // Filter by flags if provided (simple filtering in memory for now if not in Search)
        if (isFeatured.HasValue) products = products.Where(p => p.IsFeatured == isFeatured.Value).ToList();
        if (isNew.HasValue) products = products.Where(p => p.IsNew == isNew.Value).ToList();
        if (isHot.HasValue) products = products.Where(p => p.IsHot == isHot.Value).ToList();

        return Ok(new 
        { 
            status = "success",
            data = new { products },
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var product = await _productService.GetBySlug(slug);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(new { status = "success", data = new { product } });
    }

    [HttpGet("{id}/related")]
    public async Task<IActionResult> GetRelated(int id)
    {
        // Simple logic for related products: same genre
        var product = await _productService.GetWithDetails(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        
        var genreIds = product.ProductGenres.Select(pg => pg.GenreId).ToArray();
        var (related, _) = await _productService.Search(null, genreIds, null, null, null, "newest", true, 1, 5);
        
        // Exclude the current product
        related = related.Where(p => p.Id != id).ToList();
        
        return Ok(new { status = "success", data = new { products = related } });
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetWithDetails(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(new { status = "success", data = new { product } });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var created = await _productService.Create(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { status = "success", data = new { product = created } });
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Product productUpdate)
    {
        var product = await _productService.GetById(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        
        // Update fields... (simplified for now)
        product.Name = productUpdate.Name ?? product.Name;
        product.Description = productUpdate.Description ?? product.Description;
        product.Price = productUpdate.Price ?? product.Price;
        product.SalePrice = productUpdate.SalePrice ?? product.SalePrice;
        product.CoverImage = productUpdate.CoverImage ?? product.CoverImage;
        product.IsNew = productUpdate.IsNew;
        product.IsHot = productUpdate.IsHot;
        product.IsFeatured = productUpdate.IsFeatured;
        product.Slug = productUpdate.Slug ?? product.Slug;
        
        await _productService.Update(product);
        return Ok(new { status = "success", data = new { product } });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.Delete(id);
        return Ok(new { status = "success", message = "Product deleted" });
    }
}
