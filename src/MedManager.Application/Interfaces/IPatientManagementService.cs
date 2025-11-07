using MedManager.Application.DTOs;

namespace MedManager.Application.Interfaces
{
    public interface IPatientManagementService
    {
        Task<List<PatientDto>> GetPatientsByDoctorIdAsync(int doctorId);
        Task<PatientDto?> GetPatientByIdAsync(int patientId, int doctorId);
        Task<UpdatePatientDto?> GetPatientForEditAsync(int patientId, int doctorId);
        Task<(bool Success, string[] Errors)> CreatePatientAsync(CreatePatientDto dto);
        Task<(bool Success, string[] Errors)> UpdatePatientAsync(UpdatePatientDto dto, int doctorId);
        Task<(bool Success, string[] Errors)> DeletePatientAsync(int patientId, int doctorId);
    }
}
