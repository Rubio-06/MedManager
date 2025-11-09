using MedManager.Domain.Models;
using MedManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Infrastructure.Database.SeedData.Seeds
{
    public static class AllergySeed
    {
        public static async Task SeedAsync(DatabaseContext context)
        {
            if (await context.Allergies.AnyAsync())
            {
                return; // Déjà des allergies en base
            }

            var allergies = new List<Allergy>
            {
                new Allergy
                {
                    Name = "Pénicilline",
                    Description = "Allergie aux antibiotiques de type pénicilline (Amoxicilline, Augmentin, etc.)"
                },
                new Allergy
                {
                    Name = "Aspirine",
                    Description = "Allergie à l'acide acétylsalicylique et dérivés"
                },
                new Allergy
                {
                    Name = "Ibuprofène",
                    Description = "Allergie aux anti-inflammatoires non stéroïdiens (AINS)"
                },
                new Allergy
                {
                    Name = "Codéine",
                    Description = "Allergie aux opiacés et dérivés de la codéine"
                },
                new Allergy
                {
                    Name = "Sulfamides",
                    Description = "Allergie aux antibiotiques de la famille des sulfamides"
                },
                new Allergy
                {
                    Name = "Lactose",
                    Description = "Intolérance au lactose présent comme excipient"
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
                }
            };

            await context.Allergies.AddRangeAsync(allergies);
            await context.SaveChangesAsync();
        }
    }
}
