namespace MedManager.Web.Models.ViewModels
{
    public class CreatePrescriptionViewModel
    {
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string PatientGender { get; set; } = string.Empty;
        public string SocialSecurityNumber { get; set; } = string.Empty;

        public string DoctorFirstName { get; set; } = string.Empty;
        public string DoctorLastName { get; set; } = string.Empty;
        public string DoctorSpecialty { get; set; } = string.Empty;

        public List<PatientAllergyViewModel> PatientAllergies { get; set; } = new();
        public List<MedicineItemViewModel> AvailableMedicines { get; set; } = new();
    }

    public class PatientAllergyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MedicineItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Composition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class PrescriptionMedicineDto
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
