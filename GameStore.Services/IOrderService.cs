using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Store;

namespace GameStore.Services;

public interface IOrderService
{
    Task<Order?> GetById(int id);
    Task<List<Order>> GetByUser(int userId);
    Task<List<Order>> GetAll(int page, int pageSize);
    Task<(List<Order> Items, int TotalCount)> SearchOrders(int page, int pageSize, string? keyword, DateTime? fromDate, DateTime? toDate, string? status);
    Task<Order> CreateOrder(int userId, List<(int ProductId, int PlatformId, int Quantity)> items);
    Task<Order> UpdateStatus(int orderId, string status);
    Task CancelOrder(int orderId);
}
