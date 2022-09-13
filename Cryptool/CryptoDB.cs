using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Cryptool
{
    internal class CryptoDb
    {
        private const string mysalt = "5QFczpAXMjZwH8qg3sDaQn/zKAXu";
        private const string mykey = "j6RHeTqB2Nkaxm9UKh4tE6Rod0LBYN7x";

        public static bool ValidateAndSave(string pwd, string salt, string key)
        {
            try
            {
                pwd = HashSHA256(pwd);
                string config = Application.StartupPath + "\\collection.db";

                if (!File.Exists(config))
                {
                    File.WriteAllText(config, Encrypt("<collection />"));
                }

                string plaintext = File.ReadAllText(config);
                plaintext = Decrypt(plaintext);
                XElement xconf = XElement.Parse(plaintext);

                foreach (XElement X in xconf.Elements("item"))
                {
                    if (X.Attribute("pwd").Value == pwd)
                    {
                        throw new Exception("Senha já cadastrada");
                    }
                }

                XElement newitem = new XElement("item");
                newitem.Add(new XAttribute("pwd", pwd));
                newitem.Add(new XAttribute("salt", salt));
                newitem.Add(new XAttribute("key", key));
                xconf.Add(newitem);

                File.WriteAllText(config, Encrypt(xconf.ToString()));

                return true;
            }
            catch (Exception E)
            {
                MessageBox.Show("Falha! " + E.Message);
                return false;
            }
        }

        public static string[] Open(string pwd)
        {
            try
            {
                pwd = HashSHA256(pwd);
                string config = Application.StartupPath + "\\collection.db";

                if (!File.Exists(config))
                {
                    throw new Exception("Nenhum arquivo encontrado");
                }

                string plaintext = File.ReadAllText(config);
                plaintext = Decrypt(plaintext);
                XElement xconf = XElement.Parse(plaintext);

                foreach (XElement X in xconf.Elements("item"))
                {
                    if (X.Attribute("pwd").Value == pwd)
                    {
                        return new string[] 
                        {
                            X.Attribute("key").Value,
                            X.Attribute("salt").Value
                        };
                    }
                }
                throw new Exception("Senha não encontrada");
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return null;
            }
        }

        private static string HashSHA256(string value)
        {
            byte[] hashBytes;
            using (HashAlgorithm hash = SHA256.Create())
            {
                hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            }

            StringBuilder hashValue = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hashValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }
            return hashValue.ToString();
        }

        private static string Encrypt(string Text)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                byte[] bText = new UTF8Encoding().GetBytes(Text);
                var key = new Rfc2898DeriveBytes(mykey, Convert.FromBase64String(mysalt));
                Rijndael rijndael = new RijndaelManaged
                {
                    KeySize = 256,
                    BlockSize = 256,
                    Key = Convert.FromBase64String(mykey),
                    IV = key.GetBytes(mykey.Length)
                };
                MemoryStream mStream = new MemoryStream();
                CryptoStream encryptor = new CryptoStream
                    (
                    mStream,
                    rijndael.CreateEncryptor(),
                    CryptoStreamMode.Write
                    );
                encryptor.Write(bText, 0, bText.Length);
                encryptor.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            else return null;
        }

        private static string Decrypt(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                byte[] bText = Convert.FromBase64String(text);
                var key = new Rfc2898DeriveBytes(mykey, Convert.FromBase64String(mysalt));
                Rijndael rijndael = new RijndaelManaged
                {
                    KeySize = 256,
                    BlockSize = 256,
                    Key = Convert.FromBase64String(mykey),
                    IV = key.GetBytes(mykey.Length)
                };
                MemoryStream mStream = new MemoryStream();
                CryptoStream decryptor = new CryptoStream
                    (
                    mStream,
                    rijndael.CreateDecryptor(),
                    CryptoStreamMode.Write
                    );
                decryptor.Write(bText, 0, bText.Length);
                decryptor.FlushFinalBlock();
                UTF8Encoding utf8 = new UTF8Encoding();
                return utf8.GetString(mStream.ToArray());
            }
            else return null;
        }
    }
}
