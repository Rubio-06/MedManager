using MedManager.Domain.Models;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;

using Microsoft.AspNetCore.Identity;

namespace MedManager.Infrastructure.Database.SeedData
{
    public static class DevelopmentSeedData
    {
        public static async Task SeedAsync(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedUsersAsync(context, userManager);
            await SeedMedicinesAsync(context);
            await SeedAllergiesAsync(context);
            await SeedHistoriesAsync(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(DatabaseContext context, UserManager<ApplicationUser> userManager)
        {
            // Admin
            var adminUser = await userManager.FindByEmailAsync("admin@medmanager.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@medmanager.com",
                    Email = "admin@medmanager.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");

                var admin = new Admin
                {
                    FirstName = "Admin",
                    LastName = "System",
                    ApplicationUserId = adminUser.Id,
                    User = adminUser
                };
                context.Admins.Add(admin);
            }

            // Doctor
            var doctorUser = await userManager.FindByEmailAsync("doctor@medmanager.com");
            if (doctorUser == null)
            {
                doctorUser = new ApplicationUser
                {
                    UserName = "doctor@medmanager.com",
                    Email = "doctor@medmanager.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(doctorUser, "Doctor123!");
                await userManager.AddToRoleAsync(doctorUser, "Doctor");

                var doctor = new Doctor
                {
                    FirstName = "Jean",
                    LastName = "Dupont",
                    ApplicationUserId = doctorUser.Id,
                    User = doctorUser
                };
                context.Doctors.Add(doctor);
            }

            // Patient
            var patientUser = await userManager.FindByEmailAsync("patient@medmanager.com");
            if (patientUser == null)
            {
                patientUser = new ApplicationUser
                {
                    UserName = "patient@medmanager.com",
                    Email = "patient@medmanager.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(patientUser, "Patient123!");
                await userManager.AddToRoleAsync(patientUser, "Patient");

                var patient = new Patient
                {
                    FirstName = "Marie",
                    LastName = "Martin",
                    Gender = Gender.Female,
                    DateBirthday = new DateTime(1990, 5, 15),
                    SocialSecurityNumber = "190055012345678",
                    ApplicationUserId = patientUser.Id,
                    User = patientUser
                };
                context.Patients.Add(patient);
            }
        }

        private static async Task SeedMedicinesAsync(DatabaseContext context)
        {
            if (!context.Medicines.Any())
            {
                var medicines = new List<Medicine>
                {
                    new Medicine
                    {
                        Name = "Paracétamol",
                        Description = "Antalgique et antipyrétique"
                    },
                    new Medicine
                    {
                        Name = "Ibuprofène",
                        Description = "Anti-inflammatoire non stéroïdien (AINS)"
                    },
                    new Medicine
                    {
                        Name = "Amoxicilline",
                        Description = "Antibiotique de la famille des pénicillines"
                    },
                    new Medicine
                    {
                        Name = "Doliprane",
                        Description = "Antalgique à base de paracétamol"
                    },
                    new Medicine
                    {
                        Name = "Aspégic",
                        Description = "Antiagrégant plaquettaire et antalgique"
                    }
                };

                context.Medicines.AddRange(medicines);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAllergiesAsync(DatabaseContext context)
        {
            if (!context.Allergies.Any())
            {
                var allergies = new List<Allergy>
                {
                    new Allergy
                    {
                        Name = "Pénicilline",
                        Description = "Allergie aux antibiotiques de type pénicilline"
                    },
                    new Allergy
                    {
                        Name = "Arachides",
                        Description = "Allergie alimentaire aux cacahuètes"
                    },
                    new Allergy
                    {
                        Name = "Latex",
                        Description = "Allergie au latex naturel"
                    },
                    new Allergy
                    {
                        Name = "Pollen",
                        Description = "Allergie saisonnière au pollen"
                    },
                    new Allergy
                    {
                        Name = "Aspirine",
                        Description = "Allergie à l'acide acétylsalicylique"
                    }
                };

                context.Allergies.AddRange(allergies);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedHistoriesAsync(DatabaseContext context)
        {
            if (!context.Histories.Any())
            {
                var histories = new List<History>
                {
                    new History
                    {
                        Description = "Hypertension artérielle",
                        Date = DateTime.Now.AddYears(-5)
                    },
                    new History
                    {
                        Description = "Diabète de type 2",
                        Date = DateTime.Now.AddYears(-3)
                    },
                    new History
                    {
                        Description = "Asthme",
                        Date = DateTime.Now.AddYears(-10)
                    },
                    new History
                    {
                        Description = "Fracture du bras droit",
                        Date = DateTime.Now.AddYears(-2)
                    }
                };

                context.Histories.AddRange(histories);
                await context.SaveChangesAsync();
            }
        }
    }
}
