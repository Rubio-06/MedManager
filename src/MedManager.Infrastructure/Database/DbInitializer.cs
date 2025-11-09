using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MedManager.Domain.Models;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;

namespace MedManager.Infrastructure.Database
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string environment)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                // CRÉER LA TABLE MEDICINECOMPONENTS EN PREMIER (avant MigrateAsync)
                await EnsureMedicineComponentsTableExists(context);

                // CRÉER LA TABLE MEDICALHISTORIES
                await EnsureMedicalHistoriesTableExists(context);

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Seed data based on environment
                switch (environment.ToLower())
                {
                    case "development":
                        await SeedDevelopmentData(context, userManager, roleManager);
                        break;
                    case "test":
                        await SeedTestData(context, userManager, roleManager);
                        break;
                    case "production":
                        await SeedProductionData(context, userManager, roleManager);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR INITIALISATION DATABASE: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                throw;
            }
        }

        private static async Task EnsureMedicineComponentsTableExists(DatabaseContext context)
        {
            try
            {
                Console.WriteLine("🔍 Vérification de la table MedicineComponents...");
                
                // Créer la table directement, MySQL ignore si elle existe déjà
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS `MedicineComponents` (
                        `Id` int NOT NULL AUTO_INCREMENT,
                        `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                        `Dosage` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                        `MedicineId` int NOT NULL,
                        CONSTRAINT `PK_MedicineComponents` PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_MedicineComponents_Medicines_MedicineId` 
                            FOREIGN KEY (`MedicineId`) 
                            REFERENCES `Medicines` (`Id`) 
                            ON DELETE CASCADE
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                ");

                // Créer l'index
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE INDEX IF NOT EXISTS `IX_MedicineComponents_MedicineId` 
                    ON `MedicineComponents` (`MedicineId`);
                ");

                Console.WriteLine("✓ Table MedicineComponents OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Erreur lors de la création de MedicineComponents: {ex.Message}");
                // On continue quand même, peut-être que la table existe déjà
            }
        }

        private static async Task EnsureMedicalHistoriesTableExists(DatabaseContext context)
        {
            try
            {
                Console.WriteLine("🔍 Vérification de la table MedicalHistories...");
                
                // Créer la table directement, MySQL ignore si elle existe déjà
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS `MedicalHistories` (
                        `Id` int NOT NULL AUTO_INCREMENT,
                        `PatientId` int NOT NULL,
                        `Type` int NOT NULL,
                        `Title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                        `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
                        `Date` datetime(6) NOT NULL,
                        `Severity` int NOT NULL,
                        `CreatedAt` datetime(6) NOT NULL,
                        `UpdatedAt` datetime(6) NOT NULL,
                        CONSTRAINT `PK_MedicalHistories` PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_MedicalHistories_Persons_PatientId` 
                            FOREIGN KEY (`PatientId`) 
                            REFERENCES `Persons` (`Id`) 
                            ON DELETE CASCADE
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                ");

                // Créer l'index
                await context.Database.ExecuteSqlRawAsync(@"
                    CREATE INDEX IF NOT EXISTS `IX_MedicalHistories_PatientId` 
                    ON `MedicalHistories` (`PatientId`);
                ");

                Console.WriteLine("✓ Table MedicalHistories OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Erreur lors de la création de MedicalHistories: {ex.Message}");
                // On continue quand même, peut-être que la table existe déjà
            }
        }

        private static async Task SeedDevelopmentData(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed roles
            await SeedRoles(roleManager);

            // Seed development users and data
            await SeedData.DevelopmentSeedData.SeedAsync(context, userManager, roleManager);

            await context.SaveChangesAsync();
        }

        private static async Task SeedTestData(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed roles
            await SeedRoles(roleManager);

            // Seed test users and data
            // TODO: Add test-specific seed data here

            await context.SaveChangesAsync();
        }

        private static async Task SeedProductionData(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed roles only for production (no test data)
            await SeedRoles(roleManager);

            // Production should not have seeded data except for essential configurations
            // TODO: Add production-specific configurations if needed

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Doctor", "Patient" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        // Helper methods for seeding specific entities

        private static async Task<ApplicationUser?> CreateUserIfNotExists(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    return user;
                }
            }
            return user;
        }

        private static void SeedMedicinesIfEmpty(DatabaseContext context)
        {
            if (!context.Medicines.Any())
            {
                // TODO: Add medicines seed data
                // Example:
                // context.Medicines.AddRange(
                //     new Medicine { Name = "Aspirin", Description = "Pain reliever" },
                //     new Medicine { Name = "Ibuprofen", Description = "Anti-inflammatory" }
                // );
            }
        }

        private static void SeedAllergiesIfEmpty(DatabaseContext context)
        {
            if (!context.Allergies.Any())
            {
                // TODO: Add allergies seed data
                // Example:
                // context.Allergies.AddRange(
                //     new Allergy { Name = "Penicillin", Description = "Antibiotic allergy" },
                //     new Allergy { Name = "Peanuts", Description = "Food allergy" }
                // );
            }
        }
    }
}