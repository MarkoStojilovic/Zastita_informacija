using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZI
{
    internal class TEA
    {
        private string encryptMessage;

        public void setMessage(string msg)
        {
            this.encryptMessage = msg;
        }
        public string getMessage()
        {
            return this.encryptMessage;
        }
        private void code(uint[] v, uint[] k)
        {
            uint y = v[0];
            uint z = v[1];
            uint sum = 0;
            uint delta = 0x8f4519c3;
            uint n = 32;

            while (n-- > 0)
            {
                y += (z << 4 ^ z >> 5) + z ^ sum + k[sum & 3];
                sum += delta;
                z += (y << 4 ^ y >> 5) + y ^ sum + k[sum >> 11 & 3];
            }

            v[0] = y;
            v[1] = z;
        }

        private void decode(uint[] v, uint[] k)
        {
            uint n = 32;
            uint sum;
            uint y = v[0];
            uint z = v[1];
            uint delta = 0x8f4519c3;

            sum = delta << 5;

            while (n-- > 0)
            {
                z -= (y << 4 ^ y >> 5) + y ^ sum + k[sum >> 11 & 3];
                sum -= delta;
                y -= (z << 4 ^ z >> 5) + z ^ sum + k[sum & 3];
            }

            v[0] = y;
            v[1] = z;
        }

        public uint[] FormatKey(string Key)
        {
            if (Key.Length == 0)
                throw new ArgumentException("Uneti kljuc mora biti duzine izmedju 1 i 16 karaktera!");
            Key = Key.PadRight(16, ' ').Substring(0, 16);
            uint[] formattedKey = new uint[4];
            int j = 0;
            for (int i = 0; i < Key.Length; i += 4)
                formattedKey[j++] = ConvertStringToUInt(Key.Substring(i, 4));

            return formattedKey;
        }

        public string Encrypt(string Data, string Key)
        {
            uint[] formattedKey = FormatKey(Key);
            if (Data.Length % 2 != 0) Data += '\0';
            byte[] dataBytes = ASCIIEncoding.ASCII.GetBytes(Data);

            string cipher = string.Empty;
            uint[] tempData = new uint[2];
            for (int i = 0; i < dataBytes.Length; i += 2)
            {
                tempData[0] = dataBytes[i];
                tempData[1] = dataBytes[i + 1];
                code(tempData, formattedKey);
                cipher += ConvertUIntToString(tempData[0]) + ConvertUIntToString(tempData[1]);
            }

            return cipher;
        }

        public string Decrypt(string Data, string Key)
        {
            uint[] formattedKey = FormatKey(Key);

            int x = 0;
            uint[] tempData = new uint[2];
            byte[] dataBytes = new byte[Data.Length / 8 * 2];
            for (int i = 0; i < Data.Length; i += 8)
            {
                tempData[0] = ConvertStringToUInt(Data.Substring(i, 4));
                tempData[1] = ConvertStringToUInt(Data.Substring(i + 4, 4));
                decode(tempData, formattedKey);
                dataBytes[x++] = (byte)tempData[0];
                dataBytes[x++] = (byte)tempData[1];
            }
            string decipheredString = ASCIIEncoding.ASCII.GetString(dataBytes, 0, dataBytes.Length);
            if (decipheredString[decipheredString.Length - 1] == '\0')
                decipheredString = decipheredString.Substring(0, decipheredString.Length - 1);
            return decipheredString;
        }

        private string ConvertUIntToString(uint Input)
        {
            StringBuilder output = new StringBuilder();
            output.Append((char)((Input & 0xFF)));
            output.Append((char)((Input >> 8) & 0xFF));
            output.Append((char)((Input >> 16) & 0xFF));
            output.Append((char)((Input >> 24) & 0xFF));
            return output.ToString();
        }

        private uint ConvertStringToUInt(string Input)
        {
            uint output;
            output = ((uint)Input[0]);
            output += ((uint)Input[1] << 8);
            output += ((uint)Input[2] << 16);
            output += ((uint)Input[3] << 24);
            return output;
        }
        public string ToBinaryString(Encoding encoding, string text)
        {
            return string.Join("", encoding.GetBytes(text).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }

    }
}
