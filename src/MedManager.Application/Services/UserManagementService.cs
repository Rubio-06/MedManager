using MedManager.Application.DTOs;
using MedManager.Application.Interfaces;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedManager.Application.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserManagementService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var persons = await _context.Persons
                .Include(p => p.User)
                .ToListAsync();

            var userList = new List<UserDto>();

            foreach (var person in persons)
            {
                var roles = await _userManager.GetRolesAsync(person.User);
                var role = roles.FirstOrDefault() ?? "Unknown";

                var userDto = new UserDto
                {
                    PersonId = person.Id,
                    UserId = person.ApplicationUserId,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Email = person.User.Email ?? "",
                    Role = role,
                    EmailConfirmed = person.User.EmailConfirmed
                };

                // Charger les infos spécifiques selon le type
                if (person is Patient patient)
                {
                    await _context.Entry(patient).Reference(p => p.Doctor).LoadAsync();
                    userDto.DateBirthday = patient.DateBirthday;
                    userDto.DoctorName = patient.Doctor != null
                        ? $"Dr {patient.Doctor.FirstName} {patient.Doctor.LastName}"
                        : null;
                }
                else if (person is Doctor doctor)
                {
                    await _context.Entry(doctor).Collection(d => d.Patients).LoadAsync();
                    userDto.PatientCount = doctor.Patients.Count;
                }

                userList.Add(userDto);
            }

            return userList.OrderBy(u => u.Role).ThenBy(u => u.LastName).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(int personId)
        {
            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == personId);

            if (person == null)
                return null;

            var roles = await _userManager.GetRolesAsync(person.User);
            var role = roles.FirstOrDefault() ?? "Unknown";

            var userDto = new UserDto
            {
                PersonId = person.Id,
                UserId = person.ApplicationUserId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.User.Email ?? "",
                Role = role,
                EmailConfirmed = person.User.EmailConfirmed
            };

            if (person is Patient patient)
            {
                await _context.Entry(patient).Reference(p => p.Doctor).LoadAsync();
                userDto.DateBirthday = patient.DateBirthday;
                userDto.DoctorName = patient.Doctor != null
                    ? $"Dr {patient.Doctor.FirstName} {patient.Doctor.LastName}"
                    : null;
            }
            else if (person is Doctor doctor)
            {
                await _context.Entry(doctor).Collection(d => d.Patients).LoadAsync();
                userDto.PatientCount = doctor.Patients.Count;
            }

            return userDto;
        }

        public async Task<(bool Success, string[] Errors)> CreateUserAsync(CreateUserDto dto)
        {
            // Vérifier si l'email existe déjà
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return (false, new[] { "Un utilisateur avec cet email existe déjà." });
            }

            // Créer l'utilisateur Identity
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

            // Ajouter le rôle
            await _userManager.AddToRoleAsync(user, dto.Role);

            // Créer la personne selon le rôle
            Person person = dto.Role switch
            {
                "Admin" => new Admin
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    ApplicationUserId = user.Id,
                    User = user
                },
                "Doctor" => new Doctor
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    ApplicationUserId = user.Id,
                    User = user
                },
                "Patient" => new Patient
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = dto.Gender ?? Gender.Male,
                    DateBirthday = dto.DateBirthday ?? DateTime.Now.AddYears(-30),
                    SocialSecurityNumber = dto.SocialSecurityNumber ?? GenerateSSN(),
                    DoctorId = dto.DoctorId,
                    ApplicationUserId = user.Id,
                    User = user
                },
                _ => throw new InvalidOperationException("Rôle invalide")
            };

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created: {Email} with role {Role}", dto.Email, dto.Role);
            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string[] Errors)> UpdateUserAsync(UpdateUserDto dto)
        {
            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == dto.PersonId);

            if (person == null)
            {
                return (false, new[] { "Utilisateur introuvable." });
            }

            // Vérifier si c'est un admin - protection contre la modification du rôle admin
            var currentRoles = await _userManager.GetRolesAsync(person.User);
            var isCurrentlyAdmin = currentRoles.Contains("Admin");

            if (isCurrentlyAdmin && dto.Role != "Admin")
            {
                return (false, new[] { "Impossible de retirer le rôle administrateur. Cette action est protégée pour la sécurité du système." });
            }

            // Mettre à jour les infos de base
            person.FirstName = dto.FirstName;
            person.LastName = dto.LastName;

            // Mettre à jour l'email si changé
            if (person.User.Email != dto.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != person.ApplicationUserId);
                if (emailExists)
                {
                    return (false, new[] { "Cet email est déjà utilisé." });
                }

                person.User.Email = dto.Email;
                person.User.UserName = dto.Email;
                person.User.NormalizedEmail = dto.Email.ToUpper();
                person.User.NormalizedUserName = dto.Email.ToUpper();
            }

            // Mettre à jour le mot de passe si fourni
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(person.User);
                var result = await _userManager.ResetPasswordAsync(person.User, token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            // Mettre à jour le rôle si changé (déjà vérifié que ce n'est pas un admin qui perd son rôle)
            if (!currentRoles.Contains(dto.Role))
            {
                await _userManager.RemoveFromRolesAsync(person.User, currentRoles);
                await _userManager.AddToRoleAsync(person.User, dto.Role);
            }

            // Mettre à jour les champs spécifiques pour Patient
            if (person is Patient patient && dto.Role == "Patient")
            {
                patient.Gender = dto.Gender ?? Gender.Male;
                patient.DateBirthday = dto.DateBirthday ?? patient.DateBirthday;
                patient.SocialSecurityNumber = dto.SocialSecurityNumber ?? patient.SocialSecurityNumber;
                patient.DoctorId = dto.DoctorId;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User updated: {Email}", dto.Email);
            return (true, Array.Empty<string>());
        }

        public async Task<UpdateUserDto?> GetUserForEditAsync(int personId)
        {
            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == personId);

            if (person == null)
                return null;

            var roles = await _userManager.GetRolesAsync(person.User);
            var role = roles.FirstOrDefault() ?? "Unknown";

            var dto = new UpdateUserDto
            {
                PersonId = person.Id,
                UserId = person.ApplicationUserId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.User.Email ?? "",
                Role = role
            };

            if (person is Patient patient)
            {
                dto.Gender = patient.Gender;
                dto.DateBirthday = patient.DateBirthday;
                dto.SocialSecurityNumber = patient.SocialSecurityNumber;
                dto.DoctorId = patient.DoctorId;
            }

            return dto;
        }

        public async Task<(bool Success, string Error)> DeleteUserAsync(int personId)
        {
            var person = await _context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == personId);

            if (person == null)
            {
                return (false, "Utilisateur introuvable.");
            }

            // Vérifier si c'est un admin - protection contre la suppression d'admin
            var roles = await _userManager.GetRolesAsync(person.User);
            if (roles.Contains("Admin"))
            {
                return (false, "Impossible de supprimer un compte administrateur. Cette action est protégée pour la sécurité du système.");
            }

            // Supprimer la personne (cascade sur User via OnDelete)
            _context.Persons.Remove(person);

            // Supprimer l'utilisateur Identity
            await _userManager.DeleteAsync(person.User);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User deleted: {Email}", person.User.Email);
            return (true, string.Empty);
        }
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .OrderBy(d => d.LastName)
                .ToListAsync();
        }

        private string GenerateSSN()
        {
            // Générer un numéro de sécurité sociale fictif
            var random = new Random();
            return string.Concat(Enumerable.Range(0, 15).Select(_ => random.Next(0, 10)));
        }
    }
}
