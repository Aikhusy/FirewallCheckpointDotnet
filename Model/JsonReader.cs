using System;
using System.IO;
using System.Text.Json;

namespace Firewall
{
    public class DB : IDB
    {
        public required string Driver { get; set; }
        public required string Server { get; set; }
        public required string Database { get; set; }
        public required string Trusted_Connection { get; set; }

        public required string UID { get; set; }

        public required string PWD { get; set; }

        public required string Encrypt { get; set; }

        public required string TrustServerCertificate { get; set; }
    }
    public class Delay : IDelay
    {
        public double Invoke_Shell_Delay { get; set; }
        public double Run_Command_Delay { get; set; }
        public double Expert_Shell_Delay { get; set; }

    }
    public class JsonReader : IJsonReader
    {
        
        public IDB ReadDatabaseJsonConfig(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return JsonSerializer.Deserialize<DB>(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading database configuration: {ex.Message}");
                throw;
            }
        }

        public IDelay ReadDelayJsonConfig(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return JsonSerializer.Deserialize<Delay>(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading delay configuration: {ex.Message}");
                throw;
            }
        }
    }
}
