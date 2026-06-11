using MedManager.Application.DTOs;
using MedManager.Application.Interfaces;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedManager.Application.Services
{
    public class PatientManagementService : IPatientManagementService
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PatientManagementService> _logger;

        public PatientManagementService(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PatientManagementService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<PatientDto>> GetPatientsByDoctorIdAsync(int doctorId)
        {
            var patients = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(p => p.Prescriptions)
                .Include(p => p.PatientAllergies)
                .Where(p => p.DoctorId == doctorId)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            return patients.Select(p => new PatientDto
            {
                Id = p.Id,
                UserId = p.ApplicationUserId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.User.Email ?? string.Empty,
                DateBirthday = p.DateBirthday,
                Gender = p.Gender,
                SocialSecurityNumber = p.SocialSecurityNumber,
                EmailConfirmed = p.User.EmailConfirmed,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor != null
                    ? $"Dr. {p.Doctor.FirstName} {p.Doctor.LastName}"
                    : null,
                PrescriptionCount = p.Prescriptions.Count,
                AllergyCount = p.PatientAllergies.Count
            }).ToList();
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int patientId, int doctorId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(p => p.Prescriptions)
                .Include(p => p.PatientAllergies)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId);

            if (patient == null)
                return null;

            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.ApplicationUserId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.User.Email ?? string.Empty,
                DateBirthday = patient.DateBirthday,
                Gender = patient.Gender,
                SocialSecurityNumber = patient.SocialSecurityNumber,
                EmailConfirmed = patient.User.EmailConfirmed,
                DoctorId = patient.DoctorId,
                DoctorName = patient.Doctor != null
                    ? $"Dr. {patient.Doctor.FirstName} {patient.Doctor.LastName}"
                    : null,
                PrescriptionCount = patient.Prescriptions.Count,
                AllergyCount = patient.PatientAllergies.Count
            };
        }

        public async Task<UpdatePatientDto?> GetPatientForEditAsync(int patientId, int doctorId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId);

            if (patient == null)
                return null;

            return new UpdatePatientDto
            {
                Id = patient.Id,
                UserId = patient.ApplicationUserId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.User.Email ?? string.Empty,
                DateBirthday = patient.DateBirthday,
                Gender = patient.Gender,
                SocialSecurityNumber = patient.SocialSecurityNumber,
                EmailConfirmed = patient.User.EmailConfirmed
            };
        }

        public async Task<(bool Success, string[] Errors)> CreatePatientAsync(CreatePatientDto dto)
        {
            // Vérifier si le médecin existe
            var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
            if (doctor == null)
            {
                return (false, new[] { "Médecin introuvable." });
            }

            // Vérifier si l'email existe déjà
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return (false, new[] { "Un utilisateur avec cet email existe déjà." });
            }

            var socialSecurityNumberExists = await _context.Patients
                .AnyAsync(p => p.SocialSecurityNumber == dto.SocialSecurityNumber);

            if (socialSecurityNumberExists)
            {
                return (false, new[] { "Ce numéro de sécurité sociale existe déjà." });
            }

            // Créer l'utilisateur
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            // Assigner le rôle Patient
            await _userManager.AddToRoleAsync(user, "Patient");

            // Créer le Patient
            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ApplicationUserId = user.Id,
                DateBirthday = dto.DateBirthday,
                Gender = dto.Gender,
                SocialSecurityNumber = dto.SocialSecurityNumber,
                DoctorId = dto.DoctorId
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient created: {Email} by Doctor {DoctorId}", dto.Email, dto.DoctorId);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> UpdatePatientAsync(UpdatePatientDto dto, int doctorId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == dto.Id && p.DoctorId == doctorId);

            if (patient == null)
            {
                return (false, new[] { "Patient introuvable ou vous n'avez pas accès à ce patient." });
            }

            // Vérifier si l'email est déjà utilisé par un autre utilisateur
            if (patient.User.Email != dto.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null && existingUser.Id != patient.ApplicationUserId)
                {
                    return (false, new[] { "Cet email est déjà utilisé par un autre utilisateur." });
                }
            }

            var socialSecurityNumberExists = await _context.Patients
                .AnyAsync(p => p.SocialSecurityNumber == dto.SocialSecurityNumber && p.Id != dto.Id);

            if (socialSecurityNumberExists)
            {
                return (false, new[] { "Ce numéro de sécurité sociale existe déjà." });
            }

            // Mettre à jour l'utilisateur
            patient.User.Email = dto.Email;
            patient.User.UserName = dto.Email;
            patient.User.EmailConfirmed = dto.EmailConfirmed;

            // Mettre à jour le patient
            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.DateBirthday = dto.DateBirthday;
            patient.Gender = dto.Gender;
            patient.SocialSecurityNumber = dto.SocialSecurityNumber;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient updated: {Email}", dto.Email);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> DeletePatientAsync(int patientId, int doctorId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Prescriptions)
                .Include(p => p.PatientAllergies)
                .Include(p => p.PatientHistories)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId);

            if (patient == null)
            {
                return (false, new[] { "Patient introuvable ou vous n'avez pas accès à ce patient." });
            }

            // Vérifier s'il a des prescriptions
            if (patient.Prescriptions.Any())
            {
                return (false, new[] { "Impossible de supprimer ce patient car il possède des prescriptions." });
            }

            // Supprimer les relations
            _context.PatientAllergies.RemoveRange(patient.PatientAllergies);
            _context.PatientHistories.RemoveRange(patient.PatientHistories);

            // Supprimer le patient
            _context.Patients.Remove(patient);

            // Supprimer l'utilisateur
            var user = patient.User;
            await _userManager.DeleteAsync(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient deleted: {Email}", patient.User.Email);
            return (true, Array.Empty<string>());
        }
    }
}


