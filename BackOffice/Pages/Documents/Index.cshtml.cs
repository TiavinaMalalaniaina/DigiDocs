using BackOffice.Models;
using BackOffice.Models.Enums;
using BackOffice.Pages.Shared;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackOffice.Pages.Documents
{
    public class IndexModel(ISessionService sessionService, IDocumentService documentService) : AuthenticatedPageModel(sessionService)
    {
        private readonly IDocumentService _documentService = documentService;

        public IList<Document> Documents { get; set; } = new List<Document>();

        [BindProperty]
        public AddDocumentModel AddDocument { get; set; } = new AddDocumentModel();

        [BindProperty]
        public EditDocumentModel EditDocument{ get; set; } = new EditDocumentModel();

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string Category { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string AccessLevel { get; set; } = "Standard";

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalDocuments { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalDocuments / PageSize);

        [BindProperty(SupportsGet = true)]
        public string Index { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? ZipFile { get; set; }

        public async Task<IActionResult> OnPostImportZipAsync()
        {
            if (ZipFile == null) Console.WriteLine("Votre fichier est null");
            if (ZipFile == null || ZipFile.Length == 0)
            {
                ModelState.AddModelError("ZipFile", "Veuillez sélectionner un fichier ZIP.");
                return Page();
            }
            Console.WriteLine("IMPORT CSV ---");
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zip");
            using (var stream = new FileStream(tempPath, FileMode.Create))
                await ZipFile.CopyToAsync(stream);

            try
            {
                await _documentService.ImportFromZipAsync(tempPath);
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }

            return RedirectToPage();
        }

        public async Task OnGetAsync(int? pageNumber)
        {
            PageNumber = pageNumber ?? 1;
            TotalDocuments = await _documentService.GetCountAsync();
            Documents = await _documentService.GetDocumentASync(Index, PageNumber, PageSize);
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var document = new Document
            {
                Title = EditDocument.Title,
                Category = EditDocument.Category,
                Description = EditDocument.Description,
                AccessLevel = EditDocument.AccessLevel
            };
            await _documentService.UpdateDocumentAsync(document);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (AddDocument.FileData == null)
            {
                ModelState.AddModelError("UploadFile", "Veuillez sélectionner un fichier.");
                await OnGetAsync(PageNumber);
                return Page();
            }

            using var ms = new MemoryStream();
            await AddDocument.FileData.CopyToAsync(ms);

            var document = new Document
            {
                Title = AddDocument.Title,
                Category = AddDocument.Category,
                Description = AddDocument.Description,
                AccessLevel = AddDocument.AccessLevel,
                FileData = ms.ToArray(),
                FileName = AddDocument.FileData.FileName,
                ContentType = GetContentType(AddDocument.FileData.FileName)
            };

            await _documentService.AddDocumentAsync(document);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return RedirectToPage();
        }

        private ContentTypeDocument GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLower();
            return ext switch
            {
                ".pdf" => ContentTypeDocument.PDF,
                ".doc" or ".docx" => ContentTypeDocument.Word,
                ".xls" or ".xlsx" => ContentTypeDocument.Excel,
                ".ppt" or ".pptx" => ContentTypeDocument.PowerPoint,
                ".txt" => ContentTypeDocument.Text,
                _ => ContentTypeDocument.Other
            };
        }

        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc == null || doc.FileData == null)
                return NotFound();

            // Renvoie le fichier avec son type MIME
            return File(doc.FileData, "application/octet-stream", doc.FileName);
        }


    }
}
