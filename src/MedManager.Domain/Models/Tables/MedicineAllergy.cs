using Microsoft.EntityFrameworkCore;

namespace MedManager.Domain.Models.Tables
{

    [PrimaryKey(nameof(MedicineId), nameof(AllergyId))]
    public class MedicineAllergy
    {
        // Foreign Keys
        public int MedicineId { get; set; }
        public int AllergyId { get; set; }

        // Navigation Properties
        public virtual Medicine Medicine { get; set; } = null!;
        public virtual Allergy Allergy { get; set; } = null!;
    }
}