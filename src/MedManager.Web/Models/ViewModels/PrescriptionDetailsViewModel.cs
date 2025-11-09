namespace MedManager.Web.Models.ViewModels
{
    public class PrescriptionDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        // Patient Info
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public string PatientFullName => $"{PatientFirstName} {PatientLastName}";
        public string PatientEmail { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string PatientGender { get; set; } = string.Empty;
        public string SocialSecurityNumber { get; set; } = string.Empty;

        // Doctor Info
        public string DoctorFirstName { get; set; } = string.Empty;
        public string DoctorLastName { get; set; } = string.Empty;
        public string DoctorFullName => $"Dr. {DoctorFirstName} {DoctorLastName}";

        // Allergies
        public List<PatientAllergyViewModel> PatientAllergies { get; set; } = new();

        // Medicines
        public List<PrescriptionMedicineViewModel> Medicines { get; set; } = new();
    }

    public class PrescriptionMedicineViewModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Composition { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }
}
