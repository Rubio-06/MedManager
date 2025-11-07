using MedManager.Domain.Models.Users;

namespace MedManager.Web.Models.ViewModels
{
    public class PatientListItemViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateBirthday { get; set; }
        public int Age => DateTime.Now.Year - DateBirthday.Year - (DateTime.Now.DayOfYear < DateBirthday.DayOfYear ? 1 : 0);
        public Gender Gender { get; set; }
        public bool EmailConfirmed { get; set; }
        public int PrescriptionCount { get; set; }
        public int AllergyCount { get; set; }
    }
}