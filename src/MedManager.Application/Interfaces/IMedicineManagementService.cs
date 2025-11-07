using MedManager.Application.DTOs;

namespace MedManager.Application.Interfaces
{
    public interface IMedicineManagementService
    {
        Task<List<MedicineDto>> GetAllMedicinesAsync();
        Task<MedicineDto?> GetMedicineByIdAsync(int id);
        Task<UpdateMedicineDto?> GetMedicineForEditAsync(int id);
        Task<(bool Success, string[] Errors)> CreateMedicineAsync(CreateMedicineDto dto);
        Task<(bool Success, string[] Errors)> UpdateMedicineAsync(UpdateMedicineDto dto);
        Task<(bool Success, string Error)> DeleteMedicineAsync(int id);
    }
}
