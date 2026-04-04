using BackOffice.Models;
using BackOffice.Pages.Shared;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BackOffice.Pages
{
    public class IndexModel(ISessionService sessionService, IUserService _userService, IDocumentService _documentService) : AuthenticatedPageModel(sessionService)
    {
        private readonly IUserService userService = _userService;
        private readonly IDocumentService documentService = _documentService;

        public string UserRoleDistributions { get; set; } = String.Empty;
        public string CategoryDownloads { get; set; } = String.Empty;
        public List<Document> MostPopularDocuments { get; set; } = new List<Document>();

        public async Task OnGetAsync()
        {
            UserRoleDistributions = JsonSerializer.Serialize( await userService.GetUserDistributionAsync());
            CategoryDownloads = JsonSerializer.Serialize(await documentService.GetDownloadsByCategoryAsync());
            MostPopularDocuments = await documentService.GetMostPopularDocumentAsync(5);
        }
    }
}
