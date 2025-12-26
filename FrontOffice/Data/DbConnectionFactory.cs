using Microsoft.Data.SqlClient;

namespace FrontOffice.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public SqlConnection Create()
    {
        return new SqlConnection(_connectionString);
    }
}
