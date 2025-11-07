using System.Security.Claims;

using MedManager.Domain.Models.Users;
using MedManager.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;

namespace MedManager.Web.Helpers
{
    public static class UserHelper
    {
        public static async Task<Person?> GetCurrentPersonAsync(ClaimsPrincipal user, DatabaseContext context)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return null;

            return await context.Persons
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        }

        public static async Task<int?> GetCurrentPersonIdAsync(ClaimsPrincipal user, DatabaseContext context)
        {
            var person = await GetCurrentPersonAsync(user, context);
            return person?.Id;
        }

        public static bool HasRole(ClaimsPrincipal user, string role)
        {
            return user.IsInRole(role);
        }

        public static string GetUserRole(ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin"))
                return "Admin";
            if (user.IsInRole("Doctor"))
                return "Doctor";
            if (user.IsInRole("Patient"))
                return "Patient";
            return "Guest";
        }

        public static string GetDashboardRoute(ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin"))
                return "/Admin/Dashboard";
            if (user.IsInRole("Doctor"))
                return "/Doctor/Dashboard";
            if (user.IsInRole("Patient"))
                return "/Patient/Dashboard";
            return "/";
        }
    }
}
