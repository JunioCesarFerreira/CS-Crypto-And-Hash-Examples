using System;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace ConsoleTestHashes
{
    class Program
    {
        /// <summary>
        /// Programa de exemplo de uso de métodos de hash com System.Security.Cryptography.
        /// </summary>
        /// <param name="args"></param>
        static void Main()
        {
            Console.Write("pwd: ");
            string pwd = Console.ReadLine();

            string tmpStg = HashSHA1(pwd);
            Console.WriteLine("SHA1");
            Console.WriteLine("Result: " +tmpStg);
            Console.WriteLine("Lenght: " + (tmpStg.Length / 2).ToString() + " bytes\n");

            tmpStg = HashMD5(pwd);
            Console.WriteLine("MD5");
            Console.WriteLine("Result: " + tmpStg);
            Console.WriteLine("Lenght: " + (tmpStg.Length / 2).ToString() + " bytes\n");

            tmpStg = HashSHA256(pwd);
            Console.WriteLine("SHA256");
            Console.WriteLine("Result: " + tmpStg);
            Console.WriteLine("Lenght: " + (tmpStg.Length / 2).ToString() + " bytes\n");

            tmpStg = HashSHA512(pwd);
            Console.WriteLine("SHA512");
            Console.WriteLine("Result: " + tmpStg);
            Console.WriteLine("Lenght: " + (tmpStg.Length / 2).ToString() + " bytes\n");

            tmpStg = HashRIPEMD160(pwd);
            Console.WriteLine("RIPEMD160");
            Console.WriteLine("Result: " + tmpStg);
            Console.WriteLine("Lenght: " + (tmpStg.Length / 2).ToString() + " bytes\n");

            byte[] salt = new byte[8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(salt);
            }
            StringBuilder saltValue = new StringBuilder(salt.Length * 2);
            foreach (byte b in salt)
            {
                saltValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }
            tmpStg = saltValue.ToString();
            Console.WriteLine("RNG");
            Console.WriteLine("Result: " + tmpStg);
            Console.WriteLine("Lenght: " + (tmpStg.Length / 2).ToString() + " bytes");

            Console.ReadKey();
        }

        internal static string HashSHA256(string value)
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

        internal static string HashSHA1(string value)
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(value);
            using (HashAlgorithm hash = SHA1.Create())
            {
                hashBytes = hash.ComputeHash(hashBytes);
            }

            StringBuilder hashValue = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hashValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }
            return hashValue.ToString();
        }

        internal static string HashMD5(string value)
        {
            MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] hashBytes = MyMD5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(value));
            MyMD5CryptoService.Clear();

            StringBuilder hashValue = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hashValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }
            return hashValue.ToString();
        }

        internal static string HashRIPEMD160(string value)
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(value);
            using (HashAlgorithm hash = RIPEMD160.Create())
            {
                hashBytes = hash.ComputeHash(hashBytes);
            }

            StringBuilder hashValue = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hashValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }
            return hashValue.ToString();
        }

        internal static string HashSHA512(string value)
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(value);
            using (HashAlgorithm hash = SHA512.Create())
            {
                hashBytes = hash.ComputeHash(hashBytes);
            }

            StringBuilder hashValue = new StringBuilder(hashBytes.Length * 2);
            foreach (byte b in hashBytes)
            {
                hashValue.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", b);
            }
            return hashValue.ToString();
        }
    }
}
