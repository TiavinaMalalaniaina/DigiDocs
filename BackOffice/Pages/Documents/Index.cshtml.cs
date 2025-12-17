using BackOffice.Pages.Shared;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackOffice.Pages.Documents
{
    public class IndexModel(ISessionService sessionService) : AuthenticatedPageModel(sessionService)
    {
        public void OnGet()
        {
        }
    }
}
