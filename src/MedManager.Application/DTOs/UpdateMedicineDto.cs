namespace MedManager.Application.DTOs
{
    public class UpdateMedicineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Dosage { get; set; }
        public string? SideEffects { get; set; }
    }
}
