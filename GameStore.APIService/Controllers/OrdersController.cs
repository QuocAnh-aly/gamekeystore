using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameStore.Services;
using System.Security.Claims;

namespace GameStore.APIService.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) => _orderService = orderService;

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var orders = await _orderService.GetByUser(userId);
        return Ok(new { status = "success", data = new { orders } });
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var orders = await _orderService.GetAll(page, pageSize);
        return Ok(new { status = "success", data = new { orders } });
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SearchOrders(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null, [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null, [FromQuery] string? status = null)
    {
        var result = await _orderService.SearchOrders(page, pageSize, keyword, fromDate, toDate, status);
        return Ok(new 
        { 
            status = "success", 
            data = new { orders = result.Items.Select(o => new { o.Id, o.UserId, o.CreatedAt, o.TotalPrice, o.Status, o.PaymentMethod, Name = o.User?.Name }) }, 
            totalCount = result.TotalCount 
        });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        try 
        { 
            await _orderService.UpdateStatus(id, dto.Status); 
            return Ok(new { status = "success", message = "Status updated" }); 
        }
        catch (Exception ex) { return BadRequest(new { status = "error", message = ex.Message }); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetById(id);
        return order == null ? NotFound(new { status = "error", message = "Order not found" }) : Ok(new { status = "success", data = new { order } });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var userId = int.Parse(userIdStr);
        
        try
        {
            var order = await _orderService.CreateOrder(userId, dto.Items.Select(i => (i.ProductId, i.PlatformId, i.Quantity)).ToList());
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, new { status = "success", data = new { order } });
        }
        catch (Exception ex) { return BadRequest(new { status = "error", message = ex.Message }); }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try 
        { 
            await _orderService.CancelOrder(id); 
            return Ok(new { status = "success", message = "Order cancelled" }); 
        }
        catch (Exception ex) { return BadRequest(new { status = "error", message = ex.Message }); }
    }
}

public class CreateOrderDto { public List<OrderItemDto> Items { get; set; } = new(); }
public class OrderItemDto { public int ProductId { get; set; } public int PlatformId { get; set; } public int Quantity { get; set; } = 1; }
public class UpdateStatusDto { public string Status { get; set; } = ""; }
