namespace MedManager.Web.Models.ViewModels
{
    public class UserInfoViewModel
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
    }
}
