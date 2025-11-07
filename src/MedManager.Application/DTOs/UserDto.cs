namespace MedManager.Application.DTOs
{
    public class UserDto
    {
        public int PersonId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public bool EmailConfirmed { get; set; }

        // Champs spécifiques Patient
        public DateTime? DateBirthday { get; set; }
        public string? DoctorName { get; set; }

        // Champs spécifiques Doctor
        public int? PatientCount { get; set; }
    }
}
