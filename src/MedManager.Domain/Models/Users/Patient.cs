using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models;
using MedManager.Domain.Models.Tables;

namespace MedManager.Domain.Models.Users
{

    public enum Gender
    {
        Male,
        Female
    }

    public class Patient : Person
    {
        [Required, EnumDataType(typeof(Gender))]
        public required Gender Gender { get; set; }

        [Required, DataType(DataType.Date)]
        public required DateTime DateBirthday { get; set; }

        [Required, MinLength(15), MaxLength(15)]
        public required string SocialSecurityNumber { get; set; }

        // Foreign Key
        public int? DoctorId { get; set; }

        // Navigation Properties
        public virtual Doctor? Doctor { get; set; }

        // Associated Tables (1-n)
        public virtual ICollection<Prescription> Prescriptions { get; set; } = [];
        public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = [];

        // Associated Tables (n-n)
        public virtual ICollection<PatientHistory> PatientHistories { get; set; } = [];
        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; } = [];
    }
}