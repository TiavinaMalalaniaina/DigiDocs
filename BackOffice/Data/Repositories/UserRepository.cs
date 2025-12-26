using BackOffice.Models;
using BackOffice.Data;
using BackOffice.Models.Enums;
using Microsoft.Data.SqlClient;

namespace BackOffice.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnection _db;

        public UserRepository(DbConnection db)
        {
            _db = db;
        }

        public async Task UpdateUserRole(User user)
        {
            using var conn = (SqlConnection)_db.CreateConnection();
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Users
                SET 
                    Role = @Role
                WHERE Id = @Id;
            ";
            Console.WriteLine(user.Id);
            Console.WriteLine(user.Role.ToString());
            cmd.Parameters.Add(new SqlParameter("@Role", user.Role.ToString()));
            cmd.Parameters.Add(new SqlParameter("@Id", user.Id));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> GetCountAsync()
        {
            using var conn = _db.CreateConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users";
            conn.Open();
            return (int)await ((SqlCommand)cmd).ExecuteScalarAsync();
        }

        public async Task<List<User>> GetUsersAsync(string index, int page, int pageSize)
        {
            var list = new List<User>();

            using var conn = _db.CreateConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT *
                FROM Users
                WHERE (@Index IS NULL OR
                    Username LIKE '%' + @Index + '%' OR
                    Email LIKE '%' + @Index + '%' OR
                    Role LIKE '%' + @Index + '%') AND 
                    Role <> 'admin'
                ORDER BY Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            ";

            var indexParam = cmd.CreateParameter();
            indexParam.ParameterName = "@Index";
            indexParam.Value = string.IsNullOrWhiteSpace(index)
                ? DBNull.Value
                : index;
            cmd.Parameters.Add(indexParam);

            var offsetParam = cmd.CreateParameter();
            offsetParam.ParameterName = "@Offset";
            offsetParam.Value = (page - 1) * pageSize;
            cmd.Parameters.Add(offsetParam);

            var sizeParam = cmd.CreateParameter();
            sizeParam.ParameterName = "@PageSize";
            sizeParam.Value = pageSize;
            cmd.Parameters.Add(sizeParam);

            conn.Open();

            using var reader = await ((SqlCommand)cmd).ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Role = Enum.Parse<UserRole>(reader.GetString(reader.GetOrdinal("Role")))
                });
            }

            return list;
        }

        public User Login(string email, string password)
        {
            using var con = _db.CreateConnection();
            con.Open();

            using var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id,Email,Username,Role FROM Users WHERE Email = @Email and PasswordHash = @Password and Role = @Role";
            cmd.Parameters.Add(new SqlParameter("@Email", email));
            cmd.Parameters.Add(new SqlParameter("@Password", password));
            cmd.Parameters.Add(new SqlParameter("@Role", "admin"));

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Username = reader.GetString(2),
                    Role = Enum.Parse<UserRole>(reader.GetString(3))
                };
            }

            return null;
        }

    }
}
