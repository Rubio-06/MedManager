using MedManager.Domain.Models.Users;

namespace MedManager.Web.Models.ViewModels
{
    public class PatientProfileViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public DateTime DateBirthday { get; set; }
        public Gender Gender { get; set; }
        public string SocialSecurityNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public int Age { get; set; }

        // Allergies
        public List<AllergyItemViewModel> Allergies { get; set; } = new();
        public List<AllergyItemViewModel> AvailableAllergies { get; set; } = new();

        // Prescriptions
        public int PrescriptionCount { get; set; }
        public int ActivePrescriptionCount { get; set; }
        public List<PatientPrescriptionViewModel> Prescriptions { get; set; } = new();
    }

    public class AllergyItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PatientPrescriptionViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public List<PrescriptionMedicineDetailViewModel> Medicines { get; set; } = new();
    }

    public class PrescriptionMedicineDetailViewModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }
}
