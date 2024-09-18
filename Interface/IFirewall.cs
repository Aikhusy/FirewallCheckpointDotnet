using System;
using System.Collections.Generic;

//==== Firewall.cs ====
// file yang digunakan untuk mendapatkan data Firewall Dari database yang nantinya akan digunakan untuk masuk ke SSH.
namespace Firewall
{
    public interface IFirewall
    {
        
        List<Dictionary<string, object>> GetFwData();

    } 
}
