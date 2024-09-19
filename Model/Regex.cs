using System;
using System.Collections.Generic; // You need this for Dictionary
using System.Text.RegularExpressions;
using Azure.Identity;

//==== Regex.cs ====
// file yang berisi regular expression untuk membaca data yang telah didapatkan dan menyimpannya kedalam database
namespace DotNetBelajar.Model
{
    public static class Regexs
    {
        private const bool detailedSave = true; // digunakan untuk status apakah detail data akan disimpan di database

        public static int parseSecond(int days, string hourSecond)
        {
            var timeParts = hourSecond.Split(':');

            int dayInSeconds = days * 24 * 60 * 60; // 1 hari = 24 jam * 60 menit * 60 detik

            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);

            int timeInSeconds = (hours * 60 * 60) + (minutes * 60);

            int totalSeconds = dayInSeconds + timeInSeconds;

            return totalSeconds;
        }

        //untuk Casting data ke double atau default = 0
        private static double ParseDouble(string value)
        {
            double result;
            return double.TryParse(value, out result) ? result : 0;
        }

        //untuk casting tipe data ke int atau default = 0
        private static int ParseInt(string value)
        {
            int result;
            return int.TryParse(value, out result) ? result : 0;
        }

        //untuk casting tipe data ke long atau default = 0
        private static long ParseLong(string value)
        {
            long result;
            return long.TryParse(value, out result) ? result : 0;
        }

