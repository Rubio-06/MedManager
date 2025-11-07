using MedManager.Application.DTOs;
using MedManager.Application.Interfaces;
using MedManager.Domain.Models;
using MedManager.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedManager.Application.Services
{
    public class AllergyManagementService : IAllergyManagementService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<AllergyManagementService> _logger;

        public AllergyManagementService(
            DatabaseContext context,
            ILogger<AllergyManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AllergyDto>> GetAllAllergiesAsync()
        {
            var allergies = await _context.Allergies
                .Include(a => a.PatientAllergies)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return allergies.Select(a => new AllergyDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                PatientCount = a.PatientAllergies.Count
            }).ToList();
        }

        public async Task<AllergyDto?> GetAllergyByIdAsync(int id)
        {
            var allergy = await _context.Allergies
                .Include(a => a.PatientAllergies)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (allergy == null)
                return null;

            return new AllergyDto
            {
                Id = allergy.Id,
                Name = allergy.Name,
                Description = allergy.Description,
                PatientCount = allergy.PatientAllergies.Count
            };
        }

        public async Task<UpdateAllergyDto?> GetAllergyForEditAsync(int id)
        {
            var allergy = await _context.Allergies.FindAsync(id);

            if (allergy == null)
                return null;

            return new UpdateAllergyDto
            {
                Id = allergy.Id,
                Name = allergy.Name,
                Description = allergy.Description
            };
        }

        public async Task<(bool Success, string[] Errors)> CreateAllergyAsync(CreateAllergyDto dto)
        {
            // Vérifier si une allergie avec ce nom existe déjà
            var existingAllergy = await _context.Allergies
                .FirstOrDefaultAsync(a => a.Name.ToLower() == dto.Name.ToLower());

            if (existingAllergy != null)
            {
                return (false, new[] { "Une allergie avec ce nom existe déjà." });
            }

            var allergy = new Allergy
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Allergy created: {Name}", dto.Name);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> UpdateAllergyAsync(UpdateAllergyDto dto)
        {
            var allergy = await _context.Allergies.FindAsync(dto.Id);

            if (allergy == null)
            {
                return (false, new[] { "Allergie introuvable." });
            }

            // Vérifier si une autre allergie avec ce nom existe déjà
            var existingAllergy = await _context.Allergies
                .FirstOrDefaultAsync(a => a.Name.ToLower() == dto.Name.ToLower() && a.Id != dto.Id);

            if (existingAllergy != null)
            {
                return (false, new[] { "Une autre allergie avec ce nom existe déjà." });
            }

            allergy.Name = dto.Name;
            allergy.Description = dto.Description;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Allergy updated: {Name}", dto.Name);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> DeleteAllergyAsync(int id)
        {
            var allergy = await _context.Allergies
                .Include(a => a.PatientAllergies)
                .Include(a => a.MedicineAllergies)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (allergy == null)
            {
                return (false, new[] { "Allergie introuvable." });
            }

            // Vérifier si l'allergie est utilisée
            if (allergy.PatientAllergies.Any() || allergy.MedicineAllergies.Any())
            {
                return (false, new[] { "Impossible de supprimer cette allergie car elle est utilisée par des patients ou des médicaments." });
            }

            _context.Allergies.Remove(allergy);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Allergy deleted: {Name}", allergy.Name);
            return (true, Array.Empty<string>());
        }
    }
}
