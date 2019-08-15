using System;
using System.Collections.Generic;

namespace CryptoSQLite.Extensions
{
    internal static class DecimalExtensions
    {
        public static byte[] GetBytes(this decimal number)
        {
            var bits = decimal.GetBits(number);
            var bytes = new List<byte>();

            foreach (var i in bits)
            {
                bytes.AddRange(BitConverter.GetBytes(i));
            }

            bits.ZeroMemory();

            return bytes.ToArray();
        }

        public static decimal ToDecimal(this byte[] array)
        {
            if (array.Length != 16)
                throw new Exception("A decimal must be created from exactly 16 bytes");

            var bits = new int[4];

            for (var i = 0; i <= 15; i += 4)
            {
                bits[i/4] = BitConverter.ToInt32(array, i);
            }

            var @decimal = new decimal(bits);

            bits.ZeroMemory();

            return @decimal;
        }
    }
}
