using Microsoft.EntityFrameworkCore;

namespace MedManager.Domain.Models.Tables
{

    [PrimaryKey(nameof(MedicineId), nameof(HistoryId))]
    public class MedicineHistory
    {
        // Foreign Keys
        public int MedicineId { get; set; }
        public int HistoryId { get; set; }

        // Navigation Properties
        public virtual Medicine Medicine { get; set; } = null!;
        public virtual History History { get; set; } = null!;
    }
}