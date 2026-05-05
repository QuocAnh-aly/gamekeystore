using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Repository;
using System.Security.Claims;

namespace GameStore.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly GameStoreDbContext _context;

    public LibraryController(GameStoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyLibrary()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var userId = int.Parse(userIdStr);

        var library = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == "Completed")
            .SelectMany(o => o.OrderItems)
            .Select(oi => new
            {
                oi.Product.Id,
                oi.Product.Name,
                oi.Product.CoverImage,
                AcquiredAt = oi.Order.CreatedAt
            })
            .Distinct()
            .ToListAsync();

        return Ok(library);
    }

    [HttpGet("check/{productId}")]
    public async Task<IActionResult> CheckOwned(int productId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var userId = int.Parse(userIdStr);

        var owned = await _context.OrderItems
            .AnyAsync(oi => oi.Order.UserId == userId
                        && oi.Order.Status == "Completed"
                        && oi.ProductId == productId);

        return Ok(new { owned });
    }
}
