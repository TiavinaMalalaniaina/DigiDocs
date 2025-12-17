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
