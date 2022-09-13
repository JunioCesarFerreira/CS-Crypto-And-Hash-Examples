using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace CryptoDES
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static int Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            try
            {
                string cmd = args[0];           // Comando, valido apenas 'encrypt' ou 'decrypt'
                string pathsource = args[1];    // Caminho do arquivo fonte
                string pathdestiny = args[2];   // Caminho do arquivo destino
                string cryptosalt = args[3];    // Sal para criptografia
                string cryptokey = args[4];     // Chave para criptografia

                cryptokey = cryptokey.Substring(0, 16); // redução da chave para tamanho máximo utilizado no Triple DES

                if (cmd == "encrypt" || cmd == "decrypt")
                {
                    if (File.Exists(pathsource))
                    {
                        // Prepara fluxo de leitura de arquivo em binário
                        FileStream file = new FileStream(pathsource, FileMode.Open, FileAccess.Read);
                        BinaryReader br = new BinaryReader(file);

                        Console.WriteLine(pathsource + " length " + file.Length.ToString() + " bytes");

                        long Progr = 0; // Indicador de progresso do arquivo de saída
                        int Length;     // Comprimento do buffer de bytes
                        bool encrypt;   // Indica encrypt ou decrypt
                        if (cmd == "encrypt")
                        {
                            Length = 33554432;
                            encrypt = true;
                        }
                        else
                        {
                            Length = 33554440;
                            encrypt = false;
                        }
                        Console.CursorVisible = false;
                        // Loop de varredura do arquivo criptografando
                        while (true)
                        {
                            bool end = false; // Indica quando terminou

                            #region Monta blocos de bytes
                            List<byte> bytes = new List<byte>();
                            bytes.Clear();
                            for (int j = 0; j < Length; j++)
                            {
                                try
                                {
                                    byte b = br.ReadByte();
                                    bytes.Add(b);
                                }
                                catch
                                {
                                    end = true;
                                    break;
                                }
                            }
                            #endregion

                            var key = new Rfc2898DeriveBytes(cryptokey, Convert.FromBase64String(cryptosalt));

                            TripleDES tripleDes = new TripleDESCryptoServiceProvider
                            {
                                Key = Encoding.UTF8.GetBytes(cryptokey),
                                IV = key.GetBytes(8),
                                Mode = CipherMode.ECB,
                                Padding = PaddingMode.PKCS7
                            };

                            MemoryStream mStream = new MemoryStream();
                            CryptoStream cryptor;
                            if (encrypt)
                            {
                                cryptor = new CryptoStream
                                            (
                                            mStream,
                                            tripleDes.CreateEncryptor(),
                                            CryptoStreamMode.Write
                                            );
                            }
                            else
                            {
                                cryptor = new CryptoStream
                                            (
                                            mStream,
                                            tripleDes.CreateDecryptor(),
                                            CryptoStreamMode.Write
                                            );
                            }
                            cryptor.Write(bytes.ToArray(), 0, bytes.Count);
                            cryptor.FlushFinalBlock();
                            byte[] crypt = mStream.ToArray();

                            Progr += crypt.Length;
                            string Out = cmd + "ing output " + Progr.ToString() + " bytes";
                            Console.Write(Out);
                            Console.CursorLeft -= Out.Length;

                            FileStream filecrypt = new FileStream(pathdestiny, FileMode.Append);
                            BinaryWriter bw = new BinaryWriter(filecrypt);
                            bw.Write(crypt);
                            bw.Close();
                            if (end) break;
                        }
                        br.Close();
                        Console.CursorVisible = true;
                        Console.WriteLine("\r\n" + cmd + "ed");
                        return 1;
                    }
                    else
                    {
                        throw new Exception("path source not exists");
                    }
                }
                else
                {
                    throw new Exception("failed comand");
                }
            }
            catch (Exception E)
            {
                ShowWindow(handle, SW_SHOW);
                Console.WriteLine(E.ToString());
                Console.ReadKey();
            }
            return -1;
        }
    }
}