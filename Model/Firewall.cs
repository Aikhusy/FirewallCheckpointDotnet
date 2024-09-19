using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

//==== Firewall.cs ====
// This file is used to retrieve Firewall data from the database, which will be used to connect to SSH.
namespace Firewall
{
    public class Firewall : IFirewall
    {
        private readonly IConnection _connection;

        // Constructor to inject the connection dependency
        public Firewall(IConnection connection)
        {
            _connection = connection;
        }

        // Method to retrieve firewall data from the database
        public List<Dictionary<string, object>> GetFwData(OdbcConnection connections)
        {
            // Executes a select query and retrieves data from the firewall_login table
            List<Dictionary<string, object>> firewallData = _connection.ExecuteSelectQuery("firewall_login",connections);
            
            return firewallData;
        }
    }
}
