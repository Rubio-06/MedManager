namespace MedManager.Web.Models.ViewModels
{
    public class AllergyListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PatientCount { get; set; }
    }
}
