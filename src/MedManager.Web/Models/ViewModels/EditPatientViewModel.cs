using System.ComponentModel.DataAnnotations;

using MedManager.Domain.Models.Users;

namespace MedManager.Web.Models.ViewModels
{
    public class EditPatientViewModel
    {
        public int Id { get; set; }
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
        [EmailAddress(ErrorMessage = "Email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est requise")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime DateBirthday { get; set; }

        [Required(ErrorMessage = "Le sexe est requis")]
        [Display(Name = "Sexe")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Le numéro de sécurité sociale est requis")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "Le numéro de sécurité sociale doit contenir 15 caractères")]
        [Display(Name = "Numéro de sécurité sociale")]
        public string SocialSecurityNumber { get; set; } = string.Empty;

        [Display(Name = "Email confirmé")]
        public bool EmailConfirmed { get; set; }
    }
}
