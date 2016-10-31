﻿using System;

namespace CryptoSQLite
{
    internal static class Extensions
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

        public static void MemCpy(this byte[] destination, byte[] source, int len, int destinationStartIndex = 0, int sourceStartIndex = 0)
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
    }
}
