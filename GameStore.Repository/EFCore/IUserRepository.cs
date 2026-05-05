using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Users;

namespace GameStore.Repository.EFCore;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByNameAsync(string name);
    Task<User?> GetByEmailAsync(string email);
    Task<(List<User> Users, int TotalCount)> SearchAsync(string? keyword, int page, int pageSize);
    Task<bool> IsNameExists(string name);
    Task<bool> IsEmailExists(string email);
}
