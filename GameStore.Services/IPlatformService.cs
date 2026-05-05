using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Entities.Games;

namespace GameStore.Services;

public interface IPlatformService
{
    Task<List<Platform>> GetAll();
    Task<Platform?> GetById(int id);
    Task<Platform> Create(Platform platform);
    Task Update(Platform platform);
    Task Delete(int id);
}
