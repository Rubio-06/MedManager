using MedManager.Domain.Models;
using MedManager.Domain.Models.Tables;
using MedManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Infrastructure.Database.SeedData.Seeds
{
    public static class MedicineSeed
    {
        public static async Task SeedAsync(DatabaseContext context)
        {
            var medicinesData = new List<(string Name, string Description, List<(string Name, string Dosage)> Components)>
            {
                ("Amoxicilline", 
                 "Antibiotique à large spectre de la famille des pénicillines", 
                 new List<(string, string)> 
                 { 
                     ("Amoxicilline trihydratée", "500mg"), 
                     ("Pénicilline", "dérivé") 
                 }),

                ("Doliprane 1000", 
                 "Antalgique et antipyrétique à base de paracétamol", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "1000mg") 
                 }),

                ("Ibuprofène 400", 
                 "Anti-inflammatoire non stéroïdien (AINS)", 
                 new List<(string, string)> 
                 { 
                     ("Ibuprofène", "400mg") 
                 }),

                ("Aspirine 500", 
                 "Antalgique, antipyrétique et anti-inflammatoire", 
                 new List<(string, string)> 
                 { 
                     ("Acide acétylsalicylique", "500mg"), 
                     ("Aspirine", "principe actif") 
                 }),

                ("Codoliprane", 
                 "Antalgique associant paracétamol et codéine", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "500mg"), 
                     ("Codéine", "30mg") 
                 }),

                ("Augmentin", 
                 "Antibiotique associant amoxicilline et acide clavulanique", 
                 new List<(string, string)> 
                 { 
                     ("Amoxicilline", "500mg"), 
                     ("Acide clavulanique", "125mg"), 
                     ("Pénicilline", "dérivé") 
                 }),

                ("Efferalgan", 
                 "Antalgique à base de paracétamol", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "500mg") 
                 }),

                ("Bactrim", 
                 "Antibiotique de la famille des sulfamides", 
                 new List<(string, string)> 
                 { 
                     ("Sulfaméthoxazole", "400mg"), 
                     ("Triméthoprime", "80mg"), 
                     ("Sulfamides", "famille") 
                 }),

                ("Dafalgan", 
                 "Antalgique à base de paracétamol", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "1000mg") 
                 }),

                ("Nurofen", 
                 "Anti-inflammatoire non stéroïdien", 
                 new List<(string, string)> 
                 { 
                     ("Ibuprofène", "200mg") 
                 }),

                ("Lamaline", 
                 "Antalgique d'action centrale", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "300mg"), 
                     ("Opium", "10mg"), 
                     ("Caféine", "50mg"), 
                     ("Codéine", "trace") 
                 }),

                ("Aspirine protect 100", 
                 "Antiagrégant plaquettaire", 
                 new List<(string, string)> 
                 { 
                     ("Acide acétylsalicylique", "100mg"), 
                     ("Aspirine", "faible dose") 
                 }),

                ("Claradol", 
                 "Antalgique et antipyrétique", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "500mg") 
                 }),

                ("Advil", 
                 "Anti-inflammatoire non stéroïdien", 
                 new List<(string, string)> 
                 { 
                     ("Ibuprofène", "400mg") 
                 }),

                ("Doliprane lactose", 
                 "Antalgique avec lactose", 
                 new List<(string, string)> 
                 { 
                     ("Paracétamol", "500mg"), 
                     ("Lactose", "excipient") 
                 })
            };

            foreach (var (name, description, components) in medicinesData)
            {
                // Vérifier si le médicament existe déjà
                if (await context.Medicines.AnyAsync(m => m.Name == name))
                {
                    continue; // Passer au suivant
                }

                var medicine = new Medicine
                {
                    Name = name,
                    Description = description
                };
                
                await context.Medicines.AddAsync(medicine);
                await context.SaveChangesAsync(); // Sauvegarder pour obtenir l'Id

                // Ajouter les composants
                foreach (var (compName, compDosage) in components)
                {
                    var component = new MedicineComponent
                    {
                        Name = compName,
                        Dosage = compDosage,
                        MedicineId = medicine.Id
                    };
                    await context.MedicineComponents.AddAsync(component);
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedMedicineAllergiesAsync(DatabaseContext context)
        {
            if (await context.MedicineAllergies.AnyAsync())
            {
                return; // Déjà des associations en base
            }

            // Associations médicament-allergie
            var associations = new Dictionary<string, string[]>
            {
                { "Amoxicilline", new[] { "Pénicilline" } },
                { "Augmentin", new[] { "Pénicilline" } },
                { "Ibuprofène 400", new[] { "Ibuprofène" } },
                { "Nurofen", new[] { "Ibuprofène" } },
                { "Advil", new[] { "Ibuprofène" } },
                { "Aspirine 500", new[] { "Aspirine" } },
                { "Aspirine protect 100", new[] { "Aspirine" } },
                { "Codoliprane", new[] { "Codéine" } },
                { "Lamaline", new[] { "Codéine" } },
                { "Bactrim", new[] { "Sulfamides" } },
                { "Doliprane lactose", new[] { "Lactose" } }
            };

            foreach (var (medicineName, allergyNames) in associations)
            {
                var medicine = await context.Medicines.FirstOrDefaultAsync(m => m.Name == medicineName);
                if (medicine != null)
                {
                    foreach (var allergyName in allergyNames)
                    {
                        var allergy = await context.Allergies.FirstOrDefaultAsync(a => a.Name == allergyName);
                        if (allergy != null)
                        {
                            await context.MedicineAllergies.AddAsync(new MedicineAllergy
                            {
                                MedicineId = medicine.Id,
                                AllergyId = allergy.Id
                            });
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
