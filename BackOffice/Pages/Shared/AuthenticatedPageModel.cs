using BackOffice.Models;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackOffice.Pages.Shared
{
    public class AuthenticatedPageModel(ISessionService sessionService) : PageModel()
    {
        protected readonly ISessionService _sessionService = sessionService;
        public string? Username { get; private set; }
        public string? Email { get; private set; }
        public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
        {
            var user = _sessionService.GetUser();
            
            if (user == null)
            {
                context.Result = new RedirectToPageResult("/Account/Login");
            } else
            {
                Username = user.Username;
                ViewData["Username"] = user.Username;
                Email = user.Email;
                ViewData["Email"] = user.Email;
            }
            base.OnPageHandlerExecuting(context);
        }
    }
}
