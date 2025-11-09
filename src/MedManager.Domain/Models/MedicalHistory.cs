using MedManager.Domain.Models.Users;

namespace MedManager.Domain.Models
{
    public class MedicalHistory
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public MedicalHistoryType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Severity Severity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
    }

    public enum MedicalHistoryType
    {
        ChronicDisease,      // Maladie chronique
        Surgery,             // Opération chirurgicale
        Hospitalization,     // Hospitalisation
        FamilyHistory,       // Antécédent familial
        Vaccination,         // Vaccination
        CurrentCondition,    // Condition actuelle
        Other               // Autre
    }

    public enum Severity
    {
        Low,      // Faible
        Medium,   // Modérée
        High,     // Élevée
        Critical  // Critique
    }
}
