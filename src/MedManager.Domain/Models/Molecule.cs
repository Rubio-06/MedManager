using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Tables;

namespace MedManager.Domain.Models
{
    public class Molecule
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public virtual ICollection<MedicineMolecule> MedicineMolecules { get; set; } = [];
    }
}