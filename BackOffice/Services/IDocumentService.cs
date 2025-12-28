using BackOffice.Models;

namespace BackOffice.Services
{
    public interface IDocumentService
    {
        Task<int> GetCountAsync();
        Task<List<Document>> GetDocumentASync(string index, int pageNumber, int pageSize);
        Task<Document?> GetDocumentByIdAsync(int id);
        Task AddDocumentAsync(Document document);
        Task UpdateDocumentAsync(Document document);
        Task DeleteDocumentAsync(int id);
        Task ImportFromZipAsync(string zipPath);
    }
}
