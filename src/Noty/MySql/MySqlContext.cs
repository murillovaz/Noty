using MySql.Data.MySqlClient;
using Noty.Interfaces;

namespace Noty.MySql
{
    public class MySqlContext : DataContextCore<MySqlCommand, MySqlConnection, MySqlDataReader>
    {
        public MySqlContext(string connectionString) : base(connectionString)
        {
        }

        public MySqlContext(IContextConfiguration config) : base(config)
        {
        }
    }
}
