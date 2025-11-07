using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Tables;

namespace MedManager.Domain.Models
{

    public class History
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public required string Description { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Associated Tables (n-n)
        public virtual ICollection<PatientHistory> PatientHistories { get; set; } = [];
    }
}