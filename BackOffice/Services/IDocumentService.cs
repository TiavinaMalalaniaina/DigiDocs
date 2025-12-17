using BackOffice.Models;

namespace BackOffice.Services
{
    public interface IDocumentService
    {
        Task<int> GetCountAsync();
        Task<List<Document>> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Document>> GetAllDocumentsAsync();
        Task<Document?> GetDocumentByIdAsync(int id);
        Task AddDocumentAsync(Document document);
        Task UpdateDocumentAsync(Document document);
        Task DeleteDocumentAsync(int id);
        Task ImportFromZipAsync(string zipPath);
    }
}
