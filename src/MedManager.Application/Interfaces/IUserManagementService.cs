using MedManager.Application.DTOs;
using MedManager.Domain.Models.Users;

namespace MedManager.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int personId);
        Task<UpdateUserDto?> GetUserForEditAsync(int personId);
        Task<(bool Success, string[] Errors)> CreateUserAsync(CreateUserDto dto);
        Task<(bool Success, string[] Errors)> UpdateUserAsync(UpdateUserDto dto);
        Task<(bool Success, string Error)> DeleteUserAsync(int personId);
        Task<List<Doctor>> GetAllDoctorsAsync();
    }
}
