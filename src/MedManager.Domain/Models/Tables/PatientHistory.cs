using Microsoft.EntityFrameworkCore;

using MedManager.Domain.Models.Users;

namespace MedManager.Domain.Models.Tables
{
    [PrimaryKey(nameof(PatientId), nameof(HistoryId))]
    public class PatientHistory
    {
        // Foreign Keys
        public int PatientId { get; set; }
        public int HistoryId { get; set; }

        // Navigation Properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual History History { get; set; } = null!;
    }
}