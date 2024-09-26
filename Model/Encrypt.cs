using System;
using System.Security.Cryptography;
using System.Text;

namespace Firewall
{
    public class Encrypt : IEncrypt
    {
        private readonly IJsonReader _JsonReader;
        private readonly string passphrase;
        public Encrypt(IJsonReader jsons)
        {
            _JsonReader = jsons;
            IPassphrase config= _JsonReader.ReadEcryptJsonConfig("Config.json");
            passphrase = config.Encrypt_Phrase;
        }

        

        // Helper method to derive the key from the passphrase and salt
        private static byte[] DeriveKey(string passphrase, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(passphrase, salt, 16384, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32); // Key length for AES-256
            }
        }

        // Encrypt a plain password
        public string EncryptPassword(string plainPassword)
        {
            byte[] salt = new byte[16];
            byte[] nonce = new byte[12]; // 12 bytes for AES-GCM nonce
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
                rng.GetBytes(nonce);
            }

            byte[] key = DeriveKey(passphrase, salt);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainPassword);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[16]; // 16 bytes for authentication tag

            using (AesGcm aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plaintextBytes, ciphertext, tag); // Encrypts plaintext into ciphertext
            }

            // Combine nonce, salt, tag, and ciphertext
            byte[] encryptedData = new byte[nonce.Length + salt.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, encryptedData, 0, nonce.Length);
            Buffer.BlockCopy(salt, 0, encryptedData, nonce.Length, salt.Length);
            Buffer.BlockCopy(tag, 0, encryptedData, nonce.Length + salt.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, encryptedData, nonce.Length + salt.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(encryptedData); // Encode the result in Base64 for easy storage/transmission
        }

        // Decrypt an encrypted password
        public string DecryptPassword(string encryptedPassword)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedPassword);

            byte[] nonce = new byte[12]; // 12 bytes for AES-GCM nonce
            byte[] salt = new byte[16];
            byte[] tag = new byte[16]; // 16 bytes for authentication tag
            byte[] ciphertext = new byte[encryptedData.Length - 44]; // Subtract nonce, salt, and tag lengths

            Buffer.BlockCopy(encryptedData, 0, nonce, 0, 12);
            Buffer.BlockCopy(encryptedData, 12, salt, 0, 16);
            Buffer.BlockCopy(encryptedData, 28, tag, 0, 16);
            Buffer.BlockCopy(encryptedData, 44, ciphertext, 0, ciphertext.Length);

            byte[] key = DeriveKey(passphrase, salt);

            using (AesGcm aes = new AesGcm(key))
            {
                byte[] decryptedPassword = new byte[ciphertext.Length];
                aes.Decrypt(nonce, ciphertext, tag, decryptedPassword); // Decrypts ciphertext back to plaintext

                return Encoding.UTF8.GetString(decryptedPassword); // Convert byte array back to string
            }
        }
    }
}
