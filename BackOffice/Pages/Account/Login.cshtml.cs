using BackOffice.Exceptions;
using BackOffice.Services;
using BackOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Pages.Account
{
    public class LoginModel(ISessionService sessionService, IAuthService authService) : PageModel
    {
        private readonly ISessionService _sessionService = sessionService;
        private readonly IAuthService _authService = authService;

        [BindProperty]
        [Required(ErrorMessage = "Email Obligatoire")]
        public required string Email { get; set; } = "admin@mail.com";
        [BindProperty]
        [Required(ErrorMessage = "Mot de passe Obligatoire")]
        [DataType(DataType.Password)]
        public required string Password { get; set; } = "hash123";

        [TempData]
        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            ErrorMessage = null;
            if (_sessionService.IsLoggedIn())
            {
                return RedirectToPage("../Index");
            }
            return Page();
        }   

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                User user = _authService.Login(Email, Password);
                _sessionService.SetUser(user);
            } catch (LoginFailedException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            return RedirectToPage("../Index");
        }
    }
}
