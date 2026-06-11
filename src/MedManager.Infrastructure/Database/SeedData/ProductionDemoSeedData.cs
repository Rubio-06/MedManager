using MedManager.Domain.Models.Tables;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;
using MedManager.Infrastructure.Database.SeedData.Seeds;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Infrastructure.Database.SeedData
{
    public static class ProductionDemoSeedData
    {
        private const string AdminEmail = "admin@medmanager.com";
        private const string DoctorEmail = "doctor@medmanager.com";
        private const string PatientEmail = "patient@medmanager.com";

        private const string AdminPassword = "Admin123!";
        private const string DoctorPassword = "Doctor123!";
        private const string PatientPassword = "Patient123!";

        private static readonly string[] LegacyDemoEmailsToRemove =
        {
            "jean.dupont@medmanager.com",
            "sophie.martin@medmanager.com",
            "marie.bernard@patient.com",
            "pierre.dubois@patient.com",
            "julie.leroy@patient.com",
            "thomas.moreau@patient.com",
            "emma.simon@patient.com",
            "lucas.laurent@patient.com",
            "camille.lefebvre@patient.com",
            "nicolas.roux@patient.com",
            "sarah.fournier@patient.com",
            "alexandre.girard@patient.com"
        };

        public static async Task SeedAsync(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            bool cleanupLegacyDemoUsers)
        {
            await SeedRoles(roleManager);

            await AllergySeed.SeedAsync(context);
            await MedicineSeed.SeedUpdateAsync(context);
            await MedicineSeed.SeedMedicineAllergiesAsync(context);

            await EnsureAdminAsync(context, userManager);
            var doctor = await EnsureDoctorAsync(context, userManager);
            await EnsurePatientAsync(context, userManager, doctor);

            if (cleanupLegacyDemoUsers)
            {
                await RemoveLegacyDemoUsersAsync(userManager);
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Doctor", "Patient" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task EnsureAdminAsync(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync(AdminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, AdminPassword);
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Erreur création admin: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var adminPerson = await context.Admins.FirstOrDefaultAsync(a => a.ApplicationUserId == adminUser.Id);
            if (adminPerson is null)
            {
                await context.Admins.AddAsync(new Admin
                {
                    FirstName = "Admin",
                    LastName = "System",
                    ApplicationUserId = adminUser.Id,
                    User = adminUser
                });
            }
        }

        private static async Task<Doctor> EnsureDoctorAsync(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            var doctorUser = await userManager.FindByEmailAsync(DoctorEmail);
            if (doctorUser is null)
            {
                doctorUser = new ApplicationUser
                {
                    UserName = DoctorEmail,
                    Email = DoctorEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(doctorUser, DoctorPassword);
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Erreur création doctor: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(doctorUser, "Doctor"))
            {
                await userManager.AddToRoleAsync(doctorUser, "Doctor");
            }

            var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.ApplicationUserId == doctorUser.Id);
            if (doctor is null)
            {
                doctor = new Doctor
                {
                    FirstName = "Demo",
                    LastName = "Doctor",
                    ApplicationUserId = doctorUser.Id,
                    User = doctorUser
                };

                await context.Doctors.AddAsync(doctor);
                await context.SaveChangesAsync();
            }

            return doctor;
        }

        private static async Task EnsurePatientAsync(DatabaseContext context, UserManager<ApplicationUser> userManager, Doctor doctor)
        {
            var patientUser = await userManager.FindByEmailAsync(PatientEmail);
            if (patientUser is null)
            {
                patientUser = new ApplicationUser
                {
                    UserName = PatientEmail,
                    Email = PatientEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(patientUser, PatientPassword);
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Erreur création patient: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(patientUser, "Patient"))
            {
                await userManager.AddToRoleAsync(patientUser, "Patient");
            }

            var patient = await context.Patients.FirstOrDefaultAsync(p => p.ApplicationUserId == patientUser.Id);
            if (patient is null)
            {
                patient = new Patient
                {
                    FirstName = "Demo",
                    LastName = "Patient",
                    Gender = Gender.Male,
                    DateBirthday = new DateTime(1990, 1, 1),
                    SocialSecurityNumber = "190010112345678",
                    DoctorId = doctor.Id,
                    ApplicationUserId = patientUser.Id,
                    User = patientUser
                };

                await context.Patients.AddAsync(patient);
            }

            var firstAllergy = await context.Allergies.OrderBy(a => a.Id).FirstOrDefaultAsync();
            if (firstAllergy is not null)
            {
                var hasAllergy = await context.PatientAllergies.AnyAsync(pa => pa.PatientId == patient.Id && pa.AllergyId == firstAllergy.Id);
                if (!hasAllergy)
                {
                    await context.PatientAllergies.AddAsync(new PatientAllergy
                    {
                        PatientId = patient.Id,
                        AllergyId = firstAllergy.Id
                    });
                }
            }
        }

        private static async Task RemoveLegacyDemoUsersAsync(UserManager<ApplicationUser> userManager)
        {
            foreach (var email in LegacyDemoEmailsToRemove)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    await userManager.DeleteAsync(user);
                }
            }
        }
    }
}