        //untuk menyimpan nilai regex yang digunakan untuk membaca data 
        public static Dictionary<string, Regex> Patterns { get; } = new Dictionary<string, Regex>
        {
            { "uptime_default", new Regex(
                @"(?<current_time>\d{2}:\d{2}:\d{2})\s+up\s+(?<days>\d+)\s+days?,\s+(?<uptime>\d{1,2}:\d{2}),\s+(?<users>\d+)\s+user[s]?,\s+load\s+average:\s+(?<load1>\d+\.\d+),\s+(?<load5>\d+\.\d+),\s+(?<load15>\d+\.\d+)"
            )}, // untuk mendapatkan nilai uptime jika format jam= jam:menit, cth(12:20) ->12 jam 20 menit.

            { "uptime_alt", new Regex(
                @"(?<current_time>\d{2}:\d{2}:\d{2})\s+up\s+(?<days>\d+)\s+days?,\s+(?<minutes>\d+)\s+min,\s+(?<users>\d+)\s+user[s]?,\s+load\s+average:\s+(?<load1>\d+\.\d+),\s+(?<load5>\d+\.\d+),\s+(?<load15>\d+\.\d+)"
            )}, // untuk mendapatkan nilai uptime jika format jam= min, cth(25 min) -> 0 jam 25 menit. 

            { "memory", new Regex(
                @"(?<type>\w+):\s+(?<total>\d+)\s+(?<used>\d+)\s+(?<free>\d+)\s+(?<shared>\d+)?\s*(?<buff_cache>\d+)?\s*(?<available>\d+)?"
            )},// untuk mendapatkan multiline dari data RAM dan SWAP.

            { "filesystem", new Regex(
                @"(?<filesystem>[\S]+)\s+(?<blocks>\d+)\s+(?<used>\d+)\s+(?<available>\d+)\s+(?<use_percent>\d+)%\s+(?<mount_point>\S+)"
            )},// untuk mendapatkan multiline dari data Disk space.

            { "rx_tx", new Regex(
                @"(\S+)\s+Link encap:\S+\s+HWaddr\s+([\w:]+)\s*(?:inet addr:(\d+\.\d+\.\d+\.\d+))?\s*(?:Bcast:(\d+\.\d+\.\d+\.\d+))?\s*(?:Mask:(\d+\.\d+\.\d+\.\d+))?\s*UP\s+BROADCAST(?:\s+RUNNING)?\s+MULTICAST\s+MTU:(\d+)\s+Metric:(\d+)\s*RX packets:(\d+)\s+errors:(\d+)\s+dropped:(\d+)\s+overruns:(\d+)\s+frame:(\d+)\s*TX packets:(\d+)\s+errors:(\d+)\s+dropped:(\d+)\s+overruns:(\d+)\s+carrier:(\d+)\s*collisions:(\d+)\s+txqueuelen:(\d+)\s*RX bytes:(\d+)\s+\(([\d\.]+\s+\w+)\)\s+TX bytes:(\d+)\s+\(([\d\.]+\s+\w+)\)"
            )},// untuk mendapatkan multiline dari data perangkat yang terhubung dengan checkpoint.

            { "raid", new Regex(
                @"VolumeID:(?<volume_id>\d+)\s+RaidLevel:\s*(?<raid_level>[^\s]+)\s+NumberOfDisks:(?<number_of_disks>\d+)\s+RaidSize:(?<raid_size>[^\s]+)\s+State:(?<state>[^\s]+)\s+Flags:(?<flags>[^\s]+)"
            )},// untuk mendapatkan data status raid dari firewall.

            { "device_info", new Regex(
                @"CK:\s+(?<ck>CK-[\w-]+)\s+SKU:\s+(?<sku>[\w\s-]+)\s+central:\s+(?<central>\w+)\s+expiration:\s+(?<expiration>\w+)\s+ip_addr:\s+(?<ip_addr>[\d.]+)\s+signature:\s+(?<signature>\w+)",
                RegexOptions.IgnorePatternWhitespace
            )},// in development

            { "hotfix", new Regex(
                @"kernel:\s*(R\d+(\.\d+){0,2})\s*-\s*Build\s*(\d+)"
            )},// untuk mendapatkan versi dari checkpoint firewall.

            { "cpu", new Regex(
                @"CPU User Time \(%\):\s+(?<user>\d+)\s*CPU System Time \(%\):\s+(?<system>\d+)\s*CPU Idle Time \(%\):\s+(?<idle>\d+)\s*CPU Usage \(%\):\s+(?<usage>\d+)\s*CPU Queue Length:\s+(?<queue_length>[-\d]*)\s*CPU Interrupts/Sec:\s+(?<interrupts>\d+)\s*CPUs Number:\s+(?<number>\d+)\s*"
            )},// untuk mendapatkan data CPU dari checkpoint firewall.

            { "licenses", new Regex(
                @"(?<Host>\d+\.\d+\.\d+\.\d+)\s+(?<Expiration>\w+)\s+(?<Features>.+)"
            )},// untuk mendapatkan status lisensi checkpoint firewall.

            { "contract", new Regex(
                @"(?<ID>\w+)\s+\|\s+(?<Expiration>\d+\w+\d+)\s+\|\s+(?<SKU>[\w-]+)"
            )},// in development

            { "ha_status", new Regex(
                @"Product\ name:\s*(?<product_name>[^\n]*)\nVersion:\s*(?<version>[^\n]*)\nStatus:\s*(?<status>[^\n]*)\nHA\ installed:\s*(?<ha_installed>[^\n]*)\nWorking\ mode:\s*(?<working_mode>[^\n]*)\nHA\ started:\s*(?<ha_started>[^\n]*)\n",
                RegexOptions.IgnorePatternWhitespace
            )},// in development

            { "memory_stat", new Regex(
                @"Total\s+memory\s+bytes\s+used:\s+(\d+)\s+peak:\s+(\d+)"
            )},// untuk mendapatkan data status memory, digunakan oleh FailedMemory.

            { "alloc_stat", new Regex(
                @"Allocations:\s+(\d+)\s+alloc\s+(\d+)\s+failed\s+alloc"
            )},// untuk mendapatkan data failed allocation, digunakan untuk FailedMemory.

            { "free_stat", new Regex(
                @"(\d+)\s+free\s+(\d+)\s+failed\s+free"
            )},// untuk mendapatkan data free memory, digunakan untuk FailedMemory.

            { "capacity_optimisation", new Regex(
                @"(\S+)\s+(\S+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)"
            )},// untuk mendapatkan data Vals, Peaks, Slinks, digunakan untuk CapacityOptimisationRemark.

            { "capacity_limit", new Regex(
                @"limit (\d+)|unlimited"
            )},// untuk mendapatkan data limit agressive aging, digunakan untuk CapacityOptimisationRemark.

            { "sync_mode", new Regex(
                @"\b(Unicast)\b"
            )},// untuk mendapatkan data mode cluster, digunakan untuk ClusterXL and SyncState.

            { "sync_state", new Regex(
                @"^\s*\d+\s+\(local\)\s+([\d\.]+)\s+([\d%]+)\s+(\w+)\s+(.+)", RegexOptions.Multiline
            )},// untuk mendapatkan data status Sync, digunakan untuk ClusterXL and SycnState.

            {"contract_expiration",new Regex(
                @"\b\d{2}-[A-Za-z]{3}-\d{2}\b"
            )}
        };

