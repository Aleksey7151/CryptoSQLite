using System;
using System.Collections.Generic;

namespace CryptoSQLite.Extensions
{
    internal static class MemoryExtensions
    {
        public static void ZeroMemory(this byte[] buf)
        {
            for (var i = 0; i < buf.Length; i++)
                buf[i] = 0;
        }

        public static void ZeroMemory(this uint[] buf)
        {
            for (var i = 0; i < buf.Length; i++)
                buf[i] = 0;
        }

        public static void ZeroMemory(this int[] buf)
        {
            for (var i = 0; i < buf.Length; i++)
                buf[i] = 0;
        }

        public static void ZeroMemory(this ulong[] buf)
        {
            for (var i = 0; i < buf.Length; i++)
                buf[i] = 0;
        }

        public static void MemCpy(
            this byte[] destination,
            byte[] source,
            int len,
            int destinationStartIndex = 0,
            int sourceStartIndex = 0)
        {
            if(destination.Length < len + destinationStartIndex || source.Length < len + sourceStartIndex)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < len; i++)
                destination[i + destinationStartIndex] = source[i + sourceStartIndex];
        }

        public static void Xor(this byte[] buff, byte value, int startIndex)
        {
            if(startIndex >= buff.Length)
                throw new ArgumentException(nameof(startIndex));

            for (var i = 0; i < buff.Length; i++)
                buff[i] ^= value;
        }

        public static void Xor(this byte[] destination, byte[] source, int len)
        {
            if (destination.Length < len || source.Length < len)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < len; i++)
                destination[i] ^= source[i];
        }
        
        public static void Xor(this uint[] destination, uint[] source, int len)
        {
            if (destination.Length < len || source.Length < len)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < len; i++)
                destination[i] ^= source[i];
        }

        public static void Xor(this ulong[] destination, ulong[] source, int len)
        {
            if (destination.Length < len || source.Length < len)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < len; i++)
                destination[i] ^= source[i];
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else dictionary.Add(key, value);
        }

        public static void UpdateSolt(this byte[] solt, int columnNumber)
        {
            var numb = BitConverter.GetBytes(columnNumber);
            for (var i = 0; i < solt.Length; i++)
            {
                solt[i] ^= numb[i%4];
            }
        }

        public static void UpdateSolt(this uint[] solt, int columnNumber)
        {
            for (var i = 0; i < solt.Length; i++)
            {
                solt[i] ^= (uint)columnNumber;
            }
        }
    }
}
