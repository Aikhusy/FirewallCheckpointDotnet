using System;
using System.Collections.Generic;
using Renci.SshNet;
using System.Text;

//==== Shells.cs ====
// file yang berisi segala sesuatu yang dibutuhkan untuk running SSH 
namespace Firewall
{
    public static class Shells

    {
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
        private static readonly Dictionary<string, string> shellScript = new Dictionary<string, string>
        {
            
            { "uptime", @"uptime" },//uptime shell script

            { "ram", @"free" },//ram and swap shell script

            { "disk", @"df" },//fwtmp and log shell script

            { "cpu", @"cpstat os -f perf" }, //cpu shell script

            { "raid", @"raid_diagnostic" },//raid shell script

            { "rxtx", @"ifconfig -a" },//rx error and tx error shell script

            { "license", @"cplic print" },//liscence description shell script

            { "hotfix", @"fw ver -k" },//hotfix version shell script

            { "memory_error", @"fw ctl pstat" },//failed allocation shell script

            { "capacity_optimisation", @"fw tab -t connections -s" },//getting VALS, PEAK, and SLINKS shell script
            { "capacity_limit", @"fw tab -t connections grep limit" },//getting Agressive aging limit shell script
            
            { "sync_mode", @"cphaprob -a if" },//getting sync mode shell script
            { "sync_state", @"cphaprob state" }//ram and swap shell script


        };

        // Method for running shell script and get the output result
        public static string RunAndGetOutput(string commands, ShellStream stream)
        {
            
            if (!shellScript.ContainsKey(commands))

            {

                throw new ArgumentException("Command not found: " + commands);

            }

            // Buat command SSH dari dictionary shellScript
            stream.WriteLine(shellScript[commands]); 

            return ReadShellStream(stream);
        }

        public static string GetUptime(ShellStream stream)
        {
            return RunAndGetOutput("uptime",stream);
        }

        public static string GetRam(ShellStream stream)
        {
            return RunAndGetOutput("ram",stream);
        }

        public static string GetDisk(ShellStream stream)
        {
            return RunAndGetOutput("disk",stream);
        }

        public static string GetCpu(ShellStream stream)
        {
            return RunAndGetOutput("cpu",stream);
        }

        public static string GetRaid(ShellStream stream)
        {
            return RunAndGetOutput("raid",stream);
        }
        public static string GetRxTx(ShellStream stream)
        {
            return RunAndGetOutput("rxtx",stream);
        }

        public static string GetLicense(ShellStream stream)
        {
            return RunAndGetOutput("license",stream);
        }

        public static string GetHotfix(ShellStream stream)
        {
            return RunAndGetOutput("hotfix",stream);
        }

        public static string GetMemoryError(ShellStream stream)
        {
            return RunAndGetOutput("license",stream);
        }

        public static string GetCapacityOptimisation(ShellStream stream)
        {
            return RunAndGetOutput("capacity_optimisation",stream);
        }

        public static string GetCapacityLimit(ShellStream stream)
        {
            return RunAndGetOutput("capacity_limit",stream);
        }

        public static string GetSyncMode(ShellStream stream)
        {
            return RunAndGetOutput("sync_mode",stream);
        }

        public static string GetSyncState(ShellStream stream)
        {
            return RunAndGetOutput("capacity_optimisation",stream);
        }
    }


}