        //function untuk Mengolah data uptime, dan menyimpan ke database.
        public static Dictionary<string, string> RegexUptime(string inputs, int fwId, int tokenId)
        {
            // Try the first regex pattern (uptime_default)
            Match match = Patterns["uptime_default"].Match(inputs);

            // If the first pattern doesn't match, try the alternative pattern (uptime_alt)
            if (!match.Success || !match.Groups["uptime"].Success)
            {
                match = Patterns["uptime_alt"].Match(inputs);
            }

            // If a match is found
            if (match.Success)
            {
                string currentTime = match.Groups["current_time"].Value;
                string days = match.Groups["days"].Success ? match.Groups["days"].Value : "0";
                string users = match.Groups["users"].Value;
                string load1 = match.Groups["load1"].Value;
                string load5 = match.Groups["load5"].Value;
                string load15 = match.Groups["load15"].Value;
                string uptime;


                if (match.Groups["uptime"].Success)
                {
                    uptime = match.Groups["uptime"].Value;
                }


                else
                {
                    uptime = match.Groups["minutes"].Success ? $"0:{match.Groups["minutes"].Value}" : "00:00";
                }

                if (detailedSave)
                {
                    Connection.InsertUptimeData(fwId, tokenId, currentTime, int.Parse(days), uptime, int.Parse(users), double.Parse(load1), double.Parse(load5), double.Parse(load15));
                }

                return new Dictionary<string, string>
                {

                    { "uptime", uptime },
                    { "days", days }

                };
            }

            // Return default values if no match was found
            return new Dictionary<string, string>
            {
                { "uptime", "00:00" },
                { "days", "0" }
            };
        }

        //function untuk mengolah data RAM dan SWAP, dan menyimpan ke database.
        public static Dictionary<string, double> RegexMemory(string inputs, int fwId, int tokenId)
        {
            var matches = Patterns["memory"].Matches(inputs);
            double swap = 0;
            double ram = 0;

            foreach (Match match in matches)
            {
                var type = match.Groups["type"].Value;
                var totalStr = match.Groups["total"].Value;
                var freeStr = match.Groups["free"].Value;


                double total = ParseDouble(totalStr);
                double free = ParseDouble(freeStr);

                if (detailedSave)
                {
                    double used = ParseDouble(match.Groups["used"].Value);
                    double shared = ParseDouble(match.Groups["shared"].Value);
                    double buffCache = ParseDouble(match.Groups["buff_cache"].Value);
                    double available = ParseDouble(match.Groups["available"].Value);

                    Connection.InsertMemoryData(fwId, tokenId, type, total, used, free, shared, buffCache, available);
                }

                if (total > 0)
                {
                    double usage = 100 - (free / total * 100);
                    if (type == "Mem")
                    {
                        ram = Math.Round(usage, 3);
                    }
                    else if (type == "Swap")
                    {
                        swap = Math.Round(usage, 3);
                    }
                }
            }

            return new Dictionary<string, double>
            {
                { "mem", ram },
                { "swap", swap }
            };
        }

