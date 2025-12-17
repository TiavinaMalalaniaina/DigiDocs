using BackOffice.Models;
using BackOffice.Models.Enums;
using Microsoft.Data;
using Microsoft.Data.SqlClient;

namespace BackOffice.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DbConnection _db;

        public DocumentRepository(DbConnection db)
        {
            _db = db;
        }

        public async Task<int> GetCountAsync()
        {
            using var conn = _db.CreateConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Documents";
            conn.Open();
            return (int)await ((SqlCommand)cmd).ExecuteScalarAsync();
        }

        public async Task<List<Document>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var list = new List<Document>();
            using var conn = _db.CreateConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            SELECT * FROM Documents
            ORDER BY UploadDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

            var offsetParam = cmd.CreateParameter();
            offsetParam.ParameterName = "@Offset";
            offsetParam.Value = (pageNumber - 1) * pageSize;
            cmd.Parameters.Add(offsetParam);

            var sizeParam = cmd.CreateParameter();
            sizeParam.ParameterName = "@PageSize";
            sizeParam.Value = pageSize;
            cmd.Parameters.Add(sizeParam);

            conn.Open();
            using var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Document
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    FileName = reader.GetString(reader.GetOrdinal("FileName")),
                    Category = reader.GetString(reader.GetOrdinal("Category")),
                    UploadDate = reader.GetDateTime(reader.GetOrdinal("UploadDate")),
                    DownloadCount = reader.GetInt32(reader.GetOrdinal("DownloadCount")),
                    AccessLevel = reader.GetString(reader.GetOrdinal("AccessLevel")),
                    ContentType = Enum.Parse<ContentTypeDocument>(reader.GetString(reader.GetOrdinal("ContentType")))
                });
            }
            return list;
        }

        public async Task AddAsync(Document document)
        {
            using var conn = (SqlConnection)_db.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Documents 
                (Title, UploadDate, DownloadCount, FileData, FileName, Category, ContentType, Description, AccessLevel)
                VALUES (@Title, @UploadDate, @DownloadCount, @FileData, @FileName, @Category, @ContentType, @Description, @AccessLevel)";

            cmd.Parameters.Add(new SqlParameter("@Title", document.Title ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@UploadDate", document.UploadDate));
            cmd.Parameters.Add(new SqlParameter("@DownloadCount", document.DownloadCount));
            cmd.Parameters.Add(new SqlParameter("@FileData", document.FileData ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@FileName", document.FileName ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Category", document.Category ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@ContentType", document.ContentType.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Description", document.Description ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@AccessLevel", document.AccessLevel ?? (object)DBNull.Value));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = (SqlConnection)_db.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Documents WHERE Id = @Id";
            cmd.Parameters.Add(new SqlParameter("@Id", id));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            var list = new List<Document>();

            using var conn = (SqlConnection)_db.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Documents";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapDocument(reader));
            }

            return list;
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            using var conn = (SqlConnection)_db.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Documents WHERE Id = @Id";
            cmd.Parameters.Add(new SqlParameter("@Id", id));

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapDocument(reader);
            }

            return null;
        }

        public async Task UpdateAsync(Document document)
        {
            using var conn = (SqlConnection)_db.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Documents SET
                Title = @Title,
                UploadDate = @UploadDate,
                DownloadCount = @DownloadCount,
                FileData = @FileData,
                FileName = @FileName,
                Category = @Category,
                ContentType = @ContentType,
                Description = @Description,
                AccessLevel = @AccessLevel
                WHERE Id = @Id";

            cmd.Parameters.Add(new SqlParameter("@Id", document.Id));
            cmd.Parameters.Add(new SqlParameter("@Title", document.Title ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@UploadDate", document.UploadDate));
            cmd.Parameters.Add(new SqlParameter("@DownloadCount", document.DownloadCount));
            cmd.Parameters.Add(new SqlParameter("@FileData", document.FileData ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@FileName", document.FileName ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Category", document.Category ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@ContentType", document.ContentType.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Description", document.Description ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@AccessLevel", document.AccessLevel ?? (object)DBNull.Value));

            await cmd.ExecuteNonQueryAsync();
        }

        private Document MapDocument(SqlDataReader reader)
        {
            return new Document
            {
                Id = (int)reader["Id"],
                Title = reader["Title"] as string,
                UploadDate = (DateTime)reader["UploadDate"],
                DownloadCount = (int)reader["DownloadCount"],
                FileData = reader["FileData"] as byte[],
                FileName = reader["FileName"] as string,
                Category = reader["Category"] as string,
                ContentType = Enum.TryParse<ContentTypeDocument>(reader["ContentType"].ToString(), out var ct) ? ct : ContentTypeDocument.Other,
                Description = reader["Description"] as string,
                AccessLevel = reader["AccessLevel"] as string
            };
        }
    }
}
