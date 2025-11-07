using MedManager.Domain.Models.Users;

namespace MedManager.Web.Models.ViewModels
{
    public class UserListItemViewModel
    {
        public int PersonId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        // Pour Patient
        public DateTime? DateBirthday { get; set; }
        public string? DoctorName { get; set; }

        // Pour Doctor
        public int? PatientCount { get; set; }
    }
}
