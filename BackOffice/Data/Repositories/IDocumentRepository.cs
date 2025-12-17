using BackOffice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Data.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(int id);
        Task<IEnumerable<Document>> GetAllAsync();
        Task AddAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
        Task<List<Document>> GetPagedAsync(int pageNumber, int pageSize);
    }
}
