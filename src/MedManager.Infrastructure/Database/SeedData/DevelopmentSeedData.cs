using MedManager.Infrastructure.Context;
using MedManager.Infrastructure.Database.SeedData.Seeds;
using Microsoft.AspNetCore.Identity;
using MedManager.Domain.Models.Users;

namespace MedManager.Infrastructure.Database.SeedData
{
    public static class DevelopmentSeedData
    {
        public static async Task SeedAsync(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            try
            {
                Console.WriteLine("========================================");
                Console.WriteLine("🌱 DÉBUT DU SEED DE DÉVELOPPEMENT");
                Console.WriteLine("========================================");

                // Ordre d'exécution important !
                // 1. Allergies (indépendantes)
                Console.WriteLine("📋 1/6 - Seed des allergies...");
                await AllergySeed.SeedAsync(context);
                Console.WriteLine("✓ Allergies créées");
                
                // 2. Médicaments avec leurs composants (indépendants)
                Console.WriteLine("💊 2/6 - Seed des médicaments...");
                await MedicineSeed.SeedAsync(context);
                Console.WriteLine("✓ Médicaments créés");
                
                // 3. Associations médicament-allergie (dépend de 1 et 2)
                Console.WriteLine("🔗 3/6 - Seed des associations médicament-allergie...");
                await MedicineSeed.SeedMedicineAllergiesAsync(context);
                Console.WriteLine("✓ Associations créées");
                
                // 4. Admin (indépendant)
                Console.WriteLine("👑 4/6 - Seed de l'admin...");
                await AdminSeed.SeedAsync(context, userManager);
                Console.WriteLine("✓ Admin créé");
                
                // 5. Docteurs (indépendants)
                Console.WriteLine("👨‍⚕️ 5/6 - Seed des docteurs...");
                await DoctorSeed.SeedAsync(context, userManager);
                Console.WriteLine("✓ Docteurs créés");
                
                // 6. Patients (dépend des docteurs et allergies)
                Console.WriteLine("👥 6/6 - Seed des patients...");
                await PatientSeed.SeedAsync(context, userManager);
                Console.WriteLine("✓ Patients créés");

                await context.SaveChangesAsync();

                Console.WriteLine("========================================");
                Console.WriteLine("✅ SEED TERMINÉ AVEC SUCCÈS");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("❌ ERREUR LORS DU SEED");
                Console.WriteLine("========================================");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                Console.WriteLine("========================================");
                
                // NE PAS throw pour ne pas bloquer le démarrage
                // L'utilisateur verra l'erreur dans les logs
            }
        }
    }
}
