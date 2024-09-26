using System;
using System.IO;
using System.Text.Json;
using Telegram.Bot;

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
    public class TeleBot : ITeleBot
    {
        public bool Telegram_Alert_Status { get; set; }
        public string Telegram_Bot_API { get; set; }
        public long Telegram_Chat_Id { get; set; }
    }

    public class Passphrase : IPassphrase
    {
        public string Encrypt_Phrase { get; set; }
    }
    public class JsonReader : IJsonReader
    {
        private string GetAbsolutePath(string relativePath)
        {
            // Get the directory of the executing assembly
            string applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            // Combine it with the relative file path
            return Path.Combine(applicationPath, relativePath);
        }

        public IDB ReadDatabaseJsonConfig(string filePath)
        {
            string absolutePath = GetAbsolutePath(filePath);
            try
            {
                using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
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
            string absolutePath = GetAbsolutePath(filePath);
            try
            {
                using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
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

        public ITeleBot ReadTelegramJsonConfig(string filePath)
        {
            string absolutePath = GetAbsolutePath(filePath);
            try
            {
                using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
                {
                    return JsonSerializer.Deserialize<TeleBot>(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Telegram configuration: {ex.Message}");
                throw;
            }
        }

        public IPassphrase ReadEcryptJsonConfig(string filePath)
        {
            string absolutePath = GetAbsolutePath(filePath);
            try
            {
                using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read))
                {
                    return JsonSerializer.Deserialize<Passphrase>(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Telegram configuration: {ex.Message}");
                throw;
            }
        }
    }
}
