using MedManager.Domain.Models.Tables;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Infrastructure.Database.SeedData.Seeds
{
    public static class PatientSeed
    {
        public static async Task SeedAsync(
            DatabaseContext context, 
            UserManager<ApplicationUser> userManager)
        {
            // Récupérer les docteurs
            var doctors = await context.Doctors.ToListAsync();
            if (doctors.Count < 2)
            {
                throw new Exception("Les docteurs doivent être créés avant les patients");
            }

            var doctor1 = doctors[0];
            var doctor2 = doctors[1];

            var patientsData = new[]
            {
                new { 
                    FirstName = "Marie", LastName = "Bernard", Email = "marie.bernard@patient.com", 
                    Gender = Gender.Female, Birthday = new DateTime(1985, 3, 15), 
                    SSN = "185035012345678", Doctor = doctor1, 
                    Allergies = new[] { "Pénicilline", "Aspirine" } 
                },
                new { 
                    FirstName = "Pierre", LastName = "Dubois", Email = "pierre.dubois@patient.com", 
                    Gender = Gender.Male, Birthday = new DateTime(1992, 7, 22), 
                    SSN = "192075012345679", Doctor = doctor1, 
                    Allergies = new[] { "Ibuprofène" } 
                },
                new { 
                    FirstName = "Julie", LastName = "Leroy", Email = "julie.leroy@patient.com", 
                    Gender = Gender.Female, Birthday = new DateTime(1978, 11, 5), 
                    SSN = "178115012345680", Doctor = doctor1, 
                    Allergies = new[] { "Codéine" } 
                },
                new { 
                    FirstName = "Thomas", LastName = "Moreau", Email = "thomas.moreau@patient.com", 
                    Gender = Gender.Male, Birthday = new DateTime(1995, 1, 18), 
                    SSN = "195015012345681", Doctor = doctor2, 
                    Allergies = new[] { "Lactose" } 
                },
                new { 
                    FirstName = "Emma", LastName = "Simon", Email = "emma.simon@patient.com", 
                    Gender = Gender.Female, Birthday = new DateTime(1988, 9, 30), 
                    SSN = "188095012345682", Doctor = doctor2, 
                    Allergies = new[] { "Pénicilline", "Sulfamides" } 
                },
                new { 
                    FirstName = "Lucas", LastName = "Laurent", Email = "lucas.laurent@patient.com", 
                    Gender = Gender.Male, Birthday = new DateTime(1990, 4, 12), 
                    SSN = "190045012345683", Doctor = doctor2, 
                    Allergies = Array.Empty<string>() 
                },
                new { 
                    FirstName = "Camille", LastName = "Lefebvre", Email = "camille.lefebvre@patient.com", 
                    Gender = Gender.Female, Birthday = new DateTime(1982, 6, 25), 
                    SSN = "182065012345684", Doctor = doctor1, 
                    Allergies = new[] { "Aspirine", "Ibuprofène" } 
                },
                new { 
                    FirstName = "Nicolas", LastName = "Roux", Email = "nicolas.roux@patient.com", 
                    Gender = Gender.Male, Birthday = new DateTime(1975, 12, 8), 
                    SSN = "175125012345685", Doctor = doctor2, 
                    Allergies = Array.Empty<string>() 
                },
                new { 
                    FirstName = "Sarah", LastName = "Fournier", Email = "sarah.fournier@patient.com", 
                    Gender = Gender.Female, Birthday = new DateTime(1998, 2, 14), 
                    SSN = "198025012345686", Doctor = doctor1, 
                    Allergies = new[] { "Codéine" } 
                },
                new { 
                    FirstName = "Alexandre", LastName = "Girard", Email = "alexandre.girard@patient.com", 
                    Gender = Gender.Male, Birthday = new DateTime(1987, 8, 21), 
                    SSN = "187085012345687", Doctor = doctor2, 
                    Allergies = new[] { "Pénicilline" } 
                }
            };

            foreach (var patientData in patientsData)
            {
                var existingUser = await userManager.FindByEmailAsync(patientData.Email);
                if (existingUser != null)
                {
                    continue; // Ce patient existe déjà
                }

                // Créer l'utilisateur
                var patientUser = new ApplicationUser
                {
                    UserName = patientData.Email,
                    Email = patientData.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(patientUser, "Patient123!");
                if (!result.Succeeded)
                {
                    throw new Exception($"Erreur lors de la création du patient {patientData.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await userManager.AddToRoleAsync(patientUser, "Patient");

                // Créer l'entité Patient
                var patient = new Patient
                {
                    FirstName = patientData.FirstName,
                    LastName = patientData.LastName,
                    Gender = patientData.Gender,
                    DateBirthday = patientData.Birthday,
                    SocialSecurityNumber = patientData.SSN,
                    ApplicationUserId = patientUser.Id,
                    User = patientUser,
                    DoctorId = patientData.Doctor.Id
                };

                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();

                // Ajouter les allergies du patient
                foreach (var allergyName in patientData.Allergies)
                {
                    var allergy = await context.Allergies.FirstOrDefaultAsync(a => a.Name == allergyName);
                    if (allergy != null)
                    {
                        await context.PatientAllergies.AddAsync(new PatientAllergy
                        {
                            PatientId = patient.Id,
                            AllergyId = allergy.Id
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
