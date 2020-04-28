using Noty.Interfaces;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Noty.SqlServer
{
    public class SqlServerContext : DataContextCore<SqlCommand, SqlConnection, SqlDataReader, SqlDataAdapter>
    {
        public SqlServerContext(string connectionString) : base(connectionString)
        {
        }

        public SqlServerContext(IContextConfiguration config) : base(config)
        {
        }

        public async Task ExecuteBulkyInsert<TDataReader>(string tableName
            , int? timeout
            , int? batchSize
            , TDataReader dataReader
            , CancellationToken? cancellationToken
            , Action<object, SqlRowsCopiedEventArgs> sqlRowsCopiedHandler
            , int? notifyAfter
            )
            where TDataReader : DbDataReader
        {
            using (var connection = await CreateAndOpenSqlConnection())
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {

                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BulkCopyTimeout = timeout ?? 5;
                    bulkCopy.BatchSize = batchSize ?? 1000;
                    if (notifyAfter != null)
                        bulkCopy.NotifyAfter = (int)notifyAfter;
                    if(sqlRowsCopiedHandler != null)
                        bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(sqlRowsCopiedHandler);

                    if(cancellationToken == null)
                        await bulkCopy.WriteToServerAsync(dataReader);
                    else
                        await bulkCopy.WriteToServerAsync(dataReader, (CancellationToken)cancellationToken);
                }
            }
        }

        public async Task ExecuteBulkyInsert<TDataReader>(string tableName
            , int timeout
            , int batchSize
            , TDataReader dataReader
            , CancellationToken cancellationToken
            )
            where TDataReader : DbDataReader
        {
            await ExecuteBulkyInsert(tableName, timeout, batchSize, dataReader, cancellationToken, null, null);
        }

        public async Task ExecuteBulkyInsert<TDataReader>(string tableName
            , int timeout
            , int batchSize
            , TDataReader dataReader
            )
            where TDataReader : DbDataReader
        {
            await ExecuteBulkyInsert(tableName, timeout, batchSize, dataReader, null, null, null);
        }

        public async Task ExecuteBulkyInsert<TDataReader>(string tableName
            , TDataReader dataReader
            )
            where TDataReader : DbDataReader
        {
            await ExecuteBulkyInsert(tableName, null, null, dataReader, null, null, null);
        }
    }
}
