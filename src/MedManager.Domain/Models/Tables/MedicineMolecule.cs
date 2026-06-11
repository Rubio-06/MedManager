using Microsoft.EntityFrameworkCore;

using MedManager.Domain.Models;

namespace MedManager.Domain.Models.Tables
{

    [PrimaryKey(nameof(MedicineId), nameof(MoleculeId))]
    public class MedicineMolecule
    {
        // Foreign Keys
        public int MedicineId { get; set; }
        public int MoleculeId { get; set; }

        // Navigation Properties
        public virtual Medicine Medicine { get; set; } = null!;
        public virtual Molecule Molecule { get; set; } = null!;
    }
}