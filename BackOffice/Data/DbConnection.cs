using System.Data;

using Microsoft.Data.SqlClient;           
using Microsoft.Extensions.Configuration;

namespace BackOffice.Data
{
    public class DbConnection
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DbConnection(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
