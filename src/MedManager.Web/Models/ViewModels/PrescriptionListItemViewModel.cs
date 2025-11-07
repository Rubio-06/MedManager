using MedManager.Domain.Models.Users;

namespace MedManager.Web.Models.ViewModels
{
    public class PrescriptionListItemViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int MedicineCount { get; set; }
        public string Status { get; set; } = "Active"; // Active, Terminée, etc.
    }
}
