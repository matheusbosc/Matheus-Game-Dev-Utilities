using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace com.matheusbosc.utilities
{
        public static class SaveEncryption
        {
            // Use your own key and IV - MUST be 16 characters for AES-128
            private static readonly string key = "7XqhtaVtHNygsKlh";
            private static readonly string iv = "k8AZ3I9TR3y6ER21";

            public static string Encrypt(string plainText)
            {
                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using MemoryStream ms = new();
                using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                using StreamWriter sw = new(cs);
                sw.Write(plainText);
                sw.Close();

                return Convert.ToBase64String(ms.ToArray());
            }

            public static string Decrypt(string cipherText)
            {
                byte[] buffer = Convert.FromBase64String(cipherText);

                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using MemoryStream ms = new(buffer);
                using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                return sr.ReadToEnd();
            }
        }
    
}