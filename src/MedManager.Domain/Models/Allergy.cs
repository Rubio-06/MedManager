using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Tables;

namespace MedManager.Domain.Models
{

    public class Allergy
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Associated Tables (n-n)
        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; } = [];
        public virtual ICollection<MedicineAllergy> MedicineAllergies { get; set; } = [];
    }
}