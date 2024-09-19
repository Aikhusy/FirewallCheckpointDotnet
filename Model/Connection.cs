using System;
using System.Collections.Generic;
using System.Data.Odbc;

//==== Connection.cs ====
// file yang digunakan untuk menghubungkan sistem informasi dengan database.
// serta digunakan untuk menyimpan function untuk memasukkan data transaksi ke database.
namespace Firewall
{
    public class Connection : IConnection
    {
        private Random random = new Random();
        // bisa di tambahkan import configs

        private readonly IJsonReader _JsonReader;
        public Connection(IJsonReader jsons)
        {
            _JsonReader = jsons;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

        }

        public string GetConnectionString()
        {
            IDB config = _JsonReader.ReadDatabaseJsonConfig("Config.json");
            
            return $"Driver={config.Driver};Server={config.Server};Database={config.Database};Uid={config.UID};Pwd={config.PWD};Encrypt={config.Encrypt};TrustServerCertificate={config.TrustServerCertificate};";
        }

        private readonly Dictionary<string, string> SelectQueries = new Dictionary<string, string>
        {
            { "firewall_login", @"
                SELECT fk_m_firewall, fw_ip_address, fw_username, fw_password, fw_expert_password 
                FROM tbl_r_firewall_login 
                WHERE deleted_at IS NULL
            " },
            { "last_id", @"
                SELECT id from tbl_m_run_token where run_token = ?
            " },
            {
                "fw_name",@"
                select fw_name from tbl_m_firewall where id = ?"
            },
        };

        private readonly Dictionary<string, string> InsertQueries = new Dictionary<string, string>
        {
            { "uptime", @"
                INSERT INTO dbo.tbl_t_firewall_uptime
                (fk_m_firewall, fk_m_run_token, fw_current_time, fw_days_uptime, fw_uptime, fw_number_of_users, fw_load_avg_1_min, fw_load_avg_5_min, fw_load_avg_15_min) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
            " },
            { "memory", @"
                INSERT INTO dbo.tbl_t_firewall_memory
                (fk_m_firewall, fk_m_run_token, mem_type, mem_total, mem_used, mem_free, mem_shared, mem_cache, mem_available) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
            " },
            { "diskspace", @"
                INSERT INTO dbo.tbl_t_firewall_diskspace 
                (fk_m_firewall, fk_m_run_token, fw_filesystem, fw_mounted_on, fw_total, fw_available, fw_used, fw_used_percentage) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            " },
            { "cpu", @"
                INSERT INTO dbo.tbl_t_firewall_cpu 
                (fk_m_firewall, fk_m_run_token, fw_cpu_user_time_percentage, fw_cpu_system_time_percentage, fw_cpu_idle_time_percentage, fw_cpu_usage_percentage, fw_cpu_queue_length, fw_cpu_interrupt_per_sec, fw_cpu_number)
                VALUES (?,?,?,?,?,?,?,?,?)
            " },
            { "raid", @"
                INSERT INTO dbo.tbl_t_firewall_raid
                (fk_m_firewall, fk_m_run_token, raid_volume_id, raid_level, raid_number_of_disks, raid_size, raid_state, raid_flag) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            " },
            { "rxtx", @"
                INSERT INTO dbo.tbl_t_firewall_rxtx
                (fk_m_firewall, fk_m_run_token, interface, hwaddr, inet_addr, bcast, mask, mtu, metric, 
                rx_packets, rx_errors, rx_dropped, rx_overruns, rx_frame, 
                tx_packets, tx_errors, tx_dropped, tx_overruns, tx_carrier, 
                collisions, txqueuelen, rx_bytes, rx_human_readable, tx_bytes, 
                tx_human_readable) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            " },
            { "licenses_status", @"
                INSERT INTO dbo.tbl_t_firewall_license
                (fk_m_firewall, fk_m_run_token, fw_license_host, fw_license_expiration, fw_license_feature) 
                VALUES (?, ?, ?, ?, ?)
            " },
            { "licenses_contract", @"
                INSERT INTO dbo.tbl_t_firewall_contract
                (fk_m_firewall, fk_m_run_token, fw_contract_expiration, fw_contract_total)
                VALUES(?,?,?,?)
            " },
            { "hotfix", @"
                INSERT INTO dbo.tbl_t_firewall_hotfix
                (fk_m_firewall, fk_m_run_token, fw_kernel, fw_build_number) 
                VALUES(?,?,?,?)
            " },
            { "failed_memory", @"
                INSERT INTO dbo.tbl_t_firewall_failed_memory
                (fk_m_firewall, fk_m_run_token, fw_total_memory, fw_peak_memory, fw_total_alloc, fw_failed_alloc, fw_total_free, fw_failed_free)
                VALUES(?,?,?,?,?,?,?,?)
            " },
            { "capacity_optimisation", @"
                INSERT INTO dbo.tbl_t_firewall_capacity_optimisation
                (fk_m_firewall, fk_m_run_token, fw_hostname, fw_names, fw_id, fw_vals, fw_peaks, fw_slinks, fw_limit)
                VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?)
            " },
            { "cluster_xl", @"
                INSERT INTO dbo.tbl_t_firewall_clusterxl
                (fk_m_firewall, fk_m_run_token, fw_sync_mode, fw_ip_address, fw_load, fw_state, fw_name)
                VALUES (?, ?, ?, ?, ?, ?, ?)
            " },
            {"get_token", @"
                INSERT INTO dbo.tbl_m_run_token (run_token,run_type) 
                VALUES ( ?,?)
            "
            },

            {"upsert_content",@"
                UPDATE tbl_t_firewall_current_status
                    SET 
                        fk_m_run_token = ?,
                        uptime = ?,
                        fwtmp = ?,
                        varloglog = ?,
                        ram = ?,
                        swap = ?,
                        memory_error = ?,
                        cpu = ?,
                        rx_error_total = ?,
                        tx_error_total = ?,
                        sync_mode = ?,
                        sync_state = ?,
                        license_expiration_status = ?,
                        raid_state = ?,
                        hotfix_module = ?
                    WHERE fk_m_firewall = ?
            "
            },

            {"check_content",@"SELECT 1 FROM tbl_t_firewall_current_status WHERE fk_m_firewall = ?"
            },

            {"new_content",@"INSERT INTO tbl_t_firewall_current_status (fk_m_firewall, fk_m_run_token, uptime, fwtmp, varloglog, ram, swap, memory_error, cpu, rx_error_total, tx_error_total, sync_mode, sync_state, license_expiration_status, raid_state, hotfix_module)VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
            }
        };

        public void ExecuteInsertQuery(string queryName, params object[] parameters)
        {
            if (!InsertQueries.ContainsKey(queryName))
            {
                throw new ArgumentException("Query not found: " + queryName);
            }

            string query = InsertQueries[queryName];

            using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Add parameters to the command
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@param{i}", parameters[i]);
                        }

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }//✔️

        public List<Dictionary<string, object>> ExecuteSelectQuery(string queryName, OdbcConnection connection)
        {

            var results = new List<Dictionary<string, object>>();

            if (!SelectQueries.ContainsKey(queryName))
            {
                throw new ArgumentException("Query not found: " + queryName);
            }

            string query = SelectQueries[queryName];

            // Use the provided query
            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                using (OdbcDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();

                        // Loop through each column
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object columnValue = reader.GetValue(i);
                            row[columnName] = columnValue;
                        }

                        results.Add(row);
                    }
                }
            }


            return results;
        }

        public int GetToken()
        {
            string random = RandomString(12);
            string query = InsertQueries["get_token"];
            using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    // Use the provided query
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@run_token", random);
                        command.Parameters.AddWithValue("@run_type", "DailyRun");
                        command.ExecuteNonQuery();
                    }

                    string lastId = SelectQueries["last_id"];

                    using (OdbcCommand idCommand = new OdbcCommand(lastId, connection))
                    {

                        idCommand.Parameters.AddWithValue("@run_token", random);
                        object result = idCommand.ExecuteScalar();

                        if (result != null)
                        {

                            int insertedId = Convert.ToInt32(result);

                            return insertedId;
                        }

                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return 1;

        }//✔️

        public string GetFirewallName(int parameter)
        {
            string query = SelectQueries["fw_name"];

            using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    using (OdbcCommand idCommand = new OdbcCommand(query, connection))
                    {

                        idCommand.Parameters.AddWithValue("@id", parameter);
                        object result = idCommand.ExecuteScalar();

                        if (result != null)
                        {

                            string insertedId = (string)result;

                            return insertedId;
                        }

                    }
                    return "gaada";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return "offset";
        }//✔️

        public void InsertUptimeData(int firewallId, int runToken, string currentTime, int days, string uptime, int users, double load1, double load5, double load15)
        {
            ExecuteInsertQuery("uptime", firewallId, runToken, currentTime, days, uptime, users, load1, load5, load15);
        }//✔️

        public void InsertMemoryData(int firewallId, int runToken, string memType, double memTotal, double memUsed, double memFree, double memShared, double memCache, double memAvailable)
        {
            ExecuteInsertQuery("memory", firewallId, runToken, memType, memTotal, memUsed, memFree, memShared, memCache, memAvailable);
        }//✔️

        public void InsertDiskSpaceData(int firewallId, int runToken, string filesystem, string mountedOn, double total, double available, double used, double usedPercentage)
        {
            ExecuteInsertQuery("diskspace", firewallId, runToken, filesystem, mountedOn, total, available, used, usedPercentage);
        }//✔️

        public void InsertCpuData(int firewallId, int runToken, double cpuUserTimePercentage, double cpuSystemTimePercentage, double cpuIdleTimePercentage, double cpuUsagePercentage, double cpuQueueLength, double cpuInterruptPerSec, int cpuNumber)
        {
            ExecuteInsertQuery("cpu", firewallId, runToken, cpuUserTimePercentage, cpuSystemTimePercentage, cpuIdleTimePercentage, cpuUsagePercentage, cpuQueueLength, cpuInterruptPerSec, cpuNumber);
        }//✔️

        public void InsertRaidData(int firewallId, int runToken, string raidVolumeId, string raidLevel, int raidNumberOfDisks, string raidSize, string raidState, string raidFlag)
        {
            ExecuteInsertQuery("raid", firewallId, runToken, raidVolumeId, raidLevel, raidNumberOfDisks, raidSize, raidState, raidFlag);
        }//✔️

        public void InsertRxtxData(int firewallId, int runToken, string? @interface, string? hwaddr, string? inetAddr, string? bcast, string? mask, int mtu, int metric, int rxPackets, int rxErrors, int rxDropped, int rxOverruns, int rxFrame, int txPackets, int txErrors, int txDropped, int txOverruns, int txCarrier, int collisions, int txqueuelen, long rxBytes, string rxHumanReadable, long txBytes, string txHumanReadable)
        {
            ExecuteInsertQuery("rxtx", firewallId, runToken, @interface, hwaddr, inetAddr, bcast, mask, mtu, metric, rxPackets, rxErrors, rxDropped, rxOverruns, rxFrame, txPackets, txErrors, txDropped, txOverruns, txCarrier, collisions, txqueuelen, rxBytes, rxHumanReadable, txBytes, txHumanReadable);
        }

        public void InsertLicenseStatusData(int firewallId, int runToken, string licenseHost, DateTime licenseExpiration, string licenseFeature)
        {
            ExecuteInsertQuery("licenses_status", firewallId, runToken, licenseHost, licenseExpiration, licenseFeature);
        }

        public void InsertLicenseContractData(int firewallId, int runToken, string contractExpiration, int contractTotal)
        {
            ExecuteInsertQuery("licenses_contract", firewallId, runToken, contractExpiration, contractTotal);
        }//✔️

        public void InsertHotfixData(int firewallId, int runToken, string kernel, string buildNumber)
        {
            ExecuteInsertQuery("hotfix", firewallId, runToken, kernel, buildNumber);
        }//✔️

        public void InsertFailedMemoryData(int firewallId, int runToken, double totalMemory, double peakMemory, double totalAlloc, double failedAlloc, double totalFree, double failedFree)
        {
            ExecuteInsertQuery("failed_memory", firewallId, runToken, totalMemory, peakMemory, totalAlloc, failedAlloc, totalFree, failedFree);
        }//✔️

        public void InsertCapacityOptimisationData(int firewallId, int runToken, string hostname, string names, string id, string vals, string peaks, string slinks, string limit)
        {
            ExecuteInsertQuery("capacity_optimisation", firewallId, runToken, hostname, names, id, vals, peaks, slinks, limit);
        }

        public void InsertClusterXlData(int firewallId, int runToken, string syncMode, string ipAddress, string load, string state, string name)
        {
            ExecuteInsertQuery("cluster_xl", firewallId, runToken, syncMode, ipAddress, load, state, name);
        }//✔️

        public void InsertTokenData(string runToken, string runType)
        {
            ExecuteInsertQuery("get_token", runToken, runType);
        }//✔️

        public void UpsertTable(Dictionary<string, object> inputData, int fw_id, int token)
        {
            using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    string seccondQuery;
                    string query = InsertQueries["check_content"];
                    using (OdbcCommand command = new OdbcCommand(query, connection))
                    {
                        // Add parameters for the check query
                        command.Parameters.AddWithValue("@fk_m_firewall", fw_id);

                        object rowExists = command.ExecuteScalar();
                        Console.WriteLine("debug");
                        if (rowExists != null && (int)rowExists == 1)  // Adjust condition based on your logic
                        {
                            // Update if exists
                            seccondQuery = InsertQueries["upsert_content"];
                        }
                        else
                        {
                            // Insert if not exists
                            seccondQuery = InsertQueries["new_content"];
                        }

                    }



                    using (OdbcCommand command = new OdbcCommand(seccondQuery, connection))
                    {
                        Console.WriteLine("tatatara");
                        // Add all parameters for the insert/update query

                        command.Parameters.AddWithValue("@fk_m_run_token", token);
                        command.Parameters.AddWithValue("@uptime", inputData["uptime"]);
                        command.Parameters.AddWithValue("@fwtmp", inputData["fwtmp"]);
                        command.Parameters.AddWithValue("@varloglog", inputData["varloglog"]);
                        command.Parameters.AddWithValue("@memory", inputData["mem"]);
                        command.Parameters.AddWithValue("@swap", inputData["swap"]);
                        command.Parameters.AddWithValue("@memory_error", inputData["memory_failed"]);
                        command.Parameters.AddWithValue("@cpu", inputData["cpu"]);
                        command.Parameters.AddWithValue("@rx_error_total", inputData["rx_error_total"]);
                        command.Parameters.AddWithValue("@tx_error_total", inputData["tx_error_total"]);
                        command.Parameters.AddWithValue("@sync_mode", inputData["sync_mode"]);
                        command.Parameters.AddWithValue("@sync_state", inputData["sync_state"]);
                        command.Parameters.AddWithValue("@license_expiration_status", inputData["licence_status"]);
                        command.Parameters.AddWithValue("@raid_state", inputData["raid_state"]);
                        command.Parameters.AddWithValue("@hotfix_module", inputData["hotfix"]);
                        command.Parameters.AddWithValue("@fk_m_firewall", fw_id);
                        // Execute the insert or update
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }


    }
}
