using System.ComponentModel.DataAnnotations;

namespace MedManager.Domain.Models.Users
{

    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        public required string FirstName { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        public required string LastName { get; set; }

        // Link User
        public string ApplicationUserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}