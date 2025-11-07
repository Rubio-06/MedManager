using MedManager.Domain.Models.Users;

namespace MedManager.Application.DTOs
{
    public class CreatePatientDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateBirthday { get; set; }
        public Gender Gender { get; set; }
        public string SocialSecurityNumber { get; set; } = string.Empty;
        public int DoctorId { get; set; }
    }
}
