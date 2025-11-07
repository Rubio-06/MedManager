using System.ComponentModel.DataAnnotations;

namespace MedManager.Web.Models.ViewModels
{
    public class CreateMedicineViewModel
    {
        [Required(ErrorMessage = "Le nom du médicament est requis")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
        [Display(Name = "Nom du médicament")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(200, ErrorMessage = "Le dosage ne peut pas dépasser 200 caractères")]
        [Display(Name = "Dosage recommandé")]
        public string? Dosage { get; set; }

        [StringLength(1000, ErrorMessage = "Les effets secondaires ne peuvent pas dépasser 1000 caractères")]
        [Display(Name = "Effets secondaires")]
        public string? SideEffects { get; set; }
    }
}
