using System;
using System.Collections.Generic;
using System.Threading.Tasks; // For Task.Delay
using Microsoft.Extensions.DependencyInjection;
using System.Data.Odbc;

namespace Firewall
{
    class Program
    {
        static async Task Main(string[] args)

        {
            var serviceProvider = new ServiceCollection()
            .AddTransient<IJsonReader, JsonReader>()
            .AddTransient<IConnection, Connection>()
            .AddTransient<IFirewall, Firewall>()
            .BuildServiceProvider();

            // Resolve instance of ILampuMobil
            var JsonReader = serviceProvider.GetService<IJsonReader>();
            var Connection = serviceProvider.GetService<IConnection>();
            var Firewall = serviceProvider.GetService<IFirewall>();

            var fwData = Firewall.GetFwData();
            foreach (var row in fwData)
            {

                Console.WriteLine((string)row["fw_ip_address"]);

            }
            using (OdbcConnection connection = new OdbcConnection(Connection.GetConnectionString()))
            { 

                connection.Open();

            }
        }

    }
}
