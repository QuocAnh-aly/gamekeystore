using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Repository;
using GameStore.Entities.Games;
using GameStore.Entities.Store;
using GameStore.Entities.Users;

namespace GameStore.APIService.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminManagementController : ControllerBase
{
    private readonly GameStoreDbContext _context;

    public AdminManagementController(GameStoreDbContext context)
    {
        _context = context;
    }

    // ======================= CATEGORIES (Genres) =======================

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null)
    {
        var query = _context.Genres.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(g => g.Name.Contains(keyword));

        var totalCount = await query.CountAsync();
        var data = await query
            .OrderBy(g => g.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new
            {
                g.Id, g.Name, g.Slug,
                productCount = _context.ProductGenres.Count(pg => pg.GenreId == g.Id)
            })
            .ToListAsync();

        return Ok(new { data, totalCount });
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDto dto)
    {
        if (await _context.Genres.AnyAsync(g => g.Name == dto.Name))
            return BadRequest(new { message = "Category name already exists" });

        var genre = new Genre
        {
            Name = dto.Name,
            Slug = dto.Name.ToLower().Replace(" ", "-")
        };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return Ok(genre);
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) return NotFound(new { message = "Category not found" });

        var productsUsingGenre = await _context.ProductGenres.CountAsync(pg => pg.GenreId == id);
        if (productsUsingGenre > 0)
            return BadRequest(new { message = $"Cannot delete: {productsUsingGenre} products use this category" });

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category deleted" });
    }

    // ======================= INVENTORY (Game Keys) =======================

    [HttpGet("gamekeys")]
    public async Task<IActionResult> GetGameKeys(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null, [FromQuery] int? productId = null)
    {
        var query = _context.Inventories.Include(i => i.Product).Include(i => i.Platform).AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(i => i.GameKey.Contains(keyword) || i.Product.Name.Contains(keyword));

        if (productId.HasValue)
            query = query.Where(i => i.ProductId == productId.Value);

        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new
            {
                i.Id, i.ProductId, productName = i.Product.Name,
                platformName = i.Platform.Name,
                i.GameKey, i.IsSold, i.CreatedAt
            })
            .ToListAsync();

        return Ok(new { data, totalCount });
    }

    [HttpPost("gamekeys")]
    public async Task<IActionResult> CreateGameKey([FromBody] GameKeyDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) return NotFound(new { message = "Product not found" });

        var key = new Inventory
        {
            ProductId = dto.ProductId,
            PlatformId = dto.PlatformId,
            GameKey = dto.GameKey,
            IsSold = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.Inventories.Add(key);
        await _context.SaveChangesAsync();
        return Ok(key);
    }

    // ======================= USERS =======================

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(u => u.Name.Contains(keyword) || u.Email.Contains(keyword));

        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.Id, u.Name, u.Email, u.Role, u.CreatedAt
            })
            .ToListAsync();

        return Ok(new { data, totalCount });
    }
}

public class CategoryDto
{
    public string Name { get; set; } = "";
}

public class GameKeyDto
{
    public int ProductId { get; set; }
    public int PlatformId { get; set; }
    public string GameKey { get; set; } = "";
}
