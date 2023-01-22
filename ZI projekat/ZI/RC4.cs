using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZI
{
    internal class RC4
    {
        private byte[] encryptBytes;
        private byte[] S;
        private int x;
        private int y;

        public RC4()
        { }
        public void setBytes(byte[] bytes)
        {
            this.encryptBytes = bytes;
        }
        public byte[] getBytes()
        {
            return this.encryptBytes;
        }
        public static byte[] PseudoRandomRc4(int[] sBox, byte[] messageBytes)
        {
            var i = 0;
            var j = 0;
            var cnt = 0;
            var tempBox = new int[sBox.Length];
            var result = new byte[messageBytes.Length];
            Array.Copy(sBox, tempBox, tempBox.Length);

            foreach (var textByte in messageBytes)
            {
                i = (i + 1) % 256;
                j = (j + tempBox[i]) % 256;
                var temp = tempBox[i];
                tempBox[i] = tempBox[j];
                tempBox[j] = temp;
                var t = (tempBox[i] + tempBox[j]) % 256;
                var k = tempBox[t];

                var ss = textByte ^ k;
                result[cnt] = (byte)ss;
                cnt++;
            }
            return result;
        }

        public byte[] encrypt(string str, string key)
        {
            var asciiKeyBytes = Encoding.ASCII.GetBytes(key);
            var sBox = Enumerable.Range(0, 256).ToArray();
            var j = 0;
            for (var i = 0; i < 256; i++)
            {
                j = (j + sBox[i] + asciiKeyBytes[i % asciiKeyBytes.Length]) % 256;
                sBox[j] = sBox[i];
                sBox[i] = j;
            }
            var encryptBytes = PseudoRandomRc4(sBox, Encoding.ASCII.GetBytes(str));
            return encryptBytes;
        }
        public byte[] decrypt(string key)
        {
            var asciiKeyBytes = Encoding.ASCII.GetBytes(key);
            var sBox = Enumerable.Range(0, 256).ToArray();
            var j = 0;
            for (var i = 0; i < 256; i++)
            {
                j = (j + sBox[i] + asciiKeyBytes[i % asciiKeyBytes.Length]) % 256;
                sBox[j] = sBox[i];
                sBox[i] = j;
            }
            return PseudoRandomRc4(sBox, encryptBytes);
        }

        public string ToBinaryString(Encoding encoding, string text)
        {
            return string.Join("", encoding.GetBytes(text).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }



        public RC4(byte[] key)
        {
            S = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                S[i] = (byte)i;
            }
            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                Swap(S, i, j);
            }
            x = 0;
            y = 0;
        }
        public void EncryptBitmap(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                x = (x + 1) % 256;
                y = (y + S[x]) % 256;
                Swap(S, x, y);
                data[i] ^= S[(S[x] + S[y]) % 256];
            }
        }
        public void DecryptBitmap(byte[] data)
        {
            EncryptBitmap(data);
        }
        private void Swap(byte[] s, int i, int j)
        {
            byte temp = s[i];
            s[i] = s[j];
            s[j] = temp;
        }

    }
}
