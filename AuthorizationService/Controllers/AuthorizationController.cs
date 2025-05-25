using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationService.DbModels;
using AuthorizationService.Models;


namespace AuthorizationService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : Controller
{

    private readonly ApplicationDbContext _context;

    public AuthorizationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserData userData)
    {
        if (await _context.Users.AnyAsync(u => u.Login == userData.Login))
        {
            return BadRequest("Пользователь с таким именем уже существует");
        }

        if (await _context.Users.AnyAsync(u => u.Email == userData.Email))
        {
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

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        throw new NotImplementedException();
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
