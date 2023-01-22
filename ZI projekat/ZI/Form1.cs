using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace ZI
{
    public partial class Form1 : Form
    {
        protected static string key = "K/SRjz3ftDFK5+77U1PJ73v2StinkGap5yKYH5FI6tQ=";
        protected static string iv = "pf3g3uvhZEwJnLmRHwlHcg==";
        protected static string rotor1 = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";
        protected static string rotor2 = "AJDKSIRUXBLHWTMCQGZNPYFVOE";
        protected static string rotor3 = "BDFHJLCPRTXVZNYEIWGAKMUSQO";
        protected static string reflector = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
        protected static string plugboard = "ATBGCRDEFLHIJKLMNOQPSUVWXY";
        RC4 rc4 = new RC4();
        TEA tea = new TEA();
        CBC cbc = new CBC(key);
        Enigma enigma = new Enigma();
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            switch (cmbBoxAlgoritmi.Text)
            {
                case "RC4":
                    String str = textBox1.Text.ToString();
                    String key = txtKey.Text.ToString();
                    if (key.Length == 0)
                        MessageBox.Show("Unesite kljuc!");
                    else
                    {
                        Invoke(new Action(() => textBox2.Clear()));
                        byte[] encryptBytes = rc4.encrypt(str, key);
                        rc4.setBytes(encryptBytes);
                        string s = rc4.ToBinaryString(Encoding.UTF8, Encoding.ASCII.GetString(encryptBytes));
                        Invoke(new Action(() => textBox2.AppendText(rc4.ToBinaryString(Encoding.UTF8, Encoding.ASCII.GetString(encryptBytes)))));
                        Invoke(new Action(() => textBox1.Clear()));
                    }
                    break;
                case "TEA":
                    str = textBox1.Text.ToString();
                    key = txtKey.Text.ToString();
                    if (key.Length == 0)
                        MessageBox.Show("Unesite kljuc!");
                    else
                    {
                        Invoke(new Action(() => textBox2.Clear()));
                        string encryptMessage = tea.Encrypt(str, key);
                        tea.setMessage(encryptMessage);
                        Invoke(new Action(() => textBox2.AppendText(tea.ToBinaryString(Encoding.UTF8, encryptMessage))));
                        Invoke(new Action(() => textBox1.Clear()));
                    }
                    break;
                case "CBC":
                    str = textBox1.Text.ToString();                   
                    string encryptedString = cbc.Encrypt(str, iv);
                    cbc.set(encryptedString);
                    Invoke(new Action(() => textBox2.Clear()));
                    Invoke(new Action(() => textBox2.AppendText(cbc.ToBinaryString(Encoding.UTF8, encryptedString))));
                    Invoke(new Action(() => textBox1.Clear()));
                    break;
                case "Enigma":
                    var tekstZaEnkripciju = textBox1.Text;
                    Invoke(new Action(() => textBox2.Clear()));
                    bool provera = tekstZaEnkripciju.All(karakter => char.IsUpper(karakter));
                    if (provera)
                    {
                        var enkriptovaniTekst = enigma.Encrypt(tekstZaEnkripciju, rotor1, rotor2, rotor3, reflector, plugboard);
                        Invoke(new Action(() => textBox2.AppendText(enkriptovaniTekst)));
                        Invoke(new Action(() => textBox1.Clear()));
                    }
                    else
                        MessageBox.Show("Mozete uneti samo velika slova!");
                    break;
                default:
                    break;
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            switch (cmbBoxAlgoritmi.Text)
            {
                case "RC4":
                    String str = textBox2.Text.ToString();
                    String key = txtKey.Text.ToString();
                    if (key.Length == 0)
                        MessageBox.Show("Unesite kljuc!");
                    else
                    {
                        Invoke(new Action(() => textBox1.AppendText(Encoding.ASCII.GetString(rc4.decrypt(key)))));
                        Invoke(new Action(() => textBox2.Clear()));
                    }
                    break;
                case "TEA":
                    key = txtKey.Text.ToString();
                    string encryptMessage = tea.getMessage();
                    if (key.Length == 0)
                        MessageBox.Show("Unesite kljuc!");
                    else { 
                    Invoke(new Action(() => textBox1.AppendText(tea.Decrypt(encryptMessage, key))));
                    Invoke(new Action(() => textBox2.Clear()));
                        }
                    break;
                case "CBC":
                    string decryptedString = cbc.Decrypt(cbc.get(), iv);
                    Invoke(new Action(() => textBox1.AppendText(decryptedString)));
                    Invoke(new Action(() => textBox2.Clear()));
                    break;
                case "Enigma":
                    var tekstZaDekripciju = textBox2.Text;
                    var dekriptovaniTekst = enigma.Decrypt(tekstZaDekripciju, rotor1, rotor2, rotor3, reflector, plugboard);
                    Invoke(new Action(() => textBox1.Clear()));
                    Invoke(new Action(() => textBox1.AppendText(dekriptovaniTekst)));
                    Invoke(new Action(() => textBox2.Clear()));
                    break;
                default:
                    break;
            }
        }

        private void btnEncryptFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text|*.txt|All|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        using (var saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "Text |*.txt";
                            saveFileDialog.FileName = "encrypted.txt";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                var encryptedFilePath = saveFileDialog.FileName;
                                using (var encryptedFileStream = File.OpenWrite(encryptedFilePath))
                                {
                                    var aes = Aes.Create();
                                    aes.Key = Convert.FromBase64String(key); 
                                    aes.IV  = Convert.FromBase64String(iv);
                                    aes.Mode = CipherMode.CBC;
                                    using (var encryptor = aes.CreateEncryptor())
                                    {
                                        using (var cryptoStream = new CryptoStream(encryptedFileStream, encryptor, CryptoStreamMode.Write))
                                        {
                                            fileStream.CopyTo(cryptoStream);
                                        }
                                    }
                                }
                                MessageBox.Show("Fajl sacuvan kao: " + encryptedFilePath);
                            }
                        }
                    }
                }
            }
        }

        private void btnDecryptFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text|*.txt|All|*.*"; ;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var encryptedFilePath = openFileDialog.FileName;
                    using (var encryptedFileStream = File.OpenRead(encryptedFilePath))
                    {
                        using (var saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "Text |*.txt";
                            saveFileDialog.FileName = "decrypted.txt";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                var decryptedFilePath = saveFileDialog.FileName;
                                using (var decryptedFileStream = File.OpenWrite(decryptedFilePath))
                                {
                                    var aes = Aes.Create();
                                    aes.Key = Convert.FromBase64String(key);
                                    aes.IV = Convert.FromBase64String(iv);
                                    aes.Mode = CipherMode.CBC;
                                    using (var decryptor = aes.CreateDecryptor())
                                    {
                                        using (var cryptoStream = new CryptoStream(encryptedFileStream, decryptor, CryptoStreamMode.Read))
                                        {
                                            cryptoStream.CopyTo(decryptedFileStream);
                                        }
                                    }
                                }
                                MessageBox.Show("Fajl sacuvan kao: " + decryptedFilePath);
                            }
                        }
                    }
                }               
            }
        }

        private void btnCRCCompare_Click(object sender, EventArgs e)
        {
            using (var openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "All files (*.*)|*.*";
                openFileDialog1.Title = "Izaberite prvi fajl";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var file1 = openFileDialog1.FileName;
                    using (var openFileDialog2 = new OpenFileDialog())
                    {
                        openFileDialog2.Filter = "All files (*.*)|*.*";
                        openFileDialog2.Title = "Izaberite drugi fajl";
                        if (openFileDialog2.ShowDialog() == DialogResult.OK)
                        {
                            var file2 = openFileDialog2.FileName;
                            using (var fileStream1 = File.OpenRead(file1))
                            {
                                var crc = new CRC();
                                var hash1 = crc.ComputeHash(fileStream1);
                                using (var fileStream2 = File.OpenRead(file2))
                                {
                                    var hash2 = crc.ComputeHash(fileStream2);
                                    if (hash1.SequenceEqual(hash2))
                                    {
                                        MessageBox.Show("Fajlovi su identicni");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Fajlovi se razlikuju");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnEncriptBitmap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap Image|*.bmp";
            openFileDialog.Title = "Izaberite bitmapu za enkripciju";
            if (txtKey.Text.Length == 0)
                MessageBox.Show("Morate uneti kljuc!");
            else
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    byte[] key = Encoding.UTF8.GetBytes(txtKey.Text);
                    RC4 rc4 = new RC4(key);
                    rc4.EncryptBitmap(imageBytes);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Bitmap Image|*.bmp";
                    saveFileDialog.Title = "Sacuvajte generisanu bitmapu";
                    saveFileDialog.FileName = "encrypted.bmp";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, imageBytes);
                    }
                    MessageBox.Show("Bitmapa sacuvana kao: " + saveFileDialog.FileName);
                }
            }
        }

        private void btnDecryptBitmap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap Image|*.bmp";
            openFileDialog.Title = "Izaberite bitmapu za dekripciju";
            if (txtKey.Text.Length == 0)
                MessageBox.Show("Morate uneti kljuc!");
            else
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] encryptedImageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    byte[] key = Encoding.UTF8.GetBytes(txtKey.Text);
                    RC4 rc4 = new RC4(key);
                    rc4.DecryptBitmap(encryptedImageBytes);
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Bitmap Image|*.bmp";
                    saveFileDialog.Title = "Sacuvajte dekriptovanu bitmapu";
                    saveFileDialog.FileName = "decrypted.bmp";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, encryptedImageBytes);
                    }
                    MessageBox.Show("Bitmapa sacuvana kao: " + saveFileDialog.FileName);
                }
            }
        }
    }
}
