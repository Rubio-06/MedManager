using MedManager.Infrastructure.Context;
using MedManager.Web.Helpers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Patient)]
    public class PatientController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<PatientController> _logger;

        public PatientController(DatabaseContext context, ILogger<PatientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var patientPerson = await UserHelper.GetCurrentPersonAsync(User, _context);
            if (patientPerson == null)
                return RedirectToAction("AccessDenied", "Account");

            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .Include(p => p.Prescriptions)
                .Include(p => p.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                .Include(p => p.PatientHistories)
                    .ThenInclude(ph => ph.History)
                .FirstOrDefaultAsync(p => p.Id == patientPerson.Id);

            return View(patient);
        }

        public async Task<IActionResult> Prescriptions()
        {
            var patientPerson = await UserHelper.GetCurrentPersonAsync(User, _context);
            if (patientPerson == null)
                return RedirectToAction("AccessDenied", "Account");

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionMedicines)
                    .ThenInclude(pm => pm.Medicine)
                .Where(p => p.PatientId == patientPerson.Id)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            return View(prescriptions);
        }

        public async Task<IActionResult> MedicalHistory()
        {
            var patientPerson = await UserHelper.GetCurrentPersonAsync(User, _context);
            if (patientPerson == null)
                return RedirectToAction("AccessDenied", "Account");

            var patient = await _context.Patients
                .Include(p => p.PatientHistories)
                    .ThenInclude(ph => ph.History)
                .Include(p => p.PatientAllergies)
                    .ThenInclude(pa => pa.Allergy)
                .FirstOrDefaultAsync(p => p.Id == patientPerson.Id);

            return View(patient);
        }
    }
}
