using System.ComponentModel.DataAnnotations;

namespace AuthorizationService.Models
{
    public class LoginRequestDTO
    {
        [Required]
        public int Id { get; set; } 
        [Required]
        public string Password { get; set; }

        [Required]
        public string? Login { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
