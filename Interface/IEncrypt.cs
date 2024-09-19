using System;
using System.Security.Cryptography;
using System.Text;

//==== Encrypt.cs ====
// file yang berisi enkripsi yang digunakan untuk enkripsi dan dekripsi data password.
namespace Firewall
{
    public interface IEncrypt
    {

        string EncryptPassword(string plainPassword);
        

        public string DecryptPassword(string encryptedPassword);
    }
}
