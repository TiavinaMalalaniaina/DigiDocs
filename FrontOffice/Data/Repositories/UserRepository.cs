using FrontOffice.Data;
using FrontOffice.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace FrontOffice.Data.Repositories;

public class UserRepository
{
    private readonly DbConnectionFactory _factory;

    public UserRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public User? GetByUsername(string username)
    {
        using var conn = _factory.Create();
        conn.Open();

        var cmd = new SqlCommand(
            "SELECT * FROM Users WHERE Username = @username", conn);
        cmd.Parameters.AddWithValue("@username", username);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(2),
            Email = reader.GetString(3),
            PasswordHash = reader.GetString(4),
            Role = reader.GetString(5)
        };
    }

    public void Create(User user)
    {
        using var conn = _factory.Create();
        conn.Open();

        var cmd = new SqlCommand(@"
            INSERT INTO Users (Username, Email, PasswordHash, Role)
            VALUES (@username, @email, @password, @role)", conn);

        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password", user.PasswordHash);
        cmd.Parameters.AddWithValue("@role", user.Role);

        cmd.ExecuteNonQuery();
    }

    public static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(
            sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}
