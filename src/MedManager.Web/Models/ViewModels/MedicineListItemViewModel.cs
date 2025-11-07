using System.ComponentModel.DataAnnotations;

namespace MedManager.Web.Models.ViewModels
{
    public class MedicineListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Dosage { get; set; }
        public int PrescriptionCount { get; set; }
    }
}
