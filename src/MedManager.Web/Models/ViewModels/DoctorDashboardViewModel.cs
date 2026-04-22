namespace MedManager.Web.Models.ViewModels
{
    public class DoctorDashboardViewModel
    {
        // Informations du médecin
        public int DoctorId { get; set; }
        public string DoctorFirstName { get; set; } = string.Empty;
        public string DoctorLastName { get; set; } = string.Empty;

        // Statistiques générales affichées dans les cartes
        public int TotalPatients { get; set; }
        public int TotalPrescriptions { get; set; }
        public int PrescriptionsThisMonth { get; set; }
        public int MalePatients { get; set; }
        public int FemalePatients { get; set; }

        // Données pour graphique circulaire : répartition par âge (0-18, 19-60, 60+)
        public Dictionary<string, int> AgeRanges { get; set; } = new();

        // Données pour graphique linéaire : évolution des ordonnances sur 12 mois
        public Dictionary<string, int> PrescriptionsByMonth { get; set; } = new();

        // Top 10 des médicaments les plus prescrits ce mois
        public List<TopMedicineViewModel> TopMedicinesThisMonth { get; set; } = new();
    }

    public class TopMedicineViewModel
    {
        public string MedicineName { get; set; } = string.Empty;
        public int PrescriptionCount { get; set; }
    }
}
