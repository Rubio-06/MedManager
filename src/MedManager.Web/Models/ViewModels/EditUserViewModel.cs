using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Users;

namespace MedManager.Web.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int PersonId { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères")]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 50 caractères")]
        [Display(Name = "Nom")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe (optionnel)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le mot de passe")]
        public string? ConfirmPassword { get; set; }

        // Champs spécifiques pour Patient
        [Display(Name = "Sexe")]
        public Gender? Gender { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime? DateBirthday { get; set; }

        [StringLength(15, MinimumLength = 15, ErrorMessage = "Le numéro de sécurité sociale doit contenir 15 caractères")]
        [Display(Name = "Numéro de sécurité sociale")]
        public string? SocialSecurityNumber { get; set; }

        [Display(Name = "Médecin traitant")]
        public int? DoctorId { get; set; }
    }
}
