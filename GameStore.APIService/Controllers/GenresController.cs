using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameStore.Entities.Games;
using GameStore.Services;

namespace GameStore.APIService.Controllers;

[Route("api/genres")]
[ApiController]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    public GenresController(IGenreService genreService) => _genreService = genreService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var genres = await _genreService.GetAll();
        return Ok(new { status = "success", data = new { genres } });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var genre = await _genreService.GetById(id);
        if (genre == null) return NotFound(new { message = "Genre not found" });
        return Ok(new { status = "success", data = new { genre } });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] GenreDto dto)
    {
        var genre = new Genre { Name = dto.Name, Slug = dto.Slug };
        var created = await _genreService.Create(genre);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { status = "success", data = new { genre = created } });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] GenreDto dto)
    {
        var genre = await _genreService.GetById(id);
        if (genre == null) return NotFound(new { message = "Genre not found" });
        genre.Name = dto.Name;
        genre.Slug = dto.Slug;
        await _genreService.Update(genre);
        return Ok(new { status = "success", data = new { genre } });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _genreService.Delete(id);
        return Ok(new { status = "success", message = "Genre deleted" });
    }
}

public class GenreDto { public string Name { get; set; } = ""; public string Slug { get; set; } = ""; }
