using FrontOffice.Models;
using Microsoft.Data.SqlClient;
using FrontOffice.Helpers;
using FrontOffice.Data;

namespace FrontOffice.Data.Repositories
{
    public class DocumentRepository
    {
        private readonly string _connectionString;

        public DocumentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Document> GetAll()
        {
            var documents = new List<Document>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Documents", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                documents.Add(new Document
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString(),
                    Category = reader["Category"].ToString(),
                    AccessLevel = reader["AccessLevel"].ToString(),
                    FileName = reader["FileName"].ToString(),
                    ContentType = reader["ContentType"].ToString(),
                    FileData = (byte[])reader["FileData"],
                    DownloadCount = (int)reader["DownloadCount"],
                    UploadDate = (DateTime)reader["UploadDate"]
                });
            }

            return documents;
        }

        public Document GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Documents WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Document
            {
                Id = (int)reader["Id"],
                Title = reader["Title"].ToString(),
                Category = reader["Category"].ToString(),
                AccessLevel = reader["AccessLevel"].ToString(),
                FileName = reader["FileName"].ToString(),
                ContentType = reader["ContentType"].ToString(),
                FileData = (byte[])reader["FileData"]
            };
        }

        public void IncrementDownloadCount(int documentId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "UPDATE Documents SET DownloadCount = DownloadCount + 1 WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", documentId);
            cmd.ExecuteNonQuery();
        }

        public void LogDownload(int documentId, int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO DocumentDownloads (DocumentId, UserId) VALUES (@docId, @userId)", conn);
            cmd.Parameters.AddWithValue("@docId", documentId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
        }

        // public List<Document> GetAccessibleDocuments(string userRole, string? category)
        // {
        //     int userLevel = AccessHelper.GetRoleLevel(userRole);
        //     var documents = new List<Document>();

        //     using var conn = new SqlConnection(_connectionString);
        //     conn.Open();

        //     var sql = @"
        //         SELECT Id, Title, Category, AccessLevel
        //         FROM Documents
        //         WHERE
        //             CASE AccessLevel
        //                 WHEN 'standard' THEN 1
        //                 WHEN 'premium' THEN 2
        //                 WHEN 'vip' THEN 3
        //             END <= @UserLevel
        //     ";

        //     if (!string.IsNullOrEmpty(category))
        //         sql += " AND Category = @Category";

        //     var cmd = new SqlCommand(sql, conn);
        //     cmd.Parameters.AddWithValue("@UserLevel", userLevel);

        //     if (!string.IsNullOrEmpty(category))
        //         cmd.Parameters.AddWithValue("@Category", category);

        //     using var reader = cmd.ExecuteReader();
        //     while (reader.Read())
        //     {
        //         documents.Add(new Document
        //         {
        //             Id = (int)reader["Id"],
        //             Title = reader["Title"].ToString()!,
        //             Category = reader["Category"].ToString()!,
        //             AccessLevel = reader["AccessLevel"].ToString()!
        //         });
        //     }

        //     return documents;
        // }







        // public List<Document> GetAccessibleDocuments(string userRole, string? category, string sort = "date")
        // {
        //     int userLevel = AccessHelper.GetRoleLevel(userRole);
        //     var documents = new List<Document>();

        //     using var conn = new SqlConnection(_connectionString);
        //     conn.Open();

        //     string orderBy = sort switch
        //     {
        //         "popular" => "DownloadCount DESC",
        //         _ => "UploadDate DESC"
        //     };

        //     var sql = $@"
        //         SELECT Id, Title, Category, AccessLevel, DownloadCount, UploadDate
        //         FROM Documents
        //         WHERE
        //             CASE AccessLevel
        //                 WHEN 'standard' THEN 1
        //                 WHEN 'premium' THEN 2
        //                 WHEN 'vip' THEN 3
        //             END <= @UserLevel
        //     ";

        //     if (!string.IsNullOrEmpty(category))
        //         sql += " AND Category = @Category";

        //     sql += $" ORDER BY {orderBy}";

        //     var cmd = new SqlCommand(sql, conn);
        //     cmd.Parameters.AddWithValue("@UserLevel", userLevel);

        //     if (!string.IsNullOrEmpty(category))
        //         cmd.Parameters.AddWithValue("@Category", category);

        //     using var reader = cmd.ExecuteReader();
        //     while (reader.Read())
        //     {
        //         documents.Add(new Document
        //         {
        //             Id = (int)reader["Id"],
        //             Title = reader["Title"].ToString()!,
        //             Category = reader["Category"].ToString()!,
        //             AccessLevel = reader["AccessLevel"].ToString()!,
        //             DownloadCount = (int)reader["DownloadCount"],
        //             UploadDate = (DateTime)reader["UploadDate"]
        //         });
        //     }

        //     return documents;
        // }

        public List<Document> GetAccessibleDocuments(
        string userRole,
        string? category,
        string sort = "date",
        int page = 1,
        int pageSize = 5)
        {
            int userLevel = AccessHelper.GetRoleLevel(userRole);
            var documents = new List<Document>();

            int offset = (page - 1) * pageSize;

            string orderBy = sort switch
            {
                "popular" => "DownloadCount DESC",
                _ => "UploadDate DESC"
            };

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var sql = $@"
                SELECT Id, Title, Description, FileName, Category, AccessLevel, DownloadCount, UploadDate
                FROM Documents
                WHERE
                    CASE AccessLevel
                        WHEN 'standard' THEN 1
                        WHEN 'premium' THEN 2
                        WHEN 'vip' THEN 3
                    END <= @UserLevel
            ";

            if (!string.IsNullOrEmpty(category))
                sql += " AND Category = @Category";

            sql += $@"
                ORDER BY {orderBy}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
            ";

            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserLevel", userLevel);
            cmd.Parameters.AddWithValue("@Offset", offset);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            if (!string.IsNullOrEmpty(category))
                cmd.Parameters.AddWithValue("@Category", category);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // documents.Add(new Document
                // {
                //     Id = (int)reader["Id"],
                //     Title = reader["Title"].ToString()!,
                //     Category = reader["Category"].ToString()!,
                //     AccessLevel = reader["AccessLevel"].ToString()!,
                //     DownloadCount = (int)reader["DownloadCount"],
                //     UploadDate = (DateTime)reader["UploadDate"]
                // });
                documents.Add(new Document
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"].ToString()!,
                    Description = reader["Description"].ToString()!,
                    FileName = reader["FileName"].ToString()!,
                    Category = reader["Category"].ToString()!,
                    AccessLevel = reader["AccessLevel"].ToString()!,
                    DownloadCount = (int)reader["DownloadCount"],
                    UploadDate = (DateTime)reader["UploadDate"]
                });
            }

            return documents;
        }


        public int CountAccessibleDocuments(string userRole, string? category)
        {
            int userLevel = AccessHelper.GetRoleLevel(userRole);

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var sql = @"
                SELECT COUNT(*)
                FROM Documents
                WHERE
                    CASE AccessLevel
                        WHEN 'standard' THEN 1
                        WHEN 'premium' THEN 2
                        WHEN 'vip' THEN 3
                    END <= @UserLevel
            ";

            if (!string.IsNullOrEmpty(category))
                sql += " AND Category = @Category";

            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserLevel", userLevel);

            if (!string.IsNullOrEmpty(category))
                cmd.Parameters.AddWithValue("@Category", category);

            return (int)cmd.ExecuteScalar();
        }





    }
}
