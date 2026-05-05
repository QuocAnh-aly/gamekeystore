using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameStore.Entities.Store;
using GameStore.Entities.Games;
using GameStore.Repository.EFCore;

namespace GameStore.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly GameStore.Repository.GameStoreDbContext _context;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository, GameStore.Repository.GameStoreDbContext context)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Order?> GetById(int id) => await _orderRepository.GetByIdAsync(id);
    public async Task<List<Order>> GetByUser(int userId) => await _orderRepository.GetByUserAsync(userId);
    public async Task<List<Order>> GetAll(int page, int pageSize) => (await _orderRepository.GetAllAsync()).ToList();

    public async Task<(List<Order> Items, int TotalCount)> SearchOrders(int page, int pageSize, string? keyword, DateTime? fromDate, DateTime? toDate, string? status)
    {
        var query = _context.Orders.Include(o => o.User).AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            var isNumeric = int.TryParse(keyword, out int orderId);
            query = query.Where(o => (o.User != null && o.User.Name.Contains(keyword)) || (isNumeric && o.Id == orderId));
        }
        if (fromDate.HasValue) query = query.Where(o => o.CreatedAt >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(o => o.CreatedAt <= toDate.Value);
        if (!string.IsNullOrEmpty(status)) query = query.Where(o => o.Status == status);

        int totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(o => o.CreatedAt)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        return (items, totalCount);
    }

    public async Task<Order> CreateOrder(int userId, List<(int ProductId, int PlatformId, int Quantity)> items)
    {
        decimal totalPrice = 0;
        var orderItems = new List<OrderItem>();
        var user = await _userRepository.GetByIdAsync(userId);

        foreach (var (productId, platformId, quantity) in items)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new Exception($"Product {productId} not found");

            var price = product.SalePrice ?? product.Price ?? 0;
            totalPrice += price * quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = productId,
                PlatformId = platformId,
                PriceAtPurchase = price
            });
        }

        if (user == null) throw new Exception("User not found");

        var order = new Order
        {
            UserId = userId,
            TotalPrice = totalPrice,
            Status = "Completed",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            OrderItems = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order> UpdateStatus(int orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Order not found");
        order.Status = status;
        order.UpdatedAt = DateTime.Now;
        await _orderRepository.UpdateAsync(order);
        return order;
    }

    public async Task CancelOrder(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Order not found");
        if (order.Status == "Completed") throw new Exception("Cannot cancel completed order");

        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.Now;
        await _orderRepository.UpdateAsync(order);
    }
}
