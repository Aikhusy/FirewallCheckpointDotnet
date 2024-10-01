using System;
using System.Collections.Generic;
using Renci.SshNet;
using System.Text;

//==== Shells.cs ====
// file yang berisi segala sesuatu yang dibutuhkan untuk running SSH 
namespace Firewall
{
    public interface IShells

    {


        // Method for running shell script and get the output result
        string RunAndGetOutput(string commands, ShellStream stream);

        string GetUptime(ShellStream stream);

        string GetRam(ShellStream stream);

        string GetDisk(ShellStream stream);

        string GetCpu(ShellStream stream);


        string GetRaid(ShellStream stream);

        string GetRxTx(ShellStream stream);


        string GetLicense(ShellStream stream);

        string GetHotfix(ShellStream stream);


        string GetMemoryError(ShellStream stream);


        string GetCapacityOptimisation(ShellStream stream);

        string GetCapacityLimit(ShellStream stream);

        string GetSyncMode(ShellStream stream);

        string GetSyncState(ShellStream stream);
 
 
    }


}
