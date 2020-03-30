using Noty.Interfaces;
using System.Data.SqlClient;

namespace Noty.SqlServer
{
    public class SqlServerContext : DataContextCore<SqlCommand, SqlConnection, SqlDataReader>
    {
        public SqlServerContext(string connectionString) : base(connectionString)
        {
        }

        public SqlServerContext(IContextConfiguration config) : base(config)
        {
        }
    }
}
