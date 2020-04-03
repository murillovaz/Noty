using Noty.Interfaces;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task ExecuteBulkyInsert<TDataReader>(string tableName, int timeout, int batchSize, TDataReader dataReader, CancellationToken cancellationToken) where TDataReader : DbDataReader
        {
            using (var connection = await CreateAndOpenSqlConnection())
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {

                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BulkCopyTimeout = timeout;
                    bulkCopy.BatchSize = timeout;
                    bulkCopy.NotifyAfter = batchSize;

                    await bulkCopy.WriteToServerAsync(dataReader, cancellationToken);
                }
            }
        }
    }
}
