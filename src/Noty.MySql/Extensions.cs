using Microsoft.Extensions.DependencyInjection;
using Noty.Interfaces;

namespace Noty.MySql
{
    public static class Extensions
    {
        public static void AddMySqlContext(this IServiceCollection services, string connectionString)
        {
            services.AddScoped((x) => new MySqlContext(connectionString));
        }

        public static void AddMySqlContext(this IServiceCollection services, IContextConfiguration config)
        {
            services.AddScoped((x) => new MySqlContext(config));
        }

    }
}
