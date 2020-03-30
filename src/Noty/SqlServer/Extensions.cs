using Microsoft.Extensions.DependencyInjection;
using Noty.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noty.SqlServer
{
    public static class Extensions
    {
        public static void AddSqlServerContext(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<SqlServerContext>((x) => new SqlServerContext(connectionString));
        }

        public static void AddSqlServerContext(this IServiceCollection services, IContextConfiguration config)
        {
            services.AddScoped<SqlServerContext>((x) => new SqlServerContext(config));
        }

    }
}
