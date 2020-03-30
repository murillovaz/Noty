using Noty.Interfaces;
using Noty.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;


namespace Noty
{
    public class DataContextCore<TCommand, TConnection, TDataReader>  where TCommand : DbCommand where TConnection : DbConnection where TDataReader : DbDataReader
    {
        private readonly string ConnectionString;

        public DataContextCore(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DataContextCore(IContextConfiguration config)
        {
            ConnectionString = config.GetConnectionString();
        }

        private async Task<IEnumerable<T>> ExecuteCollectionReader<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var sqlConnection = await CreateAndOpenSqlConnection())
                { 
                    using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                    {
                        using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            return await DataReader.MapDataToObjectCollection<T, DbDataReader>(sqlDataReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }


        private async Task<T> ExecuteReader<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var sqlConnection = await CreateAndOpenSqlConnection())
                {
                    using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                    {
                        using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            return await DataReader.MapDataToObject<T, DbDataReader>(sqlDataReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return default(T);
        }

        private async Task<T> ExecuteScalar<T>(string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            try
            {
                using (var sqlConnection = await CreateAndOpenSqlConnection())
                {
                    using (var sqlCommand = Command.CreateCommand<TConnection, TCommand>(sqlConnection, query, commandType, parameters))
                    {
                        var result = await sqlCommand.ExecuteScalarAsync();
                        return result.MapDataToObject<T>();

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return default(T);
        }

        #region Overrides
        public async Task<T> ExecuteStoredProcedureScalar<T>(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteScalar<T>(query, CommandType.StoredProcedure, parameters);
        }

        public async Task<T> ExecuteScalar<T>(string query)
        {
            return await ExecuteScalar<T>(query, CommandType.Text);
        }

        public async Task<T> ExecuteReader<T>(string query)
        {
            return await ExecuteReader<T>(query, CommandType.Text);
        }

        public async Task<T> ExecuteStoredProcedureReader<T>(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteReader<T>(query, CommandType.StoredProcedure, parameters);
        }

        public async Task<IEnumerable<T>> ExecuteCollectionReader<T>(string query)
        {
            return await ExecuteCollectionReader<T>(query, CommandType.Text);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureCollectionReader<T>(string query, params KeyValuePair<string, object>[] parameters)
        {
            return await ExecuteCollectionReader<T>(query, CommandType.StoredProcedure, parameters);
        }

        #endregion

        private async Task<TConnection> CreateAndOpenSqlConnection()
        {
            var connection = (TConnection)Activator.CreateInstance(typeof(TConnection), ConnectionString);

            await connection.OpenAsync();

            return connection;

        }
    }
}
