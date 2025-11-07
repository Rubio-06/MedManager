using MedManager.Domain.Models.Users;

namespace MedManager.Application.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateBirthday { get; set; }
        public Gender Gender { get; set; }
        public string SocialSecurityNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public int PrescriptionCount { get; set; }
        public int AllergyCount { get; set; }
    }
}
