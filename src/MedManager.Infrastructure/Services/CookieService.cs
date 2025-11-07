using Microsoft.Extensions.DependencyInjection;

namespace MedManager.Infrastructure.Services
{
    public static class CookieService
    {
        public static IServiceCollection AddCookieServices(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            return services;
        }
    }
}