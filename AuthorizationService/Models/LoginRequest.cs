using System.ComponentModel.DataAnnotations;

namespace AuthorizationService.Models
{
    public class LoginRequest
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