        //function untuk mengolah data disk (/tmp dan /logs), dan menyimpan ke database.
        public static Dictionary<string, string> RegexDisk(string inputs, int fwId, int tokenId)
        {
            var matches = Patterns["filesystem"].Matches(inputs);
            string fwtmp = "0";
            string varloglog = "0";

            foreach (Match match in matches)
            {
                var mountPoint = match.Groups["mount_point"].Value;
                var usePercent = match.Groups["use_percent"].Value;

                if (mountPoint == "/" || mountPoint == "/fwtmp" || mountPoint == "/fwtmps")
                {
                    fwtmp = usePercent;
                }

                if (mountPoint == "/var/log" || mountPoint == "/logs" || mountPoint == "/log" || mountPoint == "/var/logs")
                {
                    varloglog = usePercent;
                }

                if (detailedSave)
                {
                    double blocks = ParseDouble(match.Groups["blocks"].Value);
                    double available = ParseDouble(match.Groups["blocks"].Value);
                    double used = ParseDouble(match.Groups["used"].Value);
                    double use_percent = ParseDouble(match.Groups["use_percent"].Value);
                    Connection.InsertDiskSpaceData(
                        fwId,
                        tokenId,
                        match.Groups["filesystem"].Value,
                        match.Groups["mount_point"].Value,
                        blocks,
                        available,
                        used,
                        use_percent
                    );
                }
            }

            return new Dictionary<string, string>
            {
                { "varloglog", varloglog },
                { "fwtmp", fwtmp }
            };
        }

        //function untuk mengolah data Rx error dan Tx error, dan menyimpan ke database.
        public static Dictionary<string, int> RegexRxTx(string inputs, int fwId, int tokenId)
        {
            var pattern = Patterns["rx_tx"];
            var matches = pattern.Matches(inputs);
            int rxErrors = 0;
            int txErrors = 0;

            foreach (Match match in matches)
            {
                if (detailedSave)
                {
                    // Extract values and cast to appropriate types
                    string @interface = match.Groups[1].Value;
                    string hwaddr = match.Groups[2].Value;
                    string? inetAddr = match.Groups[3].Success ? match.Groups[3].Value : null;
                    string? bcast = match.Groups[4].Success ? match.Groups[4].Value : null;
                    string? mask = match.Groups[5].Success ? match.Groups[5].Value : null;
                    int mtu = ParseInt(match.Groups[6].Value);
                    int metric = ParseInt(match.Groups[7].Value);
                    int rxPackets = ParseInt(match.Groups[8].Value);
                    int rxErrorss = ParseInt(match.Groups[9].Value);
                    int rxDropped = ParseInt(match.Groups[10].Value);
                    int rxOverruns = ParseInt(match.Groups[11].Value);
                    int rxFrame = ParseInt(match.Groups[12].Value);
                    int txPackets = ParseInt(match.Groups[13].Value);
                    int txErrorss = ParseInt(match.Groups[14].Value);
                    int txDropped = ParseInt(match.Groups[15].Value);
                    int txOverruns = ParseInt(match.Groups[16].Value);
                    int txCarrier = ParseInt(match.Groups[17].Value);
                    int collisions = ParseInt(match.Groups[18].Value);
                    int txqueuelen = ParseInt(match.Groups[19].Value);
                    long rxBytes = ParseLong(match.Groups[20].Value);
                    string rxHumanReadable = match.Groups[21].Value;
                    long txBytes = ParseLong(match.Groups[22].Value);
                    string txHumanReadable = match.Groups[23].Value;

                    // Insert data into the database
                    Connection.InsertRxtxData(
                        fwId,
                        tokenId,
                        @interface,
                        hwaddr,
                        inetAddr,
                        bcast,
                        mask,
                        mtu,
                        metric,
                        rxPackets,
                        rxErrorss,
                        rxDropped,
                        rxOverruns,
                        rxFrame,
                        txPackets,
                        txErrorss,
                        txDropped,
                        txOverruns,
                        txCarrier,
                        collisions,
                        txqueuelen,
                        rxBytes,
                        rxHumanReadable,
                        txBytes,
                        txHumanReadable
                    );
                }
                // Accumulate errors
                rxErrors += ParseInt(match.Groups[9].Value);
                txErrors += ParseInt(match.Groups[14].Value);
            }

            return new Dictionary<string, int>
            {
                { "rx_error", rxErrors },
                { "tx_error", txErrors }
            };
        }

