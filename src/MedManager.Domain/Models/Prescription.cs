using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Users;
using MedManager.Domain.Models.Tables;

namespace MedManager.Domain.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        // Foreign Keys
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Navigation Properties
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;

        // Associated Tables (n-n)
        public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = [];
    }
}