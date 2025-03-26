using System.ComponentModel.DataAnnotations;

namespace AuthorizationService.Model
{
    public class RegisterRequest
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirmation { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
