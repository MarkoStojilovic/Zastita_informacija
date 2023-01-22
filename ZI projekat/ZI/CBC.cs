using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZI
{
    internal class CBC
    {
        private string encryptedString;    
        readonly byte[] key;
        public void set(string s)
        {
            this.encryptedString = s;
        }
        public string get()
        {
            return this.encryptedString;
        }
        public CBC(string base64key)
        {
            this.key = Convert.FromBase64String(base64key);
        }
        public static string GenerateIV()
        {
            var rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.GenerateIV();
            return Convert.ToBase64String(rijndaelManaged.IV);
        }
        public string Encrypt(string plainString, string base64IV)
        {
            var rijndael = new RijndaelManaged
            {
                BlockSize = 128,
                Key = key,
                IV = Convert.FromBase64String(base64IV),
            };
            var encryptor = rijndael.CreateEncryptor();
            byte[] bytes = Encoding.UTF8.GetBytes(plainString);
            return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
        }
        public string Decrypt(string base64Encrypted, string base64IV)
        {
            var rijndael = new RijndaelManaged
            {
                BlockSize = 128,
                Key = key,
                IV = Convert.FromBase64String(base64IV),
            };
            var decryptor = rijndael.CreateDecryptor();
            byte[] bytes = Convert.FromBase64String(base64Encrypted);
            return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(bytes, 0, bytes.Length));
         }
        public static string GenerateNewKey()
        {
            var rijndael = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 128
            };
            rijndael.GenerateKey();
            return Convert.ToBase64String(rijndael.Key);
        }
        public string GenerateNewIv()
        {
            var rijndael = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 128
            };
            rijndael.GenerateIV();
            return Convert.ToBase64String(rijndael.IV);
        }
        public string ToBinaryString(Encoding encoding, string text)
        {
            return string.Join("", encoding.GetBytes(text).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }
        public string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }
    }
}
