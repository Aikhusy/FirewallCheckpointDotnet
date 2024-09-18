using System;
using System.Collections.Generic;

namespace Firewall
{
    public interface IDB
    {
        string Driver { get; set; }
        string Server { get; set; }
        string Database { get; set; }
        string Trusted_Connection { get; set; }
        string UID { get; set; }
        string PWD { get; set; }
        string Encrypt { get; set; }
        string TrustServerCertificate { get; set; }
    }

    public interface IDelay
    {
        double Invoke_Shell_Delay { get; set; }
        double Run_Command_Delay { get; set; }
        double Expert_Shell_Delay { get; set; }
    }

    // Interface for JSON reading functionality
    public interface IJsonReader
    {
        IDB ReadDatabaseJsonConfig(string filePath);
        IDelay ReadDelayJsonConfig(string filePath);
    }
}
