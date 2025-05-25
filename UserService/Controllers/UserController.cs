
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DbModels;
using UserService.Models;


namespace UserService.Controllers;

using System;
using UserService.Models;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetAnime/{id}")]
    public async Task<ActionResult<UserAnimeAllDTO>> GetAnimeFromUser(int id)
    {
        var animeFromUser = await _context.UserAnime
            .Where(ua => ua.IdUser == id)
            .Select(ua => new UserAnimeAllDTO
            {
                IdUser = ua.IdUser,
                IdAnime = ua.IdAnime,
                Rating = ua.Rating,
                Status = ua.Status
            })
            .ToListAsync();
        return Ok(animeFromUser);
    }

    [HttpPost("AddAnime")]
    public async Task<ActionResult<UserAnimeAllDTO>> AddAnimeToUser([FromBody] UserAnimeAllDTO anime)
    {
        var newUserAnime = new UserAnime
        {
            IdUser = anime.IdUser,
            IdAnime = anime.IdAnime,
            Rating = anime.Rating,
            Status = anime.Status
        };
        _context.UserAnime.Add(newUserAnime);
        await _context.SaveChangesAsync();
        return Ok($"{anime.IdAnime} успешно добавлено в список");
    }

    [HttpPost("UpdateRating")]
    public async Task<ActionResult<UserAnimeRatingDTO>> UpdateRating([FromBody] UserAnimeRatingDTO anime)
    {
        var userAnime = await _context.UserAnime
        .FirstOrDefaultAsync(a => (a.IdAnime == anime.IdAnime && a.IdUser == anime.IdUser));
        if (userAnime == null)
        {
            return NotFound($"Запись с ID {anime.IdAnime} не найдена");
        }
        userAnime.Rating = anime.Rating;
        await _context.SaveChangesAsync();
        return Ok("Оценка обновлена");
    }

    [HttpPost("UpdateStatus")]
    public async Task<ActionResult<UserAnimeStatusDTO>> UpdateStatus([FromBody] UserAnimeStatusDTO anime)
    {
        var userAnime = await _context.UserAnime
        .FirstOrDefaultAsync(a => (a.IdAnime == anime.IdAnime && a.IdUser == anime.IdUser));
        if (userAnime == null)
        {
            return NotFound($"Запись с ID {anime.IdAnime} не найдена");
        }
        userAnime.Status = anime.Status;
        await _context.SaveChangesAsync();
        return Ok("Стату обновлен");
    }

    [HttpPost("DeleteAnime")]
    public async Task<ActionResult<UserAnime>> DeleteAnime([FromBody] UserAnime anime)
    {
        var userAnime = await _context.UserAnime
        .FirstOrDefaultAsync(a => (a.IdAnime == anime.IdAnime && a.IdUser == anime.IdUser));
        if (userAnime == null)
        {
            return NotFound($"Запись с ID {anime.IdAnime} не найдена");
        }
        _context.UserAnime.Remove(userAnime);
        await _context.SaveChangesAsync();
        return Ok("Аниме удалено");
    }
}