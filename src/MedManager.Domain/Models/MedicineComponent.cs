using System.ComponentModel.DataAnnotations;

namespace MedManager.Domain.Models
{
    public class MedicineComponent
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public string? Dosage { get; set; }

        // Foreign Key
        public int MedicineId { get; set; }

        // Navigation Property
        public virtual Medicine Medicine { get; set; } = null!;
    }
}
