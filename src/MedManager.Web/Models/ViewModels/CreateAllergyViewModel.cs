using System.ComponentModel.DataAnnotations;

namespace MedManager.Web.Models.ViewModels
{
    public class CreateAllergyViewModel
    {
        [Required(ErrorMessage = "Le nom de l'allergie est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        [Display(Name = "Nom de l'allergie")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
