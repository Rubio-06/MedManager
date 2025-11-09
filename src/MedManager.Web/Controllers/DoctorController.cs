using MedManager.Application.DTOs;
using MedManager.Application.Interfaces;
using MedManager.Domain.Models;
using MedManager.Domain.Models.Tables;
using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;
using MedManager.Web.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Web.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IPatientManagementService _patientManagementService;
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(
            IPatientManagementService patientManagementService,
            DatabaseContext context,
            UserManager<ApplicationUser> userManager)
        {
            _patientManagementService = patientManagementService;
            _context = context;
            _userManager = userManager;
        }

        private async Task<int?> GetCurrentDoctorIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return null;

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            return doctor?.Id;
        }

        public async Task<IActionResult> Dashboard()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var doctor = await _context.Doctors
                .Include(d => d.Patients)
                    .ThenInclude(p => p.PatientAllergies)
                .Include(d => d.Prescriptions)
                .FirstOrDefaultAsync(d => d.Id == doctorId.Value);

            return View(doctor);
        }

        #region Patient Management

        public async Task<IActionResult> Patients()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var patientsDto = await _patientManagementService.GetPatientsByDoctorIdAsync(doctorId.Value);

            var patients = patientsDto.Select(dto => new PatientListItemViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateBirthday = dto.DateBirthday,
                Gender = dto.Gender,
                EmailConfirmed = dto.EmailConfirmed,
                PrescriptionCount = dto.PrescriptionCount,
                AllergyCount = dto.AllergyCount
            }).ToList();

            return View(patients);
        }

        [HttpGet]
        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient(CreatePatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var dto = new CreatePatientDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                DateBirthday = model.DateBirthday,
                Gender = model.Gender,
                SocialSecurityNumber = model.SocialSecurityNumber,
                DoctorId = doctorId.Value
            };

            var (success, errors) = await _patientManagementService.CreatePatientAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Patient créé avec succès.";
                return RedirectToAction(nameof(Patients));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(int id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var dto = await _patientManagementService.GetPatientForEditAsync(id, doctorId.Value);
            if (dto == null)
            {
                return NotFound();
            }

            var model = new EditPatientViewModel
            {
                Id = dto.Id,
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateBirthday = dto.DateBirthday,
                Gender = dto.Gender,
                SocialSecurityNumber = dto.SocialSecurityNumber,
                EmailConfirmed = dto.EmailConfirmed
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(EditPatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var dto = new UpdatePatientDto
            {
                Id = model.Id,
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateBirthday = model.DateBirthday,
                Gender = model.Gender,
                SocialSecurityNumber = model.SocialSecurityNumber,
                EmailConfirmed = model.EmailConfirmed
            };

            var (success, errors) = await _patientManagementService.UpdatePatientAsync(dto, doctorId.Value);

            if (success)
            {
                TempData["SuccessMessage"] = "Patient mis à jour avec succès.";
                return RedirectToAction(nameof(Patients));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var (success, errors) = await _patientManagementService.DeletePatientAsync(id, doctorId.Value);

            if (success)
            {
                TempData["SuccessMessage"] = "Patient supprimé avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Patients));
        }

        [HttpGet]
        public async Task<IActionResult> PatientProfile(int id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Récupérer le patient avec toutes ses informations
            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicines)
                        .ThenInclude(pm => pm.Medicine)
                .Include(p => p.MedicalHistories)
                .FirstOrDefaultAsync(p => p.Id == id && p.DoctorId == doctorId.Value);

            if (patient == null)
            {
                return NotFound();
            }

            // Récupérer toutes les allergies disponibles
            var allAllergies = await _context.Allergies.OrderBy(a => a.Name).ToListAsync();

            var viewModel = new PatientProfileViewModel
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.User.Email ?? "",
                DateBirthday = patient.DateBirthday,
                Gender = patient.Gender,
                SocialSecurityNumber = patient.SocialSecurityNumber,
                EmailConfirmed = patient.User.EmailConfirmed,
                Age = CalculateAge(patient.DateBirthday),

                // Allergies
                Allergies = patient.PatientAllergies.Select(pa => new AllergyItemViewModel
                {
                    Id = pa.AllergyId,
                    Name = pa.Allergy.Name,
                    Description = pa.Allergy.Description ?? ""
                }).ToList(),

                // Toutes les allergies disponibles pour la modal
                AvailableAllergies = allAllergies.Select(a => new AllergyItemViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description ?? ""
                }).ToList(),

                // Prescriptions (pour l'instant juste le comptage, on détaillera plus tard)
                PrescriptionCount = patient.Prescriptions.Count,
                ActivePrescriptionCount = patient.Prescriptions.Count,
                Prescriptions = patient.Prescriptions.OrderByDescending(p => p.DateCreated).Select(p => new PatientPrescriptionViewModel
                {
                    Id = p.Id,
                    DateCreated = p.DateCreated,
                    Medicines = p.PrescriptionMedicines.Select(pm => new PrescriptionMedicineDetailViewModel
                    {
                        MedicineId = pm.MedicineId,
                        MedicineName = pm.Medicine.Name,
                        Quantity = pm.Quantity,
                        Dosage = pm.Dosage,
                        Duration = pm.Duration,
                        Instructions = pm.Instructions
                    }).ToList()
                }).ToList(),

                // Medical Histories
                MedicalHistories = patient.MedicalHistories.OrderByDescending(mh => mh.Date).Select(mh => new MedicalHistoryViewModel
                {
                    Id = mh.Id,
                    Type = mh.Type,
                    TypeDisplay = GetMedicalHistoryTypeDisplay(mh.Type),
                    Title = mh.Title,
                    Description = mh.Description,
                    Date = mh.Date,
                    Severity = mh.Severity,
                    SeverityDisplay = GetSeverityDisplay(mh.Severity),
                    CreatedAt = mh.CreatedAt
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAllergies([FromForm] int patientId, [FromForm] List<int> allergyIds)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            // Vérifier que le patient appartient au médecin
            var patient = await _context.Patients
                .Include(p => p.PatientAllergies)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId.Value);

            if (patient == null)
            {
                return Json(new { success = false, message = $"Patient introuvable (ID: {patientId}, DoctorID: {doctorId})" });
            }

            if (allergyIds == null || !allergyIds.Any())
            {
                return Json(new { success = false, message = "Aucune allergie sélectionnée" });
            }

            // Ajouter uniquement les nouvelles allergies
            var existingAllergyIds = patient.PatientAllergies.Select(pa => pa.AllergyId).ToList();
            var newAllergyIds = allergyIds.Where(id => !existingAllergyIds.Contains(id)).ToList();

            foreach (var allergyId in newAllergyIds)
            {
                patient.PatientAllergies.Add(new PatientAllergy
                {
                    PatientId = patientId,
                    AllergyId = allergyId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{newAllergyIds.Count} allergie(s) ajoutée(s) avec succès";
            return Json(new { success = true, message = $"{newAllergyIds.Count} allergie(s) ajoutée(s)" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAllergy([FromForm] int patientId, [FromForm] int allergyId)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            // Vérifier que le patient appartient au médecin
            var patient = await _context.Patients
                .Include(p => p.PatientAllergies)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId.Value);

            if (patient == null)
            {
                return Json(new { success = false, message = "Patient introuvable" });
            }

            // Trouver et supprimer l'allergie
            var patientAllergy = patient.PatientAllergies.FirstOrDefault(pa => pa.AllergyId == allergyId);

            if (patientAllergy == null)
            {
                return Json(new { success = false, message = "Allergie non trouvée pour ce patient" });
            }

            patient.PatientAllergies.Remove(patientAllergy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergie retirée avec succès";
            return Json(new { success = true, message = "Allergie retirée" });
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatients(List<int> patientIds)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (patientIds == null || !patientIds.Any())
            {
                TempData["ErrorMessage"] = "Aucun patient sélectionné.";
                return RedirectToAction(nameof(Patients));
            }

            int successCount = 0;
            int failCount = 0;
            List<string> errorMessages = new List<string>();

            foreach (var patientId in patientIds)
            {
                var (success, errors) = await _patientManagementService.DeletePatientAsync(patientId, doctorId.Value);
                if (success)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                    errorMessages.AddRange(errors);
                }
            }

            if (successCount > 0)
            {
                TempData["SuccessMessage"] = $"{successCount} patient(s) supprimé(s) avec succès.";
            }

            if (failCount > 0)
            {
                TempData["ErrorMessage"] = $"{failCount} patient(s) n'ont pas pu être supprimés : {string.Join(", ", errorMessages.Distinct())}";
            }

            return RedirectToAction(nameof(Patients));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDoctorFromPatients(List<int> patientIds)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (patientIds == null || !patientIds.Any())
            {
                TempData["ErrorMessage"] = "Aucun patient sélectionné.";
                return RedirectToAction(nameof(Patients));
            }

            int successCount = 0;
            foreach (var patientId in patientIds)
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId.Value);

                if (patient != null)
                {
                    patient.DoctorId = null;
                    successCount++;
                }
            }

            if (successCount > 0)
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Vous avez été retiré en tant que médecin traitant pour {successCount} patient(s).";
            }
            else
            {
                TempData["ErrorMessage"] = "Aucun patient n'a pu être modifié.";
            }

            return RedirectToAction(nameof(Patients));
        }

        #endregion

        #region Prescription Management

        public async Task<IActionResult> Prescriptions()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.PrescriptionMedicines)
                .Where(p => p.DoctorId == doctorId.Value)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            var viewModel = prescriptions.Select(p => new PrescriptionListItemViewModel
            {
                Id = p.Id,
                DateCreated = p.DateCreated,
                PatientName = $"{p.Patient.FirstName} {p.Patient.LastName}",
                PatientId = p.PatientId,
                MedicineCount = p.PrescriptionMedicines.Count,
                Status = "Active"
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePrescription(int? patientId)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == doctorId.Value);

            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new CreatePrescriptionViewModel
            {
                DoctorFirstName = doctor.FirstName,
                DoctorLastName = doctor.LastName,
                DoctorSpecialty = "Médecin généraliste" // Hardcodé car le modèle n'a pas de propriété Specialty
            };

            Patient? patient = null;

            // Si patientId est fourni, charger les infos du patient
            if (patientId.HasValue)
            {
                patient = await _context.Patients
                    .Include(p => p.User)
                    .Include(p => p.PatientAllergies)
                        .ThenInclude(pa => pa.Allergy)
                    .FirstOrDefaultAsync(p => p.Id == patientId.Value && p.DoctorId == doctorId.Value);

                if (patient == null)
                {
                    return NotFound();
                }

                viewModel.PatientId = patient.Id;
                viewModel.PatientFirstName = patient.FirstName;
                viewModel.PatientLastName = patient.LastName;
                viewModel.PatientEmail = patient.User.Email ?? "";
                viewModel.PatientAge = CalculateAge(patient.DateBirthday);
                viewModel.PatientGender = patient.Gender == MedManager.Domain.Models.Users.Gender.Male ? "Homme" : "Femme";
                viewModel.SocialSecurityNumber = patient.SocialSecurityNumber;

                // Charger les allergies du patient
                viewModel.PatientAllergies = patient.PatientAllergies.Select(pa => new PatientAllergyViewModel
                {
                    Id = pa.AllergyId,
                    Name = pa.Allergy.Name
                }).ToList();
            }

            // Charger tous les médicaments disponibles
            var medicines = await _context.Medicines
                .Include(m => m.Components)
                .Include(m => m.MedicineAllergies)
                .OrderBy(m => m.Name)
                .ToListAsync();
            
            viewModel.AvailableMedicines = medicines.Select(m =>
            {
                bool hasAllergyConflict = false;
                
                if (patient != null)
                {
                    // Vérifier les allergies directes (table MedicineAllergies)
                    hasAllergyConflict = m.MedicineAllergies.Any(ma => 
                        patient.PatientAllergies.Any(pa => pa.AllergyId == ma.AllergyId));
                    
                    // Si pas de conflit direct, vérifier les composants
                    if (!hasAllergyConflict)
                    {
                        hasAllergyConflict = m.Components.Any(comp => 
                            patient.PatientAllergies.Any(pa => 
                                pa.Allergy.Name.Equals(comp.Name, StringComparison.OrdinalIgnoreCase) ||
                                comp.Name.Contains(pa.Allergy.Name, StringComparison.OrdinalIgnoreCase)));
                    }
                }

                return new MedicineItemViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Composition = string.Join(", ", m.Components.Select(c => $"{c.Name} {c.Dosage}")),
                    Description = m.Description ?? "",
                    Price = 0, // Le prix n'est pas dans le modèle actuel
                    HasAllergyConflict = hasAllergyConflict
                };
            }).ToList();

            return View("CreatePrescriptionNew", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription(int patientId, string medicines)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Désérialiser les médicaments depuis JSON
            List<PrescriptionMedicineDto>? medicinesList = null;
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                medicinesList = System.Text.Json.JsonSerializer.Deserialize<List<PrescriptionMedicineDto>>(medicines, options);
            }
            catch
            {
                TempData["ErrorMessage"] = "Erreur lors de la lecture des médicaments.";
                return RedirectToAction(nameof(CreatePrescription), new { patientId });
            }

            if (medicinesList == null || !medicinesList.Any())
            {
                TempData["ErrorMessage"] = "Vous devez ajouter au moins un médicament à l'ordonnance.";
                return RedirectToAction(nameof(CreatePrescription), new { patientId });
            }

            // Vérifier les allergies du patient
            var patient = await _context.Patients
                .Include(p => p.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                        .ThenInclude(a => a.MedicineAllergies)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId.Value);

            if (patient == null)
            {
                return NotFound();
            }

            // Vérifier les conflits d'allergies
            var medicineIds = medicinesList.Select(m => m.MedicineId).ToList();
            var prescribedMedicines = await _context.Medicines
                .Include(m => m.MedicineAllergies)
                .Include(m => m.Components) // Charger les composants
                .Where(m => medicineIds.Contains(m.Id))
                .ToListAsync();

            foreach (var medicine in prescribedMedicines)
            {
                // Vérifier les allergies directes (médicament dans la table MedicineAllergies)
                var allergyConflict = medicine.MedicineAllergies
                    .Any(ma => patient.PatientAllergies.Any(pa => pa.AllergyId == ma.AllergyId));

                if (allergyConflict)
                {
                    var allergyName = medicine.MedicineAllergies
                        .Where(ma => patient.PatientAllergies.Any(pa => pa.AllergyId == ma.AllergyId))
                        .Select(ma => patient.PatientAllergies.First(pa => pa.AllergyId == ma.AllergyId).Allergy.Name)
                        .FirstOrDefault();

                    TempData["ErrorMessage"] = $"⚠️ ATTENTION : Le patient est allergique à {allergyName}. Le médicament '{medicine.Name}' ne peut pas être prescrit.";
                    return RedirectToAction(nameof(CreatePrescription), new { patientId });
                }

                // Vérifier si un composant du médicament correspond à une allergie du patient
                foreach (var component in medicine.Components)
                {
                    var componentMatchesAllergy = patient.PatientAllergies.Any(pa => 
                        pa.Allergy.Name.Equals(component.Name, StringComparison.OrdinalIgnoreCase) ||
                        component.Name.Contains(pa.Allergy.Name, StringComparison.OrdinalIgnoreCase));

                    if (componentMatchesAllergy)
                    {
                        var matchedAllergy = patient.PatientAllergies
                            .First(pa => pa.Allergy.Name.Equals(component.Name, StringComparison.OrdinalIgnoreCase) ||
                                        component.Name.Contains(pa.Allergy.Name, StringComparison.OrdinalIgnoreCase))
                            .Allergy.Name;

                        TempData["ErrorMessage"] = $"⚠️ ATTENTION : Le patient est allergique à {matchedAllergy}. Le médicament '{medicine.Name}' contient '{component.Name}' et ne peut pas être prescrit.";
                        return RedirectToAction(nameof(CreatePrescription), new { patientId });
                    }
                }
            }

            // Créer l'ordonnance
            var prescription = new Prescription
            {
                PatientId = patientId,
                DoctorId = doctorId.Value,
                DateCreated = DateTime.Now
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            // Ajouter les médicaments
            foreach (var med in medicinesList)
            {
                var prescriptionMedicine = new PrescriptionMedicine
                {
                    PrescriptionId = prescription.Id,
                    MedicineId = med.MedicineId,
                    Quantity = med.Quantity,
                    Dosage = med.Dosage,
                    Duration = med.Duration,
                    Instructions = med.Instructions
                };

                _context.PrescriptionMedicines.Add(prescriptionMedicine);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Ordonnance créée avec succès pour {patient.FirstName} {patient.LastName}.";
            return RedirectToAction(nameof(PrescriptionDetails), new { id = prescription.Id });
        }

        [HttpGet]
        public async Task<IActionResult> PrescriptionDetails(int id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                    .ThenInclude(p => p.User)
                .Include(p => p.Patient.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionMedicines)
                    .ThenInclude(pm => pm.Medicine)
                        .ThenInclude(m => m.Components)
                .FirstOrDefaultAsync(p => p.Id == id && p.DoctorId == doctorId.Value);

            if (prescription == null)
            {
                return NotFound();
            }

            var viewModel = new PrescriptionDetailsViewModel
            {
                Id = prescription.Id,
                DateCreated = prescription.DateCreated,
                PatientId = prescription.PatientId,
                PatientFirstName = prescription.Patient.FirstName,
                PatientLastName = prescription.Patient.LastName,
                PatientEmail = prescription.Patient.User.Email ?? "",
                PatientAge = CalculateAge(prescription.Patient.DateBirthday),
                PatientGender = prescription.Patient.Gender == MedManager.Domain.Models.Users.Gender.Male ? "Homme" : "Femme",
                SocialSecurityNumber = prescription.Patient.SocialSecurityNumber,
                DoctorFirstName = prescription.Doctor.FirstName,
                DoctorLastName = prescription.Doctor.LastName,
                PatientAllergies = prescription.Patient.PatientAllergies.Select(pa => new PatientAllergyViewModel
                {
                    Id = pa.AllergyId,
                    Name = pa.Allergy.Name
                }).ToList(),
                Medicines = prescription.PrescriptionMedicines.Select(pm => new PrescriptionMedicineViewModel
                {
                    MedicineId = pm.MedicineId,
                    MedicineName = pm.Medicine.Name,
                    Composition = string.Join(", ", pm.Medicine.Components.Select(c => $"{c.Name} {c.Dosage}")),
                    Quantity = pm.Quantity,
                    Dosage = pm.Dosage,
                    Duration = pm.Duration,
                    Instructions = pm.Instructions
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePrescription([FromForm] int prescriptionId)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            // Vérifier que l'ordonnance appartient au médecin
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionMedicines)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == prescriptionId && p.DoctorId == doctorId.Value);

            if (prescription == null)
            {
                return Json(new { success = false, message = "Ordonnance introuvable" });
            }

            // Supprimer les médicaments de l'ordonnance d'abord (cascade)
            _context.PrescriptionMedicines.RemoveRange(prescription.PrescriptionMedicines);

            // Supprimer l'ordonnance
            _context.Prescriptions.Remove(prescription);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Ordonnance du {prescription.DateCreated:dd/MM/yyyy} supprimée avec succès" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorPatients()
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new List<object>());
            }

            var patients = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                .Where(p => p.DoctorId == doctorId.Value)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new
                {
                    id = p.Id,
                    firstName = p.FirstName,
                    lastName = p.LastName,
                    email = p.User.Email,
                    allergies = p.PatientAllergies.Select(pa => new { 
                        id = pa.AllergyId, 
                        name = pa.Allergy.Name 
                    }).ToList()
                })
                .ToListAsync();

            return Json(patients);
        }

        #endregion

        #region Medical History Management

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicalHistory([FromForm] int patientId, [FromForm] string type, 
            [FromForm] string title, [FromForm] string description, [FromForm] DateTime date, [FromForm] string severity)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            // Vérifier que le patient appartient au docteur
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == patientId && p.DoctorId == doctorId.Value);
            if (patient == null)
            {
                return Json(new { success = false, message = "Patient non trouvé" });
            }

            // Parser les enums
            if (!Enum.TryParse<MedicalHistoryType>(type, out var historyType))
            {
                return Json(new { success = false, message = "Type d'antécédent invalide" });
            }

            if (!Enum.TryParse<Severity>(severity, out var severityLevel))
            {
                return Json(new { success = false, message = "Niveau de sévérité invalide" });
            }

            var medicalHistory = new MedicalHistory
            {
                PatientId = patientId,
                Type = historyType,
                Title = title,
                Description = description,
                Date = date,
                Severity = severityLevel,
                CreatedAt = DateTime.Now
            };

            _context.MedicalHistories.Add(medicalHistory);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Antécédent ajouté avec succès" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMedicalHistory([FromForm] int id, [FromForm] int patientId, 
            [FromForm] string type, [FromForm] string title, [FromForm] string description, 
            [FromForm] DateTime date, [FromForm] string severity)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            var medicalHistory = await _context.MedicalHistories
                .Include(mh => mh.Patient)
                .FirstOrDefaultAsync(mh => mh.Id == id && mh.Patient.DoctorId == doctorId.Value);

            if (medicalHistory == null)
            {
                return Json(new { success = false, message = "Antécédent non trouvé" });
            }

            // Parser les enums
            if (!Enum.TryParse<MedicalHistoryType>(type, out var historyType))
            {
                return Json(new { success = false, message = "Type d'antécédent invalide" });
            }

            if (!Enum.TryParse<Severity>(severity, out var severityLevel))
            {
                return Json(new { success = false, message = "Niveau de sévérité invalide" });
            }

            medicalHistory.Type = historyType;
            medicalHistory.Title = title;
            medicalHistory.Description = description;
            medicalHistory.Date = date;
            medicalHistory.Severity = severityLevel;
            medicalHistory.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Antécédent modifié avec succès" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicalHistory([FromForm] int id)
        {
            var doctorId = await GetCurrentDoctorIdAsync();
            if (doctorId == null)
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            var medicalHistory = await _context.MedicalHistories
                .Include(mh => mh.Patient)
                .FirstOrDefaultAsync(mh => mh.Id == id && mh.Patient.DoctorId == doctorId.Value);

            if (medicalHistory == null)
            {
                return Json(new { success = false, message = "Antécédent non trouvé" });
            }

            _context.MedicalHistories.Remove(medicalHistory);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Antécédent supprimé avec succès" });
        }

        #endregion

        #region Helper Methods

        private static string GetMedicalHistoryTypeDisplay(MedicalHistoryType type)
        {
            return type switch
            {
                MedicalHistoryType.ChronicDisease => "Maladie chronique",
                MedicalHistoryType.Surgery => "Opération chirurgicale",
                MedicalHistoryType.Hospitalization => "Hospitalisation",
                MedicalHistoryType.FamilyHistory => "Antécédent familial",
                MedicalHistoryType.Vaccination => "Vaccination",
                MedicalHistoryType.CurrentCondition => "Condition actuelle",
                MedicalHistoryType.Other => "Autre",
                _ => "Non spécifié"
            };
        }

        private static string GetSeverityDisplay(Severity severity)
        {
            return severity switch
            {
                Severity.Low => "Faible",
                Severity.Medium => "Modérée",
                Severity.High => "Élevée",
                Severity.Critical => "Critique",
                _ => "Non spécifié"
            };
        }

        #endregion
    }
}
