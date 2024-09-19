using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Odbc;
using Renci.SshNet;
using System.IO;
using System.Text.RegularExpressions;

namespace Firewall
{

    class Program
    {
        private static void FlushShellStream(ShellStream stream)
        {
            while (stream.DataAvailable)
            {
                // Read remaining data in the stream
                string output = stream.Read();
                Console.WriteLine(output);
            }
        }

        private static string ReadShellStream(ShellStream stream)
        {
            var output = new StringBuilder();
            string line;

            // Loop to read all available lines from the stream
            while ((line = stream.ReadLine(TimeSpan.FromSeconds(1))) != null)
            {
                output.AppendLine(line);
            }

            return output.ToString();
        }
        static async Task Main(string[] args)

        {
            var serviceProvider = new ServiceCollection()
            .AddTransient<IJsonReader, JsonReader>()
            .AddTransient<IConnection, Connection>()
            .AddTransient<IFirewall, Firewall>()
            .AddTransient<IEncrypt, Encrypt>()
            .BuildServiceProvider();

            // Resolve instance of ILampuMobil
            var JsonReader = serviceProvider.GetService<IJsonReader>();
            var Connection = serviceProvider.GetService<IConnection>();
            var Firewall = serviceProvider.GetService<IFirewall>();
            var Encrypt = serviceProvider.GetService<IEncrypt>();

            if (JsonReader == null || Connection == null || Firewall == null || Encrypt == null)
            {
                Console.WriteLine("One or more dependencies are not resolved.");
                return;
            }

            using (OdbcConnection connection = new OdbcConnection(Connection.GetConnectionString()))
            {
                connection.Open();
                var fwData = Firewall.GetFwData(connection);
                foreach (var row in fwData)
                {

                    string ipAddress = (string)row["fw_ip_address"];
                    string username = (string)row["fw_username"];
                    string password = Encrypt.DecryptPassword((string)row["fw_password"]);
                    string expertPassword = Encrypt.DecryptPassword((string)row["fw_expert_password"]);
                    string firewallName = Connection.GetFirewallName(connection, (int)row["fk_m_firewall"]);
                    int port = 1982;

                    Console.WriteLine(ipAddress);


                    using (var client = new SshClient(ipAddress, port, username, password))
                    {
                        try
                        {
                            client.Connect();
                            Console.WriteLine("Connected to SSH server");

                            // Create a ShellStream for sending and receiving data interactively
                            var stream = client.CreateShellStream("customShell", 80, 24, 800, 600, 1024);

                            // Send a command that requires a password
                            stream.WriteLine("expert");
                            await Task.Delay(500);  // Small delay to wait for prompt

                            if (stream.Expect(new Regex("Enter expert password:"), TimeSpan.FromSeconds(5)) != null)
                            {
                                stream.WriteLine(expertPassword); 
                                Console.WriteLine(Shells.GetUptime(stream));
                            }
                            // Read response (this is where you would expect a password prompt)

                            // Proceed with other commands

                            
                            FlushShellStream(stream);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                        finally
                        {
                            client.Disconnect();
                            Console.WriteLine("Disconnected from SSH server");
                        }
                    }


                }
            }
        }

    }
}
