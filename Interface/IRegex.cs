using System;
using System.Collections.Generic; // You need this for Dictionary
using System.Text.RegularExpressions;
using System.Data.Odbc;
//==== Regex.cs ====
// file yang berisi regular expression untuk membaca data yang telah didapatkan dan menyimpannya kedalam database
namespace Firewall
{
    public interface IRegexs
    {
        // const bool detailedSave; // digunakan untuk status apakah detail data akan disimpan di database
        bool GetDetailedSave();
        //untuk menyimpan nilai regex yang digunakan untuk membaca data 
        Dictionary<string, Regex> Patterns { get; }

        //function untuk Mengolah data uptime, dan menyimpan ke database.
        Dictionary<string, string> RegexUptime(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data RAM dan SWAP, dan menyimpan ke database.
        Dictionary<string, double> RegexMemory(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data disk (/tmp dan /logs), dan menyimpan ke database.
        Dictionary<string, string> RegexDisk(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data Rx error dan Tx error, dan menyimpan ke database.
        Dictionary<string, int> RegexRxTx(OdbcConnection connection,string inputs, long fwId, long tokenId);
        //function untuk mengolah data Raid, dan menyimpan ke database.
        string RegexRaid(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data patch firewall, dan menyimpannya ke database.
        string RegexHotfix(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data CPU, dan menyimpannya ke database.
        string RegexCpu(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data memory gagal, dan menyimpannya ke database.
        int RegexFailedMemory(OdbcConnection connection,string inputs, long fwId, long tokenId);

        //function untuk mengolah data SyncMode dan Sync State, serta menyimpannya ke database.
        Dictionary<string, string> RegexSyncMode(OdbcConnection connection,string inputstate, string inputMode, long fwId, long tokenId);

        string RegexExpiration(OdbcConnection connection,string inputs, long fwId, long tokenId);
        
        string RegexCapacityOptimisationRemark(OdbcConnection connection, string inputSlinks, string inputLimit, long fwId, long tokenId);
    }
}
