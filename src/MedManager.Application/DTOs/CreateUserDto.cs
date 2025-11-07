using MedManager.Domain.Models.Users;

namespace MedManager.Application.DTOs
{
    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Champs spécifiques Patient
        public Gender? Gender { get; set; }
        public DateTime? DateBirthday { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public int? DoctorId { get; set; }
    }
}
