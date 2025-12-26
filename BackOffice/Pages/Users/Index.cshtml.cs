using BackOffice.Models;
using BackOffice.Models.Enums;
using BackOffice.Pages.Shared;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Numerics;

namespace BackOffice.Pages.Users
{
    public class IndexModel(ISessionService sessionService, IUserService userService) : AuthenticatedPageModel(sessionService)
    {
        private readonly IUserService _userService = userService;
        public IList<User> Users{ get; set; } = new List<User>();
        public int TotalUser { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public string Index{ get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public string Role { get; set; } = String.Empty;
        public async Task OnGetAsync(int? page)
        {
            Page = page ?? 1;
            TotalUser = await _userService.GetUserCountAsync();
            Users = await _userService.GetUsersAsync(Index, Page, PageSize);
        }

        public async Task OnGetSearchAsync()
        {
            Page = 1;
            TotalUser = await _userService.GetUserCountAsync();
            Users = await _userService.GetUsersAsync(Index, Page, PageSize);
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            User user = new User
            {
                Id = Id,
                Role = Enum.Parse<UserRole>(Role)
            };
            await _userService.UpdateUserRoleAsync(user);
            return RedirectToPage("/Users/Index");
        }
    }
}
