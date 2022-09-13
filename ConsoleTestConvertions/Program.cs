using System;
using System.Linq;
using System.Text;

namespace ConsoleTestConvertions
{
    /// <summary>
    /// Programa de exemplo de codificação em base 64 e exemplos de exibição dos dados em hexadecimal e binário.
    /// </summary>
    class Program
    {
        private static readonly char[] b64 =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', '/', '+'
        };

        static void Main()
        {
            Console.WriteLine("Teste de codificação em base 64\r\nEntre com uma string:");

            string input_test = Console.ReadLine();
            if (input_test == "")
            {
                input_test = "The quick brown fox jumps over the lazy dog";
                Console.WriteLine(input_test);
            }

            Console.WriteLine("\nString de entrada em hexadecimal:");
            foreach (char ch in input_test.ToArray())
            {
                Console.Write(string.Format("{0:X2} ", Convert.ToUInt64(ch)));
            }
            Console.WriteLine();

            Console.WriteLine("\nString de entrada em decimal:");
            foreach (char ch in input_test.ToArray())
            {
                Console.Write(string.Format("{0:D2} ", Convert.ToUInt64(ch)));
            }
            Console.WriteLine();

            Console.WriteLine("\nString de entrada em binário 8 bits:");
            string bin_stg = "";
            foreach (char ch in input_test.ToArray())
            {
                Console.Write(ConvertToBinary(ch) + ' ');
                bin_stg += ConvertToBinary(ch);
            }
            Console.WriteLine();

            Console.WriteLine("\nString de entrada em binário 6 bits:");
            // Converte para base 64
            int count = 5;
            int value = 0;
            string rest = "";
            string rest_dec = "";
            foreach (char bin_ch in bin_stg.ToArray())
            {
                value += Convert.ToUInt16(bin_ch-48)*Pow2Int(count);
                Console.Write(bin_ch);
                count--;
                if (count < 0)
                {
                    rest += b64[value];
                    rest_dec += (value.ToString() + ' ');
                    value = 0;
                    count = 5;
                    Console.Write(' ');
                }
            }

            int pad = 0;
            while (count >= 0 && count!=5)
            {
                Console.Write('0');
                count--;
                if (count < 0)
                {
                    rest += b64[value];
                    rest_dec += (value.ToString() + ' ');
                }
                pad++;
            }
            switch (pad)
            {
                case 2:
                    rest += '=';
                    break;
                case 4:
                    rest += "==";
                    break;
            }

            Console.WriteLine();

            Console.WriteLine("\nSaida base 64 em decimal:");
            Console.WriteLine(rest_dec);

            Console.WriteLine("\nSaida base 64 em ASCII:");
            Console.WriteLine(rest);

            Console.WriteLine("\r\nVerificando...");
            Console.WriteLine(Base64Encode(input_test));

            Console.ReadKey();
        }

        public static int Pow2Int(int v)
        {
            int r = 1;
            while (v > 0)
            {
                r *= 2;
                v--;
            }
            return r;
        }

        public static string ConvertToBinary(char v)
        {
            string acum = "";
            long number = Convert.ToInt64(v);

            while (number != 0)
            {
                long r = number % 2;
                acum += r;
                number /= 2;
            }

            char[] chain = acum.ToArray();
            char[] Byte = { '0', '0', '0', '0', '0', '0', '0', '0'};
            for (int i = 0; i < acum.Length; i++)
            {
                Byte[7-i] = chain[i];
            }

            return string.Join(null, Byte);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
