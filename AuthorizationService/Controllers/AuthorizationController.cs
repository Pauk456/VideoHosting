using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationService.DbModels;
using AuthorizationService.Models;
using AuthorizationService.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


namespace AuthorizationService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : Controller
{

    private readonly ApplicationDbContext _context;

    private string secretKey;

    public AuthorizationController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequestDTO userData)
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

        if (userData.Password != userData.PasswordConfirmation)
        {
            return BadRequest("Пароли не совпадают");
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
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
    {
        var user = _context.Users.FirstOrDefault(u => (u.Login == request.Login || u.Email == request.Email));
        var ans = PasswordHasher.VerifyPassword(request.Password, user.Password);
        if (user == null && !ans)
        {
            return BadRequest(PasswordHasher.HashPassword(request.Password));
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Name, user.Login)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var loginResponseDTO = new LoginResponseDTO()
        {
            Token = tokenHandler.WriteToken(token),
            User = user
        };
        return Ok(loginResponseDTO);
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
