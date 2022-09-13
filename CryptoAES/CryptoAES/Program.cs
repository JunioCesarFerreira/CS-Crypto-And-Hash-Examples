using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace CryptoAES
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
                string cmd = args[0];           // Comando, válido apenas 'encrypt' ou 'decrypt'.
                string pathsource = args[1];    // Caminho do arquivo fonte.
                string pathdestiny = args[2];   // Caminho do arquivo destino.
                string cryptosalt = args[3];    // Sal para criptografia.
                string cryptokey = args[4];     // Chave para criptografia.

                if (cryptosalt.Length != 28) throw new Exception("invalid salt");
                if (cryptokey.Length != 32) throw new Exception("invalid key");
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
                            Length = 33554464;
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

                            #region Criptografia AES
                            // Instancia de derivação da chave
                            var key = new Rfc2898DeriveBytes(cryptokey, Convert.FromBase64String(cryptosalt));
                            // Instancia para uso do algoritmo RijnDael AES 
                            Rijndael rijndael = new RijndaelManaged
                            {
                                KeySize = 256,
                                BlockSize = 256,
                                Key = Convert.FromBase64String(cryptokey),
                                IV = key.GetBytes(cryptokey.Length)
                            };
                            MemoryStream mStream = new MemoryStream();
                            CryptoStream cryptor;
                            if (encrypt)
                            {
                                cryptor = new CryptoStream
                                            (
                                            mStream,
                                            rijndael.CreateEncryptor(),
                                            CryptoStreamMode.Write
                                            );
                            }
                            else
                            {
                                cryptor = new CryptoStream
                                            (
                                            mStream,
                                            rijndael.CreateDecryptor(),
                                            CryptoStreamMode.Write
                                            );
                            }
                            cryptor.Write(bytes.ToArray(), 0, bytes.Count);
                            cryptor.FlushFinalBlock();
                            byte[] crypt = mStream.ToArray();
                            #endregion

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
                Console.WriteLine(E.Message);
            }
            return -1;
        }
    }
}
