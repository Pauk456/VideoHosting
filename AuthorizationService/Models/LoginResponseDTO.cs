using AuthorizationService.DbModels;

namespace AuthorizationService.Models
{
    public class LoginResponseDTO
    {
        public Users? User { get; set; }

        public string Token {  get; set; }
    }
}
