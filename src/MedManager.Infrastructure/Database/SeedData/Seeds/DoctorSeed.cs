using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Infrastructure.Database.SeedData.Seeds
{
    public static class DoctorSeed
    {
        public static async Task SeedAsync(
            DatabaseContext context, 
            UserManager<ApplicationUser> userManager)
        {
            var doctors = new[]
            {
                new { Email = "jean.dupont@medmanager.com", FirstName = "Jean", LastName = "Dupont" },
                new { Email = "sophie.martin@medmanager.com", FirstName = "Sophie", LastName = "Martin" }
            };

            foreach (var doctorData in doctors)
            {
                var existingUser = await userManager.FindByEmailAsync(doctorData.Email);
                if (existingUser != null)
                {
                    continue; // Ce docteur existe déjà
                }

                // Créer l'utilisateur
                var doctorUser = new ApplicationUser
                {
                    UserName = doctorData.Email,
                    Email = doctorData.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(doctorUser, "Doctor123!");
                if (!result.Succeeded)
                {
                    throw new Exception($"Erreur lors de la création du docteur {doctorData.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await userManager.AddToRoleAsync(doctorUser, "Doctor");

                // Créer l'entité Doctor
                var doctor = new Doctor
                {
                    FirstName = doctorData.FirstName,
                    LastName = doctorData.LastName,
                    ApplicationUserId = doctorUser.Id,
                    User = doctorUser
                };

                await context.Doctors.AddAsync(doctor);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<List<Doctor>> GetAllDoctorsAsync(DatabaseContext context)
        {
            return await context.Doctors.ToListAsync();
        }
    }
}
