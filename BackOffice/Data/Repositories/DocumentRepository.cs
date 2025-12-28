using BackOffice.Models;
using BackOffice.Models.Enums;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DigiDocsDbContext _context;

        public DocumentRepository(DigiDocsDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<Document>> GetDocumentsAsync(string? index, int pageNumber, int pageSize)
        {
            var query = _context.Documents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(index))
            {
                query = query.Where(d =>
                    d.Title.Contains(index) ||
                    d.FileName.Contains(index) ||
                    d.Category.Contains(index)
                );
            }

            return await query
                .OrderByDescending(d => d.UploadDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task AddAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return; // ou throw

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var existingDocument = await _context.Documents.FindAsync(document.Id);
            if (existingDocument == null)
                return; // ou throw

            // Mettre à jour les champs
            existingDocument.Title = document.Title;
            existingDocument.UploadDate = document.UploadDate;
            existingDocument.DownloadCount = document.DownloadCount;
            existingDocument.FileData = document.FileData;
            existingDocument.FileName = document.FileName;
            existingDocument.Category = document.Category;
            existingDocument.ContentType = document.ContentType;
            existingDocument.Description = document.Description;
            existingDocument.AccessLevel = document.AccessLevel;

            await _context.SaveChangesAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
