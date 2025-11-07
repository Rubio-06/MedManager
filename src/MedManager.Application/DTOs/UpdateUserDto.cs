using MedManager.Domain.Models.Users;

namespace MedManager.Application.DTOs
{
    public class UpdateUserDto
    {
        public int PersonId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? NewPassword { get; set; }

        // Champs spécifiques Patient
        public Gender? Gender { get; set; }
        public DateTime? DateBirthday { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public int? DoctorId { get; set; }
    }
}
