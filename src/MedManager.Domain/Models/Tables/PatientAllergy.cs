using Microsoft.EntityFrameworkCore;

using MedManager.Domain.Models.Users;

namespace MedManager.Domain.Models.Tables
{

    [PrimaryKey(nameof(PatientId), nameof(AllergyId))]
    public class PatientAllergy
    {
        // Foreign Keys
        public int PatientId { get; set; }
        public int AllergyId { get; set; }

        // Navigation Properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Allergy Allergy { get; set; } = null!;
    }
}