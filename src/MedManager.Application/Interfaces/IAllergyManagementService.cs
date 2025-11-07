using MedManager.Application.DTOs;

namespace MedManager.Application.Interfaces
{
    public interface IAllergyManagementService
    {
        Task<List<AllergyDto>> GetAllAllergiesAsync();
        Task<AllergyDto?> GetAllergyByIdAsync(int id);
        Task<UpdateAllergyDto?> GetAllergyForEditAsync(int id);
        Task<(bool Success, string[] Errors)> CreateAllergyAsync(CreateAllergyDto dto);
        Task<(bool Success, string[] Errors)> UpdateAllergyAsync(UpdateAllergyDto dto);
        Task<(bool Success, string[] Errors)> DeleteAllergyAsync(int id);
    }
}
