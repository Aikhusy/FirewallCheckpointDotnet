using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Odbc;
using Renci.SshNet;
using System.IO;
using System.Text.RegularExpressions;
using Renci.SshNet.Security;
using System.Data;

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
            .AddTransient<IRegexs, Regexs>()
            .AddTransient<IShells, Shells>()
            .AddTransient<INotif, Notif>()
            .BuildServiceProvider();

            // Resolve instance of ILampuMobil
            var JsonReader = serviceProvider.GetService<IJsonReader>();
            var Connection = serviceProvider.GetService<IConnection>();
            var Firewall = serviceProvider.GetService<IFirewall>();
            var Encrypt = serviceProvider.GetService<IEncrypt>();
            var Regexs = serviceProvider.GetService<IRegexs>();
            var Shells = serviceProvider.GetService<IShells>();
            var Notif = serviceProvider.GetService<INotif>();

            StringBuilder messages = new StringBuilder();
            if (JsonReader == null || Connection == null || Firewall == null || Encrypt == null)
            {
                Console.WriteLine("One or more dependencies are not resolved.");
                return;
            }

            using (OdbcConnection connection = new OdbcConnection(Connection.GetConnectionString()))
            {
                connection.Open();
                var fwData = Firewall.GetFwData(connection);
                long token;

                if (Regexs.GetDetailedSave() == true)
                {
                    token = Connection.GetToken(connection);
                }
                else
                {

                    token = 1;

                }

                foreach (var row in fwData)
                {
                    // foreach (var key in row.Keys)
                    // {
                    //     
                    // }
                    // Console.WriteLine(row["fk_m_firewall"].GetType());
                    string ipAddress = (string)row["fw_ip_address"];
                    string username = (string)row["fw_username"];
                    string password = Encrypt.DecryptPassword((string)row["fw_password"]);
                    string expertPassword = Encrypt.DecryptPassword((string)row["fw_expert_password"]);
                    string firewallName = Connection.GetFirewallName(connection, (long)(row["fk_m_firewall"]));

                    int port = 1982;
                    long id = (long)row["fk_m_firewall"];

                    using (var client = new SshClient(ipAddress, port, username, password))
                    {
                        try
                        {
                            client.Connect();
                            Console.WriteLine("Connected to SSH server firewall " + firewallName);

                            // Create a ShellStream for sending and receiving data interactively
                            var stream = client.CreateShellStream("customShell", 80, 24, 800, 600, 1024);

                            // Send a command that requires a password
                            stream.WriteLine("expert");
                            await Task.Delay(500);  // Small delay to wait for prompt

                            if (stream.Expect(new Regex("Enter expert password:"), TimeSpan.FromSeconds(5)) != null)
                            {
                                stream.WriteLine(expertPassword);
                                string upt;
                                Dictionary<string, string> upts;
                                while (true)
                                {
                                    upt = Shells.GetUptime(stream);
                                    // Console.WriteLine(upt);
                                    upts = Regexs.RegexUptime(connection, upt, id, token);
                                    if (upt != null)
                                    {
                                        break;
                                    }
                                }


                                string ram = Shells.GetRam(stream);
                                // Console.WriteLine(upt);
                                Dictionary<string, double> rams = Regexs.RegexMemory(connection, ram, id, token);

                                string disk = Shells.GetDisk(stream);
                                // Console.WriteLine(disk);
                                Dictionary<string, string> disks = Regexs.RegexDisk(connection, disk, id, token);

                                string cpu = Shells.GetCpu(stream);
                                // Console.WriteLine(cpu);
                                string cpus = Regexs.RegexCpu(connection, cpu, id, token);

                                string raid = Shells.GetRaid(stream);
                                // Console.WriteLine(raid);
                                string raids = Regexs.RegexRaid(connection, raid, id, token);

                                string rxtx = Shells.GetRxTx(stream);
                                // Console.WriteLine(rxtx);
                                Dictionary<string, int> rxtxs = Regexs.RegexRxTx(connection, rxtx, id, token);

                                string license = Shells.GetLicense(stream);
                                // Console.WriteLine(license);
                                string licences = Regexs.RegexExpiration(connection, license, id, token);

                                string hotfix = Shells.GetHotfix(stream);
                                // Console.WriteLine(hotfix);
                                string hotfixs = Regexs.RegexHotfix(connection, hotfix, id, token);

                                string memoryError = Shells.GetMemoryError(stream);
                                // Console.WriteLine(memoryError);

                                int memoryErrors = Regexs.RegexFailedMemory(connection, memoryError, id, token);

                                string capacitySlinks = Shells.GetCapacityOptimisation(stream);
                                string capacityLimit = Shells.GetCapacityLimit(stream);
                                string cor = Regexs.RegexCapacityOptimisationRemark(connection, capacitySlinks, capacityLimit, id, token);

                                string syncMode = Shells.GetSyncMode(stream);
                                string syncState = Shells.GetSyncState(stream);

                                // if (id == 1)
                                // {
                                //     degubs debug = new degubs();
                                //     syncMode = debug.BiggerData;
                                //     syncState = debug.BiggerData;
                                // }

                                string time = upts["uptime"];
                                string[] splitTime = time.Split(':');

                                Dictionary<string, string> sync = Regexs.RegexSyncMode(connection, syncState, syncMode, id, token);

                                Dictionary<string, object> upsert = new Dictionary<string, object>
                                {
                                    {"uptime",upts["days"]+ " days " + splitTime[0] + " Hours " + splitTime[1] +" minutes "},
                                    {"fwtmp",disks["fwtmp"]},
                                    {"varloglog",disks["varloglog"]},
                                    {"mem",rams["mem"]},
                                    {"swap",rams["swap"]},
                                    {"memory_failed",memoryErrors},
                                    {"cpu",cpus},
                                    {"rx_error_total",rxtxs["rx_error"]},
                                    {"tx_error_total",rxtxs["tx_error"]},
                                    {"sync_mode",sync["sync_mode"]},
                                    {"sync_state",sync["sync_state"]},
                                    {"licence_status",licences},
                                    {"raid_state",raids},
                                    {"hotfix",hotfixs},
                                };
                                Connection.UpsertTable(connection, upsert, id, token);

                                messages.Append(Notif.CheckDataStatus(upsert, firewallName));

                            }

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

            await Notif.RunBot(messages);

        }

    }
}
