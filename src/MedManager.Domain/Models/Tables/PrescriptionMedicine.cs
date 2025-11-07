using Microsoft.EntityFrameworkCore;

namespace MedManager.Domain.Models.Tables
{
    [PrimaryKey(nameof(PrescriptionId), nameof(MedicineId))]
    public class PrescriptionMedicine
    {
        // Foreign Keys
        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }

        // Navigation Properties
        public virtual Prescription Prescription { get; set; } = null!;
        public virtual Medicine Medicine { get; set; } = null!;

        // Additional Properties
        public int Quantity { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }
}