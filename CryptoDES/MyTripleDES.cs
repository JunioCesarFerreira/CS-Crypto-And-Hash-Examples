using System;
using System.Text;
using System.Security.Cryptography;

namespace Crypto
{
    internal class MyTripleDES
    {

        private readonly string mysecurityKey;

        public MyTripleDES(string Key)
        {
            mysecurityKey = Key;
        }

        public byte[] Encrypt(byte[] MyEncryptedArray)
        {
            MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));
            MyMD5CryptoService.Clear();

            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider
            {
                Key = MysecurityKeyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var MyCrytpoTransform = MyTripleDESCryptoService.CreateEncryptor();

            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock
                (
                MyEncryptedArray, 
                0,
                MyEncryptedArray.Length
                );

            MyTripleDESCryptoService.Clear();

            return MyresultArray;
        }



        public byte[] Decrypt(byte[] MyDecryptArray)
        {
            MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));
            MyMD5CryptoService.Clear();

            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider
            {
                Key = MysecurityKeyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var MyCrytpoTransform = MyTripleDESCryptoService.CreateDecryptor();

            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock
                (
                MyDecryptArray,
                0,
                MyDecryptArray.Length
                );

            MyTripleDESCryptoService.Clear();

            return MyresultArray;
        }

        public string Encrypt(string TextToEncrypt)
        {
            byte[] MyEncryptedArray = UTF8Encoding.UTF8.GetBytes(TextToEncrypt);

            MD5CryptoServiceProvider MyMD5CryptoService = new MD5CryptoServiceProvider();

            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(mysecurityKey));

            MyMD5CryptoService.Clear();

            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider
            {
                Key = MysecurityKeyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var MyCrytpoTransform = MyTripleDESCryptoService.CreateEncryptor();

            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock
                (
                MyEncryptedArray, 0,
                MyEncryptedArray.Length
                );

            MyTripleDESCryptoService.Clear();

            return Convert.ToBase64String(MyresultArray, 0, MyresultArray.Length);
        }



        public string Decrypt(string TextToDecrypt)
        {
            byte[] MyDecryptArray = Convert.FromBase64String
               (TextToDecrypt);

            MD5CryptoServiceProvider MyMD5CryptoService = new
               MD5CryptoServiceProvider();

            byte[] MysecurityKeyArray = MyMD5CryptoService.ComputeHash
               (UTF8Encoding.UTF8.GetBytes(mysecurityKey));

            MyMD5CryptoService.Clear();

            var MyTripleDESCryptoService = new TripleDESCryptoServiceProvider
            {
                Key = MysecurityKeyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var MyCrytpoTransform = MyTripleDESCryptoService.CreateDecryptor();

            byte[] MyresultArray = MyCrytpoTransform.TransformFinalBlock
                (
                MyDecryptArray, 
                0,
                MyDecryptArray.Length
                );

            MyTripleDESCryptoService.Clear();

            return UTF8Encoding.UTF8.GetString(MyresultArray);
        }
    }
}