        //function untuk mengolah data Raid, dan menyimpan ke database.
        public static string RegexRaid(string inputs, int fwId, int tokenId)
        {
            // Try the first regex pattern (raid_default)
            var pattern = Patterns["raid"];
            var matches = pattern.Matches(inputs);

            string states = "Can't find state";

            foreach (Match match in matches)
            {

                var state = match.Groups["state"].Value;
                // Update the states variable with the current state
                states = state;

                // Insert data into the database
                if (detailedSave) // Ensure to check if detailedSave is enabled
                {

                    var volumeId = match.Groups["volume_id"].Value;
                    var raidLevel = match.Groups["raid_level"].Value;
                    int numberOfDisks = ParseInt(match.Groups["number_of_disks"].Value);
                    var raidSize = match.Groups["raid_size"].Value;
                    var flags = match.Groups["flags"].Value;
                    Connection.InsertRaidData(fwId, tokenId, volumeId, raidLevel, numberOfDisks, raidSize, state, flags);

                }
            }

            return states;
        }

        //function untuk mengolah data patch firewall, dan menyimpannya ke database.
        public static string RegexHotfix(string inputs, int fwId, int tokenId)
        {
            // Define the regex pattern for hotfix
            var pattern = Patterns["hotfix"];
            var matches = pattern.Matches(inputs);

            string hotfix = "No hotfix found";

            foreach (Match match in matches)
            {
                var hotfixValue = match.Groups[1].Value; // Adjust the index or name according to your pattern

                // Update the hotfix variable with the current match value
                hotfix = hotfixValue;

                // Insert data into the database
                if (detailedSave) // Ensure to check if detailedSave is enabled
                {
                    var buildNumber = match.Groups[2].Value;
                    Connection.InsertHotfixData(fwId, tokenId, hotfixValue, buildNumber);
                }
            }

            return hotfix;
        }

        //function untuk mengolah data CPU, dan menyimpannya ke database.
        public static string RegexCpu(string inputs, int fwId, int tokenId)
        {

            var pattern = Patterns["cpu"];
            var match = pattern.Match(inputs);

            if (match.Success)
            {

                // Extract and convert the CPU queue length value
                double cpuQueueLength = ParseDouble(match.Groups["queue_length"].Value);
                // Nullable integer to represent NULL in SQL


                // Insert data into the database
                if (detailedSave) // Ensure to check if detailedSave is enabled
                {
                    Connection.InsertCpuData(
                        fwId,
                        tokenId,
                        int.Parse(match.Groups["user"].Value),
                        int.Parse(match.Groups["system"].Value),
                        int.Parse(match.Groups["idle"].Value),
                        int.Parse(match.Groups["usage"].Value),
                        cpuQueueLength,  // You can also pass cpuQueueLength if you handle nulls correctly
                        int.Parse(match.Groups["interrupts"].Value),
                        int.Parse(match.Groups["number"].Value)
                    );
                }

                // Return CPU usage as a string
                return match.Groups["usage"].Value;
            }

            // Return default value if no match is found
            return "No CPU found";
        }

