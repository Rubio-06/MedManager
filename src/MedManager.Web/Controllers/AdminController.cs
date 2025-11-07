using MedManager.Application.DTOs;
using MedManager.Application.Interfaces;
using MedManager.Infrastructure.Context;
using MedManager.Web.Helpers;
using MedManager.Web.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class AdminController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IUserManagementService _userManagementService;
        private readonly IMedicineManagementService _medicineManagementService;
        private readonly IAllergyManagementService _allergyManagementService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            DatabaseContext context,
            IUserManagementService userManagementService,
            IMedicineManagementService medicineManagementService,
            IAllergyManagementService allergyManagementService,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManagementService = userManagementService;
            _medicineManagementService = medicineManagementService;
            _allergyManagementService = allergyManagementService;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalPatients = await _context.Patients.CountAsync(),
                TotalMedicines = await _context.Medicines.CountAsync(),
                TotalPrescriptions = await _context.Prescriptions.CountAsync()
            };

            return View(stats);
        }

        #region User Management

        public async Task<IActionResult> Users()
        {
            var usersDto = await _userManagementService.GetAllUsersAsync();

            // Mapper DTOs vers ViewModels
            var users = usersDto.Select(dto => new UserListItemViewModel
            {
                PersonId = dto.PersonId,
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                EmailConfirmed = dto.EmailConfirmed,
                DateBirthday = dto.DateBirthday,
                DoctorName = dto.DoctorName,
                PatientCount = dto.PatientCount
            }).ToList();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Doctors = await _userManagementService.GetAllDoctorsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _userManagementService.GetAllDoctorsAsync();
                return View(model);
            }

            // Mapper ViewModel vers DTO
            var dto = new CreateUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = model.Role,
                Password = model.Password,
                Gender = model.Gender,
                DateBirthday = model.DateBirthday,
                SocialSecurityNumber = model.SocialSecurityNumber,
                DoctorId = model.DoctorId
            };

            var (success, errors) = await _userManagementService.CreateUserAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Utilisateur créé avec succès.";
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            ViewBag.Doctors = await _userManagementService.GetAllDoctorsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var dto = await _userManagementService.GetUserForEditAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            // Protection : empêcher l'édition d'un compte admin
            if (dto.Role == "Admin")
            {
                TempData["ErrorMessage"] = "Les comptes administrateurs ne peuvent pas être modifiés pour des raisons de sécurité.";
                return RedirectToAction(nameof(Users));
            }

            // Mapper DTO vers ViewModel
            var model = new EditUserViewModel
            {
                PersonId = dto.PersonId,
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                Gender = dto.Gender,
                DateBirthday = dto.DateBirthday,
                SocialSecurityNumber = dto.SocialSecurityNumber,
                DoctorId = dto.DoctorId
            };

            ViewBag.Doctors = await _userManagementService.GetAllDoctorsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _userManagementService.GetAllDoctorsAsync();
                return View(model);
            }

            // Mapper ViewModel vers DTO
            var dto = new UpdateUserDto
            {
                PersonId = model.PersonId,
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = model.Role,
                NewPassword = model.NewPassword,
                Gender = model.Gender,
                DateBirthday = model.DateBirthday,
                SocialSecurityNumber = model.SocialSecurityNumber,
                DoctorId = model.DoctorId
            };

            var (success, errors) = await _userManagementService.UpdateUserAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Utilisateur mis à jour avec succès.";
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            ViewBag.Doctors = await _userManagementService.GetAllDoctorsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var (success, error) = await _userManagementService.DeleteUserAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Utilisateur supprimé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToAction(nameof(Users));
        }

        #endregion

        #region Medicine Management

        public async Task<IActionResult> Medicines()
        {
            var medicinesDto = await _medicineManagementService.GetAllMedicinesAsync();

            var medicines = medicinesDto.Select(dto => new MedicineListItemViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Dosage = dto.Dosage,
                PrescriptionCount = dto.PrescriptionCount
            }).ToList();

            return View(medicines);
        }

        [HttpGet]
        public IActionResult CreateMedicine()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedicine(CreateMedicineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateMedicineDto
            {
                Name = model.Name,
                Description = model.Description,
                Dosage = model.Dosage,
                SideEffects = model.SideEffects
            };

            var (success, errors) = await _medicineManagementService.CreateMedicineAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Médicament créé avec succès.";
                return RedirectToAction(nameof(Medicines));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMedicine(int id)
        {
            var dto = await _medicineManagementService.GetMedicineForEditAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            var model = new EditMedicineViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Dosage = dto.Dosage,
                SideEffects = dto.SideEffects
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicine(EditMedicineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new UpdateMedicineDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Dosage = model.Dosage,
                SideEffects = model.SideEffects
            };

            var (success, errors) = await _medicineManagementService.UpdateMedicineAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Médicament mis à jour avec succès.";
                return RedirectToAction(nameof(Medicines));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var (success, error) = await _medicineManagementService.DeleteMedicineAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Médicament supprimé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToAction(nameof(Medicines));
        }

        #endregion

        #region Allergy Management

        public async Task<IActionResult> Allergies()
        {
            var allergiesDto = await _allergyManagementService.GetAllAllergiesAsync();

            var allergies = allergiesDto.Select(dto => new AllergyListItemViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                PatientCount = dto.PatientCount
            }).ToList();

            return View(allergies);
        }

        [HttpGet]
        public IActionResult CreateAllergy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAllergy(CreateAllergyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateAllergyDto
            {
                Name = model.Name,
                Description = model.Description
            };

            var (success, errors) = await _allergyManagementService.CreateAllergyAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Allergie créée avec succès.";
                return RedirectToAction(nameof(Allergies));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAllergy(int id)
        {
            var dto = await _allergyManagementService.GetAllergyForEditAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            var model = new EditAllergyViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllergy(EditAllergyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new UpdateAllergyDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };

            var (success, errors) = await _allergyManagementService.UpdateAllergyAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Allergie mise à jour avec succès.";
                return RedirectToAction(nameof(Allergies));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllergy(int id)
        {
            var (success, error) = await _allergyManagementService.DeleteAllergyAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Allergie supprimée avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = error;
            }

            return RedirectToAction(nameof(Allergies));
        }

        #endregion
    }
}
