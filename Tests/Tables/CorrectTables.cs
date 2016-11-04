using System;
using CryptoSQLite;

namespace Tests.Tables
{
    [CryptoTable("OpenStrings")]
    internal class OpenStrings
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public string Str1 { get; set; }

        public string Str2 { get; set; }
    }

    [CryptoTable("EncryptedStrings")]
    internal class EncryptedStrings
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string Str1 { get; set; }

        [Encrypted]
        public string Str2 { get; set; }
    }

    [CryptoTable("ShortNumbers")]
    internal class ShortNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public short ShortMaxVal { get; set; }
        public short ShortMinVal { get; set; }

        public ShortNumbers()
        {
            ShortMaxVal = short.MaxValue;
            ShortMinVal = short.MinValue;
        }
    }

    [CryptoTable("ShortEncryptedNumbers")]
    internal class ShortEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public short ShortMaxVal { get; set; }

        [Encrypted]
        public short ShortMinVal { get; set; }

        public ShortEncryptedNumbers()
        {
            ShortMaxVal = short.MaxValue;
            ShortMinVal = short.MinValue;
        }
    }

    [CryptoTable("UShortNumbers")]
    internal class UShortNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public ushort UShortMaxVal { get; set; }
        public ushort UShortMinVal { get; set; }

        public UShortNumbers()
        {
            UShortMaxVal = ushort.MaxValue;
            UShortMinVal = ushort.MinValue;
        }
    }

    [CryptoTable("UShortEncryptedNumbers")]
    internal class UShortEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public ushort UShortMaxVal { get; set; }

        [Encrypted]
        public ushort UShortMinVal { get; set; }

        public UShortEncryptedNumbers()
        {
            UShortMaxVal = ushort.MaxValue;
            UShortMinVal = ushort.MinValue;
        }
    }

    [CryptoTable("IntNumbers")]
    internal class IntNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public int IntMaxVal { get; set; }

        public int IntMinVal { get; set; }

        public IntNumbers()
        {
            IntMinVal = int.MinValue;
            IntMaxVal = int.MaxValue;
        }
    }

    [CryptoTable("IntEncryptedNumbers")]
    internal class IntEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public int IntMaxVal { get; set; }

        [Encrypted]
        public int IntMinVal { get; set; }

        public IntEncryptedNumbers()
        {
            IntMinVal = int.MinValue;
            IntMaxVal = int.MaxValue;
        }
    }

    [CryptoTable("UIntNumbers")]
    internal class UIntNumbers
    {
        [PrimaryKey, AutoIncremental]
        public uint Id { get; set; }

        public uint UIntMaxVal { get; set; }

        public uint UIntMinVal { get; set; }

        public UIntNumbers()
        {
            UIntMaxVal = uint.MaxValue;
            UIntMinVal = uint.MinValue;
        }
    }

    [CryptoTable("UIntEncryptedNumbers")]
    internal class UIntEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public uint Id { get; set; }

        [Encrypted]
        public uint UIntMaxVal { get; set; }

        [Encrypted]
        public uint UIntMinVal { get; set; }

        public UIntEncryptedNumbers()
        {
            UIntMaxVal = uint.MaxValue;
            UIntMinVal = uint.MinValue;
        }
    }

    [CryptoTable("ULongNumbers")]
    internal class ULongNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public ulong ULongMaxVal { get; set; }

        public ulong ULongMinVal { get; set; }

        public ULongNumbers()
        {
            ULongMinVal = ulong.MinValue;
            ULongMaxVal = ulong.MaxValue;
        }
    }

    [CryptoTable("ULongEncryptedNumbers")]
    internal class ULongEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public ulong ULongMaxVal { get; set; }

        [Encrypted]
        public ulong ULongMinVal { get; set; }

        public ULongEncryptedNumbers()
        {
            ULongMinVal = ulong.MinValue;
            ULongMaxVal = ulong.MaxValue;
        }
    }

    [CryptoTable("LongNumbers")]
    internal class LongNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public long LongMaxVal { get; set; }

        public long LongMinVal { get; set; }

        public LongNumbers()
        {
            LongMaxVal = long.MaxValue;
            LongMinVal = long.MinValue;
        }
    }

    [CryptoTable("LongEncryptedNumbers")]
    internal class LongEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public long LongMaxVal { get; set; }

        [Encrypted]
        public long LongMinVal { get; set; }

        public LongEncryptedNumbers()
        {
            LongMaxVal = long.MaxValue;
            LongMinVal = long.MinValue;
        }
    }

    [CryptoTable("DateTimeTable")]
    internal class DateTimeTable
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Encrypted]
        public DateTime EncryptedDate { get; set; }
    }

    [CryptoTable("FloatNumbers")]
    internal class FloatNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public float FloatMaxVal { get; set; }

        public float FloatMinVal { get; set; }

        public FloatNumbers()
        {
            FloatMaxVal = 711.411f;
            FloatMinVal = -2372.232f;
        }
    }

    [CryptoTable("DoubleNumbers")]
    internal class DoubleNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public double DoubleMaxVal { get; set; }

        public double DoubleMinVal { get; set; }

        public DoubleNumbers()
        {
            DoubleMaxVal = 16123.1281;
            DoubleMinVal = -1223.14134;
        }
    }

    [CryptoTable("AccountsData")]
    internal class AccountsData
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Column("Name")]
        public string AccountName { get; set; }

        [Encrypted, Column("Password")]
        public string AccountPassword { get; set; }

        [Encrypted]
        public uint SocialSecureId { get; set; }

        public int Age { get; set; }

        public bool IsAdministrator { get; set; }

        [Ignored]
        public string IgnoredString { get; set; }
    }
}
