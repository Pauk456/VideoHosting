
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


    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetUser(int id)
    {
        var user = await _context.Users
           .Where(ua => ua.Id == id)
           .Select(ua => new UserData
           {
               Login = ua.Login,
               Email = ua.Email,
               Password = ua.Password
           })
           .ToListAsync();
        return Ok(user);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Register([FromBody] UserData userData)
    {
        if (await _context.Users.AnyAsync(u => u.Login == userData.Login))
        {
            return BadRequest("Пользователь с таким именем уже существует");
        }

        if (await _context.Users.AnyAsync(u => u.Email == userData.Email)) {
            return BadRequest("Пользователь с такой почтой уже существует");
        }

        if (string.IsNullOrWhiteSpace(userData.Email) || !IsValidEmail(userData.Email))
        {
            return BadRequest("Некорректный email");
        }

        if (userData.Login.Length < 3)
        {
            return BadRequest("Имя пользователя должно быть больше 2 символов");
        }

        if (userData.Password.Length < 6)
        {
            return BadRequest("Пароль должен содержать больше 5 символов");
        }

        string passwordHash = PasswordHasher.HashPassword(userData.Password);

        var user = new Users
        {
            Login = userData.Login,
            Password = passwordHash,
            Email = userData.Email
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok(user.Id);
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
        return Ok("Статус обновлен");
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

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}