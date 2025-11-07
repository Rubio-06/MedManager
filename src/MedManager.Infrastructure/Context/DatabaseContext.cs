using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MedManager.Domain.Models;
using MedManager.Domain.Models.Tables;
using MedManager.Domain.Models.Users;

namespace MedManager.Infrastructure.Context
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // DbSets for main entities
        public DbSet<Person> Persons { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        // DbSets for junction tables (many-to-many relationships)
        public DbSet<MedicineAllergy> MedicineAllergies { get; set; }
        public DbSet<MedicineHistory> MedicineHistories { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }
        public DbSet<PatientHistory> PatientHistories { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Table-per-Hierarchy (TPH) inheritance for Person
            builder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Person>("Person")
                .HasValue<Admin>("Admin")
                .HasValue<Doctor>("Doctor")
                .HasValue<Patient>("Patient");

            // Configure Person -> ApplicationUser relationship
            builder.Entity<Person>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Doctor -> Patient relationship (one-to-many)
            builder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Prescription relationships
            builder.Entity<Prescription>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many relationships using junction tables

            // MedicineAllergy
            builder.Entity<MedicineAllergy>()
                .HasOne(ma => ma.Medicine)
                .WithMany(m => m.MedicineAllergies)
                .HasForeignKey(ma => ma.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MedicineAllergy>()
                .HasOne(ma => ma.Allergy)
                .WithMany()
                .HasForeignKey(ma => ma.AllergyId)
                .OnDelete(DeleteBehavior.Cascade);

            // MedicineHistory
            builder.Entity<MedicineHistory>()
                .HasOne(mh => mh.Medicine)
                .WithMany(m => m.MedicineHistories)
                .HasForeignKey(mh => mh.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MedicineHistory>()
                .HasOne(mh => mh.History)
                .WithMany()
                .HasForeignKey(mh => mh.HistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // PatientAllergy
            builder.Entity<PatientAllergy>()
                .HasOne(pa => pa.Patient)
                .WithMany(p => p.PatientAllergies)
                .HasForeignKey(pa => pa.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PatientAllergy>()
                .HasOne(pa => pa.Allergy)
                .WithMany(a => a.PatientAllergies)
                .HasForeignKey(pa => pa.AllergyId)
                .OnDelete(DeleteBehavior.Cascade);

            // PatientHistory
            builder.Entity<PatientHistory>()
                .HasOne(ph => ph.Patient)
                .WithMany(p => p.PatientHistories)
                .HasForeignKey(ph => ph.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PatientHistory>()
                .HasOne(ph => ph.History)
                .WithMany(h => h.PatientHistories)
                .HasForeignKey(ph => ph.HistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // PrescriptionMedicine
            builder.Entity<PrescriptionMedicine>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescriptionMedicines)
                .HasForeignKey(pm => pm.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PrescriptionMedicine>()
                .HasOne(pm => pm.Medicine)
                .WithMany(m => m.PrescriptionMedicines)
                .HasForeignKey(pm => pm.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index configurations for better performance
            builder.Entity<Patient>()
                .HasIndex(p => p.SocialSecurityNumber)
                .IsUnique();

            builder.Entity<Medicine>()
                .HasIndex(m => m.Name);

            builder.Entity<Allergy>()
                .HasIndex(a => a.Name);
        }
    }
}