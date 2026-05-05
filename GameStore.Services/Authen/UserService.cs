using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Common.Auth;
using GameStore.Entities.Users;
using GameStore.Repository.EFCore;

namespace GameStore.Services.Authen;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Authenticate(string name, string password)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            return null;

        var user = await _userRepository.GetByNameAsync(name);
        if (user == null) return null;

        if (user.Password != password) return null;

        return user;
    }

    public async Task<User?> GetById(int id) => await _userRepository.GetByIdAsync(id);
    public async Task<List<User>> GetAll() => (await _userRepository.GetAllAsync()).ToList();

    public async Task<User> Register(User user, string password)
    {
        user.Password = password;
        user.CreatedAt = DateTime.Now;
        user.IsVerified = true;
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task Update(User user, string? password = null)
    {
        if (!string.IsNullOrEmpty(password))
        {
            user.Password = password;
        }
        await _userRepository.UpdateAsync(user);
    }

    public async Task Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
        }
    }

    public async Task<(List<User> Users, int TotalCount)> Search(string? keyword, int page, int pageSize) =>
        await _userRepository.SearchAsync(keyword, page, pageSize);

    public async Task<bool> IsUsernameExists(string name) =>
        await _userRepository.IsNameExists(name);

    public async Task<bool> IsEmailExists(string email) =>
        await _userRepository.IsEmailExists(email);

    public async Task<decimal> GetWalletBalance(int userId)
    {
        return 0; // Legacy
    }

    public async Task AddToWallet(int userId, decimal amount)
    {
        // Legacy
    }
}