        //function untuk mengolah data memory gagal, dan menyimpannya ke database.
        public static int RegexFailedMemory(string inputs, int fwId, int tokenId)
        {
            // Define patterns for memory, allocation, and free statistics
            var memoryStat = Patterns["memory_stat"];
            var allocStat = Patterns["alloc_stat"];
            var freeStat = Patterns["free_stat"];

            // Initialize variables for the memory stats
            int totalMemory = 0;
            int peakMemory = 0;

            // Initialize variables for allocation stats
            int totalAlloc = 0;
            int failedAlloc = 0;

            // Initialize variables for free stats
            int totalFree = 0;
            int failedFree = 0;

            // Match the patterns in the input
            var memoryMatch = memoryStat.Match(inputs);
            var allocMatch = allocStat.Match(inputs);
            var freeMatch = freeStat.Match(inputs);

            // If memory pattern matches, extract the values
            if (memoryMatch.Success)
            {
                Console.WriteLine("memory");
                totalMemory = ParseInt(memoryMatch.Groups[1].Value);
                peakMemory = ParseInt(memoryMatch.Groups[2].Value);
            }

            // If allocation pattern matches, extract the values
            if (allocMatch.Success)
            {
                Console.WriteLine("alloc");
                totalAlloc = ParseInt(allocMatch.Groups[1].Value);
                failedAlloc = int.Parse(allocMatch.Groups[2].Value);
            }

            // If free pattern matches, extract the values
            if (freeMatch.Success)
            {
                Console.WriteLine("free");
                totalFree = ParseInt(freeMatch.Groups[1].Value);
                failedFree = int.Parse(freeMatch.Groups[2].Value);
            }
            // Insert the data into the database
            if (detailedSave) // Ensure the detailedSave flag is respected
            {
                Connection.InsertFailedMemoryData(
                    fwId,
                    tokenId,
                    totalMemory,
                    peakMemory,
                    totalAlloc,
                    failedAlloc,
                    totalFree,
                    failedFree
                );
            }

            // Return the failed allocation count
            return failedAlloc;
        }

        //function untuk mengolah data SyncMode dan Sync State, serta menyimpannya ke database.
        public static Dictionary<string, string> RegexSyncMode(string inputState, string inputMode, int fwId, int tokenId)
        {
            // Define the regex patterns for sync_mode and sync_state
            var syncModePattern = Patterns["sync_mode"];
            var syncStatePattern = Patterns["sync_state"];

            // Default values
            string syncMode = "Ha Module Not Started";
            string ipAddress = "-";
            string load = "-";
            string state = "-";
            string name = "-";

            // Search for the sync mode in the inputMode string
            var syncModeMatch = syncModePattern.Match(inputMode);

            if (syncModeMatch.Success)
            {
                syncMode = syncModeMatch.Groups[1].Value;
            }

            // Find all matches for sync state in the inputState string
            var syncStateMatches = syncStatePattern.Matches(inputState);

            foreach (Match match in syncStateMatches)
            {

                state = match.Groups[3].Value;

                // Insert the data into the database
                if (detailedSave) // Check if detailedSave is enabled
                {
                    ipAddress = match.Groups[1].Value;
                    load = match.Groups[2].Value;
                    name = match.Groups[4].Value;
                    Connection.InsertClusterXlData(fwId, tokenId, syncMode, ipAddress, load, state, name);
                }
            }

            // Return the sync mode and state in a dictionary
            return new Dictionary<string, string>
                {
                    { "sync_mode", syncMode },
                    { "sync_state", state }
                };
        }

        public static string RegexExpiration(string inputs, int fwId, int tokenId)
        {
            var contract = Patterns["contract_expiration"];
            var matches = contract.Matches(inputs);
            string result = " ";
            Dictionary<string, int> counts = new Dictionary<string, int>();
            HashSet<string> uniqueDates = new HashSet<string>(); // Inisialisasi HashSet untuk menyimpan tanggal unik

            // Mencari semua tanggal yang sesuai dengan pattern
            foreach (Match match in matches)
            {

                uniqueDates.Add(match.Value);

                // Jika tanggal belum ada di dictionary, inisialisasi dengan 1
                if (!counts.ContainsKey(match.Value))
                {
                    counts[match.Value] = 1;

                    result += match.Value;
                    result += " ";
                }
                else
                {
                    counts[match.Value] += 1;

                }


            }

            if (detailedSave)
            {
                foreach (KeyValuePair<string, int> count in counts) // Perbaikan tipe data
                {
                    // Menggunakan count.Key dan count.Value untuk akses dictionary
                    Connection.InsertLicenseContractData(fwId, tokenId, count.Key, count.Value);
                }
            }


            return result;

        }
    }
}
