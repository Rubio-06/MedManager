using MedManager.Application.DTOs;
using MedManager.Application.Interfaces;
using MedManager.Domain.Models;
using MedManager.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedManager.Application.Services
{
    public class MedicineManagementService : IMedicineManagementService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<MedicineManagementService> _logger;

        public MedicineManagementService(
            DatabaseContext context,
            ILogger<MedicineManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<MedicineDto>> GetAllMedicinesAsync()
        {
            var medicines = await _context.Medicines
                .Include(m => m.PrescriptionMedicines)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Dosage = m.Dosage,
                SideEffects = m.SideEffects,
                PrescriptionCount = m.PrescriptionMedicines.Count
            }).ToList();
        }

        public async Task<MedicineDto?> GetMedicineByIdAsync(int id)
        {
            var medicine = await _context.Medicines
                .Include(m => m.PrescriptionMedicines)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null)
                return null;

            return new MedicineDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Description = medicine.Description,
                Dosage = medicine.Dosage,
                SideEffects = medicine.SideEffects,
                PrescriptionCount = medicine.PrescriptionMedicines.Count
            };
        }

        public async Task<UpdateMedicineDto?> GetMedicineForEditAsync(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);

            if (medicine == null)
                return null;

            return new UpdateMedicineDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Description = medicine.Description,
                Dosage = medicine.Dosage,
                SideEffects = medicine.SideEffects
            };
        }

        public async Task<(bool Success, string[] Errors)> CreateMedicineAsync(CreateMedicineDto dto)
        {
            // Vérifier si un médicament avec ce nom existe déjà
            var existingMedicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Name.ToLower() == dto.Name.ToLower());

            if (existingMedicine != null)
            {
                return (false, new[] { "Un médicament avec ce nom existe déjà." });
            }

            var medicine = new Medicine
            {
                Name = dto.Name,
                Description = dto.Description,
                Dosage = dto.Dosage,
                SideEffects = dto.SideEffects
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Medicine created: {Name}", dto.Name);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> UpdateMedicineAsync(UpdateMedicineDto dto)
        {
            var medicine = await _context.Medicines.FindAsync(dto.Id);

            if (medicine == null)
            {
                return (false, new[] { "Médicament introuvable." });
            }

            // Vérifier si un autre médicament avec ce nom existe déjà
            var existingMedicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Name.ToLower() == dto.Name.ToLower() && m.Id != dto.Id);

            if (existingMedicine != null)
            {
                return (false, new[] { "Un autre médicament avec ce nom existe déjà." });
            }

            medicine.Name = dto.Name;
            medicine.Description = dto.Description;
            medicine.Dosage = dto.Dosage;
            medicine.SideEffects = dto.SideEffects;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Medicine updated: {Name}", dto.Name);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string Error)> DeleteMedicineAsync(int id)
        {
            var medicine = await _context.Medicines
                .Include(m => m.PrescriptionMedicines)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null)
            {
                return (false, "Médicament introuvable.");
            }

            // Vérifier s'il y a des prescriptions associées
            if (medicine.PrescriptionMedicines.Any())
            {
                return (false, $"Impossible de supprimer ce médicament car il est utilisé dans {medicine.PrescriptionMedicines.Count} prescription(s).");
            }

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Medicine deleted: {Name}", medicine.Name);
            return (true, string.Empty);
        }
    }
}
