using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZI
{
    public class Enigma
    {
        public string Encrypt(string plainText, string rotor1, string rotor2, string rotor3, string reflector, string plugboard)
        {
            var rotor1Array = rotor1.ToCharArray();
            var rotor2Array = rotor2.ToCharArray();
            var rotor3Array = rotor3.ToCharArray();

            var reflectorArray = reflector.ToCharArray();

            var plugboardArray = plugboard.ToCharArray();

            var cipherText = "";
            for (var i = 0; i < plainText.Length; i++)
            {
                var currentChar = plainText[i];

                StepRotor(ref rotor1Array);
                if (i % 26 == 0)
                {
                    StepRotor(ref rotor2Array);
                    if (i % (26 * 26) == 0)
                    {
                        StepRotor(ref rotor3Array);
                    }
                }

                currentChar = ApplyPlugboard(currentChar, plugboardArray);

                currentChar = ApplyRotor(currentChar, rotor1Array);
                currentChar = ApplyRotor(currentChar, rotor2Array);
                currentChar = ApplyRotor(currentChar, rotor3Array);

                currentChar = ApplyReflector(currentChar, reflectorArray);

                currentChar = ApplyRotor(currentChar, rotor3Array, true);
                currentChar = ApplyRotor(currentChar, rotor2Array, true);
                currentChar = ApplyRotor(currentChar, rotor1Array, true);

                currentChar = ApplyPlugboard(currentChar, plugboardArray);

                cipherText += currentChar;
            }
            
            return cipherText;
        }
        public string Decrypt(string cipherText, string rotor1, string rotor2, string rotor3, string reflector, string plugboard)
        {
            var rotor1Array = rotor1.ToCharArray();
            var rotor2Array = rotor2.ToCharArray();
            var rotor3Array = rotor3.ToCharArray();

            var reflectorArray = reflector.ToCharArray();

            var plugboardArray = plugboard.ToCharArray();

            var plainText = "";
            for (var i = 0; i < cipherText.Length; i++)
            {
                var currentChar = cipherText[i];

                StepRotor(ref rotor1Array);
                if (i % 26 == 0)
                {
                    StepRotor(ref rotor2Array);
                    if (i % (26 * 26) == 0)
                    {
                        StepRotor(ref rotor3Array);
                    }
                }

                currentChar = ApplyPlugboard(currentChar, plugboardArray);

                currentChar = ApplyRotor(currentChar, rotor1Array);
                currentChar = ApplyRotor(currentChar, rotor2Array);
                currentChar = ApplyRotor(currentChar, rotor3Array);

                currentChar = ApplyReflector(currentChar, reflectorArray);

                currentChar = ApplyRotor(currentChar, rotor3Array, true);
                currentChar = ApplyRotor(currentChar, rotor2Array, true);
                currentChar = ApplyRotor(currentChar, rotor1Array, true);

                currentChar = ApplyPlugboard(currentChar, plugboardArray);

                plainText += currentChar;
            }
            return plainText;
        }

        private void StepRotor(ref char[] rotor)
        {
            var firstChar = rotor[0];
            Array.Copy(rotor, 1, rotor, 0, rotor.Length - 1);
            rotor[rotor.Length - 1] = firstChar;
        }
        private char ApplyPlugboard(char currentChar, char[] plugboard)
        {
            var index = Array.IndexOf(plugboard, currentChar);
            return (index >= 0) ? plugboard[index ^ 1] : currentChar;
        }

        private char ApplyRotor(char currentChar, char[] rotor, bool reverse = false)
        {
            var index = Array.IndexOf(reverse ? Enumerable.Range('A', 26).Select(x => (char)x).ToArray() : rotor, currentChar);
            return (char)(reverse ? 'A' + index : rotor[index]);
        }

        private char ApplyReflector(char currentChar, char[] reflector)
        {
            var index = Array.IndexOf(reflector, currentChar);
            return reflector[(index + 13) % 26];
        }

    }
}
