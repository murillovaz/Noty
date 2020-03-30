using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Noty.SqlServer
{
    public static class Command
    {
        private static void AddParameter<TCommand>(this TCommand sqlCommand, IEnumerable<KeyValuePair<string, object>> parameters) where TCommand : DbCommand
        {
            foreach (var item in parameters)
            {
                var parameter = sqlCommand.CreateParameter();
                parameter.ParameterName = item.Key;
                parameter.Value = item.Value;
                sqlCommand.Parameters.Add(parameter);
            }
        }

        public static TCommand CreateCommand<TConnection, TCommand>(TConnection sqlConnection, string query, CommandType commandType, IEnumerable<KeyValuePair<string, object>> parameters = null) where TConnection : DbConnection where TCommand : DbCommand
        {
            var command = (TCommand)Activator.CreateInstance(typeof(TCommand), query, sqlConnection);

            command.CommandType = commandType;
            command.CommandTimeout = 300;

            if (parameters != null)
                command.AddParameter(parameters);

            return command;
        }
    }
}
