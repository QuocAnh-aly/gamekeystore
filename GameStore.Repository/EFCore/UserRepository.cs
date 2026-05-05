using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameStore.Entities.Users;

namespace GameStore.Repository.EFCore;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(GameStoreDbContext context) : base(context) { }

    public async Task<User?> GetByNameAsync(string name) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Name == name);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<(List<User> Users, int TotalCount)> SearchAsync(string? keyword, int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(u => u.Name.ToLower().Contains(keyword)
                || (u.Email != null && u.Email.ToLower().Contains(keyword)));
        }
        var totalCount = await query.CountAsync();
        var users = await query.OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (users, totalCount);
    }

    public async Task<bool> IsNameExists(string name) =>
        await _dbSet.AnyAsync(u => u.Name == name);

    public async Task<bool> IsEmailExists(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);
}
