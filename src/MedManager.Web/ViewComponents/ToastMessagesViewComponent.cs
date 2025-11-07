using Microsoft.AspNetCore.Mvc;

namespace MedManager.Web.ViewComponents
{
    public class ToastMessagesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new ToastMessagesViewModel
            {
                SuccessMessage = TempData["SuccessMessage"]?.ToString(),
                ErrorMessage = TempData["ErrorMessage"]?.ToString(),
                WarningMessage = TempData["WarningMessage"]?.ToString(),
                InfoMessage = TempData["InfoMessage"]?.ToString()
            };

            return View(model);
        }
    }

    public class ToastMessagesViewModel
    {
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public string? WarningMessage { get; set; }
        public string? InfoMessage { get; set; }
    }
}
