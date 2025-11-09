using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace MedManager.Infrastructure.Database.SeedData.Seeds
{
    public static class AdminSeed
    {
        public static async Task SeedAsync(
            DatabaseContext context, 
            UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@medmanager.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser != null)
            {
                return; // Admin existe déjà
            }

            // Créer l'utilisateur admin
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!result.Succeeded)
            {
                throw new Exception($"Erreur lors de la création de l'admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");

            // Créer l'entité Admin
            var admin = new Admin
            {
                FirstName = "Admin",
                LastName = "System",
                ApplicationUserId = adminUser.Id,
                User = adminUser
            };

            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
