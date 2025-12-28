using BackOffice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Data.Repositories
{
    public interface IDocumentRepository
    {
        Task AddAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
        Task<List<Document>> GetDocumentsAsync(string index, int pageNumber, int pageSize);
        Task<Document> GetDocumentByIdAsync(int id);
    }
}
