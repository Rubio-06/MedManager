using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Tables;

namespace MedManager.Domain.Models
{

    public class Medicine
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Dosage { get; set; }

        [MaxLength(1000)]
        public string? SideEffects { get; set; }

        // Components (composition)
        public virtual ICollection<MedicineComponent> Components { get; set; } = [];

        // Associated Tables (n-n)
        public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = [];
        public virtual ICollection<MedicineAllergy> MedicineAllergies { get; set; } = [];
        public virtual ICollection<MedicineHistory> MedicineHistories { get; set; } = [];
    }
}