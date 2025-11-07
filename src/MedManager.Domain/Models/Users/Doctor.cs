using MedManager.Domain.Models;

namespace MedManager.Domain.Models.Users
{

    public class Doctor : Person
    {
        // Associated Tables (1-n)
        public virtual ICollection<Prescription> Prescriptions { get; set; } = [];
        public virtual ICollection<Patient> Patients { get; set; } = [];
    }
}