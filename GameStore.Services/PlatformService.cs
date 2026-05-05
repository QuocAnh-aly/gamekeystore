using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Games;
using GameStore.Repository.EFCore;

namespace GameStore.Services;

public class PlatformService : IPlatformService
{
    private readonly IRepository<Platform> _repository;
    public PlatformService(IRepository<Platform> repository) => _repository = repository;

    public async Task<List<Platform>> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return items.ToList();
    }

    public async Task<Platform?> GetById(int id) => await _repository.GetByIdAsync(id);

    public async Task<Platform> Create(Platform platform) => await _repository.AddAsync(platform);

    public async Task Update(Platform platform) => await _repository.UpdateAsync(platform);

    public async Task Delete(int id) => await _repository.DeleteByIdAsync(id);
}
