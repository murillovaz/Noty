using Microsoft.Extensions.DependencyInjection;
using Noty.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noty.MySql
{
    public static class Extensions
    {
        public static void AddMySqlContext(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<MySqlContext>((x) => new MySqlContext(connectionString));
        }

        public static void AddMySqlContext(this IServiceCollection services, IContextConfiguration config)
        {
            services.AddScoped<MySqlContext>((x) => new MySqlContext(config));
        }

    }
}
