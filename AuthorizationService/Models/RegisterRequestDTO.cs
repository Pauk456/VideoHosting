using System.ComponentModel.DataAnnotations;

namespace AuthorizationService.Model
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirmation { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
