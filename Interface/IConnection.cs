using System;
using System.Collections.Generic;
using System.Data.Odbc;

//==== Connection.cs ====
// file yang digunakan untuk menghubungkan sistem informasi dengan database.
// serta digunakan untuk menyimpan function untuk memasukkan data transaksi ke database.
namespace Firewall
{
    public interface IConnection
    {

        public string RandomString(int length);

        string GetConnectionString();
        public void ExecuteInsertQuery(OdbcConnection connection,string queryName, params object[] parameters);
        public List<Dictionary<string, object>> ExecuteSelectQuery(string queryName,OdbcConnection connection);
        public int GetToken(OdbcConnection connection);
        public string GetFirewallName(OdbcConnection connection,long parameter);
        public void InsertUptimeData(OdbcConnection connection, long firewallId, long runToken, string currentTime, int days, string uptime, int users, double load1, double load5, double load15);
        public void InsertMemoryData(OdbcConnection connection, long firewallId, long runToken, string memType, double memTotal, double memUsed, double memFree, double memShared, double memCache, double memAvailable);

        public void InsertDiskSpaceData(OdbcConnection connection, long firewallId, long runToken, string filesystem, string mountedOn, double total, double available, double used, double usedPercentage);

        public void InsertCpuData(OdbcConnection connection, long firewallId, long runToken, double cpuUserTimePercentage, double cpuSystemTimePercentage, double cpuIdleTimePercentage, double cpuUsagePercentage, double cpuQueueLength, double cpuInterruptPerSec, int cpuNumber);

        public void InsertRaidData(OdbcConnection connection, long firewallId, long runToken, string raidVolumeId, string raidLevel, int raidNumberOfDisks, string raidSize, string raidState, string raidFlag);

        public void InsertRxtxData(OdbcConnection connection, long firewallId, long runToken, string? @interface, string? hwaddr, string? inetAddr, string? bcast, string? mask, int mtu, int metric, int rxPackets, int rxErrors, int rxDropped, int rxOverruns, int rxFrame, int txPackets, int txErrors, int txDropped, int txOverruns, int txCarrier, int collisions, int txqueuelen, long rxBytes, string rxHumanReadable, long txBytes, string txHumanReadable);

        public void InsertLicenseStatusData(OdbcConnection connection, long firewallId, long runToken, string licenseHost, DateTime licenseExpiration, string licenseFeature);

        public void InsertLicenseContractData(OdbcConnection connection, long firewallId, long runToken, string contractExpiration, int contractTotal);

        public void InsertHotfixData(OdbcConnection connection, long firewallId, long runToken, string kernel, string buildNumber);

        public void InsertFailedMemoryData(OdbcConnection connection, long firewallId, long runToken, double totalMemory, double peakMemory, double totalAlloc, double failedAlloc, double totalFree, double failedFree);
        public void InsertCapacityOptimisationData(OdbcConnection connection, long firewallId, long runToken, string hostname, string names, string id, string vals, string peaks, string slinks, string limit);

        public void InsertClusterXlData(OdbcConnection connection, long firewallId, long runToken, string syncMode, string ipAddress, string load, string state, string name);
        public void InsertTokenData(OdbcConnection connection,string runToken, string runType);

        public void UpsertTable(OdbcConnection connection,Dictionary<string, object> inputData, long fw_id, long token);       
    }
}
