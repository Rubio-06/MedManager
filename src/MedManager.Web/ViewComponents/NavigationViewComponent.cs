using MedManager.Infrastructure.Context;
using MedManager.Web.Helpers;
using MedManager.Web.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Web.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly DatabaseContext _context;

        public NavigationViewComponent(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return View("Default", new UserInfoViewModel());
            }

            var claimsPrincipal = (System.Security.Claims.ClaimsPrincipal)User;
            var person = await UserHelper.GetCurrentPersonAsync(claimsPrincipal, _context);
            var role = UserHelper.GetUserRole(claimsPrincipal);

            var userInfo = new UserInfoViewModel
            {
                PersonId = person?.Id ?? 0,
                FirstName = person?.FirstName ?? "",
                LastName = person?.LastName ?? "",
                Email = User.Identity.Name ?? "",
                Role = role
            };

            return View("Default", userInfo);
        }
    }
}
