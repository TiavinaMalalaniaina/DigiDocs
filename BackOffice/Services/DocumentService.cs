using BackOffice.Data.Repositories;
using BackOffice.Models;
using BackOffice.Models.Enums;
using System.Globalization;
using System.IO.Compression;

namespace BackOffice.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }
        public async Task ImportFromZipAsync(string zipPath)
        {
            if (!File.Exists(zipPath))
                throw new FileNotFoundException("Le fichier ZIP est introuvable", zipPath);

            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            ZipFile.ExtractToDirectory(zipPath, tempFolder);

            var csvFile = Directory.GetFiles(tempFolder, "*.csv", SearchOption.AllDirectories).FirstOrDefault();
            if (csvFile == null)
            {
                Directory.Delete(tempFolder, true);
                throw new Exception("Aucun fichier CSV trouvé dans le ZIP !");
            }

            var lines = File.ReadAllLines(csvFile);
            foreach (var line in lines.Skip(1)) // ignorer l'entête
            {
                Console.WriteLine(line);
                var cols = line.Split(',');
                if (cols.Length < 5) continue;
                Console.WriteLine(cols[0]);
                var doc = new Document
                {
                    Title = cols[0].Trim(),
                    FileName = cols[1].Trim(),
                    Category = cols[2].Trim(),
                    Description = cols[3].Trim(),
                    AccessLevel = cols[4].Trim(),
                    UploadDate = DateTime.Now,
                    DownloadCount = 0,
                    ContentType = GetContentType(cols[1].Trim())
                };

                var fileInZip = Directory.GetFiles(tempFolder, doc.FileName, SearchOption.AllDirectories).FirstOrDefault();
                if (fileInZip != null)
                {
                    doc.FileData = File.ReadAllBytes(fileInZip);
                }

                await AddDocumentAsync(doc);
            }

            // Nettoyer le dossier temporaire
            Directory.Delete(tempFolder, true);
        }

        public Task<int> GetCountAsync() => _documentRepository.GetCountAsync();
        public Task<List<Document>> GetPagedAsync(int pageNumber, int pageSize) => _documentRepository.GetPagedAsync(pageNumber, pageSize);


        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _documentRepository.GetAllAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }

        public async Task AddDocumentAsync(Document document)
        {
            document.UploadDate = DateTime.UtcNow;
            document.DownloadCount = 0;

            await _documentRepository.AddAsync(document);
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            await _documentRepository.UpdateAsync(document);
        }

        public async Task DeleteDocumentAsync(int id)
        {
            await _documentRepository.DeleteAsync(id);
        }

        private ContentTypeDocument GetContentType(string filename)
        {
            var ext = Path.GetExtension(filename).ToLower();
            return ext switch
            {
                ".pdf" => ContentTypeDocument.PDF,
                ".doc" => ContentTypeDocument.Word,
                ".docx" => ContentTypeDocument.Word,
                ".xls" => ContentTypeDocument.Excel,
                ".xlsx" => ContentTypeDocument.Excel,
                _ => ContentTypeDocument.Other
            };
        }
    }
}
