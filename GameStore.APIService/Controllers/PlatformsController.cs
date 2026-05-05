using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameStore.Entities.Games;
using GameStore.Services;

namespace GameStore.APIService.Controllers;

[Route("api/platforms")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformService _platformService;
    public PlatformsController(IPlatformService platformService) => _platformService = platformService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var platforms = await _platformService.GetAll();
        return Ok(new { status = "success", data = new { platforms } });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var platform = await _platformService.GetById(id);
        if (platform == null) return NotFound(new { message = "Platform not found" });
        return Ok(new { status = "success", data = new { platform } });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Platform platform)
    {
        var created = await _platformService.Create(platform);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { status = "success", data = new { platform = created } });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Platform platformUpdate)
    {
        var platform = await _platformService.GetById(id);
        if (platform == null) return NotFound(new { message = "Platform not found" });
        platform.Name = platformUpdate.Name;
        platform.Slug = platformUpdate.Slug;
        await _platformService.Update(platform);
        return Ok(new { status = "success", data = new { platform } });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _platformService.Delete(id);
        return Ok(new { status = "success", message = "Platform deleted" });
    }
}
