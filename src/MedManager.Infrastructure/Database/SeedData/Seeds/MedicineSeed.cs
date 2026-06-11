using MedManager.Domain.Models;
using MedManager.Domain.Models.Tables;
using MedManager.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace MedManager.Infrastructure.Database.SeedData.Seeds
{
    public static class MedicineSeed
    {
        public static Task SeedAsync(DatabaseContext context)
        {
            return SeedInitialAsync(context);
        }

        public static async Task SeedInitialAsync(DatabaseContext context)
        {
            await SeedCatalogAsync(context, treatExistingDataAsUpdate: false);
        }

        public static async Task SeedUpdateAsync(DatabaseContext context)
        {
            await SeedCatalogAsync(context, treatExistingDataAsUpdate: true);
        }

        private static async Task SeedCatalogAsync(DatabaseContext context, bool treatExistingDataAsUpdate)
        {
            var medicinesData = new List<MedicineDefinition>
            {
                new("Amoxicilline", "Antibiotique à large spectre de la famille des pénicillines",
                    [
                        ("Amoxicilline trihydratée", "500mg"),
                        ("Pénicilline", "dérivé")
                    ],
                    [
                        ("Amoxicilline", "Substance active antibiotique"),
                        ("Pénicilline", "Famille antibiotique")
                    ]),

                new("Doliprane 1000", "Antalgique et antipyrétique à base de paracétamol",
                    [
                        ("Paracétamol", "1000mg")
                    ],
                    [
                        ("Paracétamol", "Antalgique et antipyrétique")
                    ]),

                new("Ibuprofène 400", "Anti-inflammatoire non stéroïdien (AINS)",
                    [
                        ("Ibuprofène", "400mg")
                    ],
                    [
                        ("Ibuprofène", "Anti-inflammatoire non stéroïdien")
                    ]),

                new("Aspirine 500", "Antalgique, antipyrétique et anti-inflammatoire",
                    [
                        ("Acide acétylsalicylique", "500mg"),
                        ("Aspirine", "principe actif")
                    ],
                    [
                        ("Acide acétylsalicylique", "Principe actif de l'aspirine")
                    ]),

                new("Codoliprane", "Antalgique associant paracétamol et codéine",
                    [
                        ("Paracétamol", "500mg"),
                        ("Codéine", "30mg")
                    ],
                    [
                        ("Paracétamol", "Antalgique courant"),
                        ("Codéine", "Antalgique opioïde")
                    ]),

                new("Augmentin", "Antibiotique associant amoxicilline et acide clavulanique",
                    [
                        ("Amoxicilline", "500mg"),
                        ("Acide clavulanique", "125mg"),
                        ("Pénicilline", "dérivé")
                    ],
                    [
                        ("Amoxicilline", "Substance active antibiotique"),
                        ("Acide clavulanique", "Inhibiteur de bêta-lactamase")
                    ]),

                new("Efferalgan", "Antalgique à base de paracétamol",
                    [
                        ("Paracétamol", "500mg")
                    ],
                    [
                        ("Paracétamol", "Antalgique et antipyrétique")
                    ]),

                new("Bactrim", "Antibiotique de la famille des sulfamides",
                    [
                        ("Sulfaméthoxazole", "400mg"),
                        ("Triméthoprime", "80mg"),
                        ("Sulfamides", "famille")
                    ],
                    [
                        ("Sulfaméthoxazole", "Antibiotique sulfamide"),
                        ("Triméthoprime", "Antibiotique synergique")
                    ]),

                new("Dafalgan", "Antalgique à base de paracétamol",
                    [
                        ("Paracétamol", "1000mg")
                    ],
                    [
                        ("Paracétamol", "Antalgique et antipyrétique")
                    ]),

                new("Nurofen", "Anti-inflammatoire non stéroïdien",
                    [
                        ("Ibuprofène", "200mg")
                    ],
                    [
                        ("Ibuprofène", "Anti-inflammatoire non stéroïdien")
                    ]),

                new("Lamaline", "Antalgique d'action centrale",
                    [
                        ("Paracétamol", "300mg"),
                        ("Opium", "10mg"),
                        ("Caféine", "50mg"),
                        ("Codéine", "trace")
                    ],
                    [
                        ("Paracétamol", "Antalgique courant"),
                        ("Opium", "Analgésique opiacé"),
                        ("Caféine", "Adjuvant"),
                        ("Codéine", "Antalgique opioïde")
                    ]),

                new("Aspirine protect 100", "Antiagrégant plaquettaire",
                    [
                        ("Acide acétylsalicylique", "100mg"),
                        ("Aspirine", "faible dose")
                    ],
                    [
                        ("Acide acétylsalicylique", "Principe actif de l'aspirine")
                    ]),

                new("Claradol", "Antalgique et antipyrétique",
                    [
                        ("Paracétamol", "500mg")
                    ],
                    [
                        ("Paracétamol", "Antalgique et antipyrétique")
                    ]),

                new("Advil", "Anti-inflammatoire non stéroïdien",
                    [
                        ("Ibuprofène", "400mg")
                    ],
                    [
                        ("Ibuprofène", "Anti-inflammatoire non stéroïdien")
                    ]),

                new("Doliprane lactose", "Antalgique avec lactose",
                    [
                        ("Paracétamol", "500mg"),
                        ("Lactose", "excipient")
                    ],
                    [
                        ("Paracétamol", "Antalgique et antipyrétique"),
                        ("Lactose", "Excipient")
                    ])
            };

            if (!treatExistingDataAsUpdate && await context.Medicines.AnyAsync())
            {
                return;
            }

            await SeedMoleculesAsync(context, medicinesData);

            foreach (var medicineDefinition in medicinesData)
            {
                var medicine = await context.Medicines
                    .Include(m => m.Components)
                    .Include(m => m.MedicineMolecules)
                    .FirstOrDefaultAsync(m => m.Name == medicineDefinition.Name);

                if (medicine == null)
                {
                    medicine = new Medicine
                    {
                        Name = medicineDefinition.Name,
                        Description = medicineDefinition.Description
                    };

                    await context.Medicines.AddAsync(medicine);
                    await context.SaveChangesAsync();
                }
                else
                {
                    medicine.Description = medicineDefinition.Description;
                }

                if (medicine.Components.Any())
                {
                    context.MedicineComponents.RemoveRange(medicine.Components);
                }

                if (medicine.MedicineMolecules.Any())
                {
                    context.MedicineMolecules.RemoveRange(medicine.MedicineMolecules);
                }

                await context.SaveChangesAsync();

                foreach (var (componentName, componentDosage) in medicineDefinition.Components)
                {
                    await context.MedicineComponents.AddAsync(new MedicineComponent
                    {
                        Name = componentName,
                        Dosage = componentDosage,
                        MedicineId = medicine.Id
                    });
                }

                foreach (var moleculeName in medicineDefinition.Molecules.Select(m => m.Name).Distinct())
                {
                    var molecule = await context.Molecules.FirstAsync(m => m.Name == moleculeName);
                    await context.MedicineMolecules.AddAsync(new MedicineMolecule
                    {
                        MedicineId = medicine.Id,
                        MoleculeId = molecule.Id
                    });
                }

                await context.SaveChangesAsync();
            }
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

        private static async Task SeedMoleculesAsync(DatabaseContext context, IReadOnlyCollection<MedicineDefinition> medicinesData)
        {
            var moleculeDefinitions = medicinesData
                .SelectMany(m => m.Molecules)
                .GroupBy(m => m.Name)
                .Select(g => g.First())
                .ToList();

            foreach (var (name, description) in moleculeDefinitions)
            {
                var molecule = await context.Molecules.FirstOrDefaultAsync(m => m.Name == name);
                if (molecule == null)
                {
                    await context.Molecules.AddAsync(new Molecule
                    {
                        Name = name,
                        Description = description
                    });
                }
                else
                {
                    molecule.Description = description;
                }
            }

            await context.SaveChangesAsync();
        }

        private sealed record MedicineDefinition(
            string Name,
            string Description,
            IReadOnlyList<(string Name, string Dosage)> Components,
            IReadOnlyList<(string Name, string? Description)> Molecules);
    }
}
