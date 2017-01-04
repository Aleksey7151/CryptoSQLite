using System;

namespace CryptoSQLite.CrossTests.Tables
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

        public short? NullAble1 { get; set; }

        public short? NullAble2 { get; set; }

        public static ShortNumbers GetDefault()
        {
            return new ShortNumbers
            {
                ShortMaxVal = short.MaxValue,
                ShortMinVal = short.MinValue,
                NullAble2 = short.MinValue,
                NullAble1 = null
            };
        }

        public bool Equals(ShortNumbers o)
        {
            if (o == null) return false;

            return ShortMaxVal == o.ShortMaxVal &&
                   ShortMinVal == o.ShortMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
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

        [Encrypted]
        public short? NullAble1 { get; set; }

        [Encrypted]
        public short? NullAble2 { get; set; }

        public static ShortEncryptedNumbers GetDefault()
        {
            return new ShortEncryptedNumbers
            {
                ShortMaxVal = short.MaxValue,
                ShortMinVal = short.MinValue,
                NullAble1 = null,
                NullAble2 = short.MaxValue
            };
        }

        public bool Equals(ShortEncryptedNumbers o)
        {
            if (o == null) return false;

            return ShortMaxVal == o.ShortMaxVal &&
                   ShortMinVal == o.ShortMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("UShortNumbers")]
    internal class UShortNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public ushort UShortMaxVal { get; set; }

        public ushort UShortMinVal { get; set; }

        public ushort? NullAble1 { get; set; }

        public ushort? NullAble2 { get; set; }

        public static UShortNumbers GetDefault()
        {
            return new UShortNumbers
            {
                UShortMaxVal = ushort.MaxValue,
                UShortMinVal = ushort.MinValue,
                NullAble1 = ushort.MaxValue,
                NullAble2 = null
            };
        }

        public bool Equals(UShortNumbers o)
        {
            if (o == null) return false;

            return UShortMaxVal == o.UShortMaxVal &&
                   UShortMinVal == o.UShortMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
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

        [Encrypted]
        public ushort? NullAble1 { get; set; }

        [Encrypted]
        public ushort? NullAble2 { get; set; }

        public static UShortEncryptedNumbers GetDefault()
        {
            return new UShortEncryptedNumbers
            {
                UShortMaxVal = ushort.MaxValue,
                UShortMinVal = ushort.MinValue,
                NullAble2 = null,
                NullAble1 = ushort.MaxValue
            };
        }

        public bool Equals(UShortEncryptedNumbers o)
        {
            if (o == null) return false;

            return UShortMaxVal == o.UShortMaxVal &&
                   UShortMinVal == o.UShortMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("IntNumbers")]
    internal class IntNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public int IntMaxVal { get; set; }

        public int IntMinVal { get; set; }

        public int? NullAble1 { get; set; }

        public int? NullAble2 { get; set; }

        public static IntNumbers GetDefault()
        {
            return new IntNumbers
            {
                IntMinVal = int.MinValue,
                IntMaxVal = int.MaxValue,
                NullAble2 = null,
                NullAble1 = int.MinValue
            };
        }
        public bool Equals(IntNumbers o)
        {
            if (o == null) return false;

            return IntMaxVal == o.IntMaxVal &&
                   IntMinVal == o.IntMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
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

        [Encrypted]
        public int? NullAble1 { get; set; }

        [Encrypted]
        public int? NullAble2 { get; set; }

        public static IntEncryptedNumbers GetDefault()
        {
            return new IntEncryptedNumbers
            {
                IntMinVal = int.MinValue,
                IntMaxVal = int.MaxValue,
                NullAble2 = null,
                NullAble1 = int.MinValue
            };
        }

        public bool Equals(IntEncryptedNumbers o)
        {
            if (o == null) return false;

            return IntMaxVal == o.IntMaxVal &&
                   IntMinVal == o.IntMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("UIntNumbers")]
    internal class UIntNumbers
    {
        [PrimaryKey, AutoIncremental]
        public uint Id { get; set; }

        public uint UIntMaxVal { get; set; }

        public uint UIntMinVal { get; set; }

        public uint? NullAble1 { get; set; }

        public uint? NullAble2 { get; set; }

        public static UIntNumbers GetDefault()
        {
            return new UIntNumbers
            {
                UIntMaxVal = uint.MaxValue,
                UIntMinVal = uint.MinValue,
                NullAble2 = null,
                NullAble1 = uint.MinValue
            };
        }

        public bool Equals(UIntNumbers o)
        {
            if (o == null) return false;

            return UIntMaxVal == o.UIntMaxVal &&
                   UIntMinVal == o.UIntMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
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

        [Encrypted]
        public uint? NullAble1 { get; set; }

        [Encrypted]
        public uint? NullAble2 { get; set; }

        public static UIntEncryptedNumbers GetDefault()
        {
            return new UIntEncryptedNumbers
            {
                UIntMinVal = uint.MinValue,
                UIntMaxVal = uint.MaxValue,
                NullAble2 = null,
                NullAble1 = uint.MaxValue
            };
        }

        public bool Equals(UIntEncryptedNumbers o)
        {
            if (o == null) return false;

            return UIntMaxVal == o.UIntMaxVal &&
                   UIntMinVal == o.UIntMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("ULongNumbers")]
    internal class ULongNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public ulong ULongMaxVal { get; set; }

        public ulong ULongMinVal { get; set; }

        public ulong? NullAble1 { get; set; }

        public ulong? NullAble2 { get; set; }

        public static ULongNumbers GetDefault()
        {
            return new ULongNumbers
            {
                ULongMinVal = ulong.MinValue,
                ULongMaxVal = ulong.MaxValue,
                NullAble2 = null,
                NullAble1 = ulong.MaxValue
            };
        }

        public bool Equals(ULongNumbers o)
        {
            if (o == null) return false;

            return ULongMaxVal == o.ULongMaxVal &&
                   ULongMinVal == o.ULongMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
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

        [Encrypted]
        public ulong? NullAble1 { get; set; }

        [Encrypted]
        public ulong? NullAble2 { get; set; }

        public static ULongEncryptedNumbers GetDefault()
        {
            return new ULongEncryptedNumbers
            {
                ULongMinVal = ulong.MinValue,
                ULongMaxVal = ulong.MaxValue,
                NullAble2 = null,
                NullAble1 = ulong.MaxValue
            };
        }

        public bool Equals(ULongEncryptedNumbers o)
        {
            if (o == null) return false;

            return ULongMaxVal == o.ULongMaxVal &&
                   ULongMinVal == o.ULongMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("LongNumbers")]
    internal class LongNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public long LongMaxVal { get; set; }

        public long LongMinVal { get; set; }

        public long? NullAble1 { get; set; }

        public long? NullAble2 { get; set; }

        public static LongNumbers GetDefault()
        {
            return new LongNumbers
            {
                LongMaxVal = long.MaxValue,
                LongMinVal = long.MinValue,
                NullAble2 = null,
                NullAble1 = long.MinValue
            };
        }

        public bool Equals(LongNumbers o)
        {
            if (o == null) return false;

            return LongMaxVal == o.LongMaxVal &&
                   LongMinVal == o.LongMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
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

        [Encrypted]
        public long? NullAble1 { get; set; }

        [Encrypted]
        public long? NullAble2 { get; set; }

        public static LongEncryptedNumbers GetDefault()
        {
            return new LongEncryptedNumbers
            {
                LongMaxVal = long.MaxValue,
                LongMinVal = long.MinValue,
                NullAble2 = null,
                NullAble1 = long.MaxValue
            };
        }

        public bool Equals(LongEncryptedNumbers o)
        {
            if (o == null) return false;

            return LongMaxVal == o.LongMaxVal &&
                   LongMinVal == o.LongMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("DateTimeTable")]
    internal class DateTimeTable
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime? NullAbleDate { get; set; }

        public bool Equals(DateTimeTable o)
        {
            if (o == null) return false;

            return Date == o.Date &&
                   NullAbleDate == o.NullAbleDate;
        }
    }

    [CryptoTable("DateTimeEncryptedTable")]
    internal class DateTimeEncryptedTable
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public DateTime Date { get; set; }

        [Encrypted]
        public DateTime? NullAbleDate { get; set; }

        public bool Equals(DateTimeEncryptedTable o)
        {
            if (o == null) return false;

            return Date == o.Date &&
                   NullAbleDate == o.NullAbleDate;
        }
    }

    [CryptoTable("FloatNumbers")]
    internal class FloatNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public float FloatMaxVal { get; set; }

        public float FloatMinVal { get; set; }

        public float? NullAble1 { get; set; }

        public float? NullAble2 { get; set; }

        public static FloatNumbers GetDefault()
        {
            return new FloatNumbers
            {
                FloatMinVal = float.MinValue,
                FloatMaxVal = float.MaxValue,
                NullAble2 = null,
                NullAble1 = float.MaxValue
            };
        }

        public bool Equals(FloatNumbers o)
        {
            if (o == null) return false;

            return Math.Abs(FloatMaxVal - o.FloatMaxVal) < 0.0001 &&
                   Math.Abs(FloatMinVal - o.FloatMinVal) < 0.0001 &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("FloatEncryptedNumbers")]
    internal class FloatEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public float FloatMaxVal { get; set; }

        [Encrypted]
        public float FloatMinVal { get; set; }

        [Encrypted]
        public float? NullAble1 { get; set; }

        [Encrypted]
        public float? NullAble2 { get; set; }

        public static FloatEncryptedNumbers GetDefault()
        {
            return new FloatEncryptedNumbers
            {
                FloatMinVal = float.MinValue,
                FloatMaxVal = float.MaxValue,
                NullAble2 = null,
                NullAble1 = float.MaxValue
            };
        }

        public bool Equals(FloatEncryptedNumbers o)
        {
            if (o == null) return false;

            return Math.Abs(FloatMaxVal - o.FloatMaxVal) < 0.0001 &&
                   Math.Abs(FloatMinVal - o.FloatMinVal) < 0.0001 &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("DoubleNumbers")]
    internal class DoubleNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public double DoubleMaxVal { get; set; }

        public double DoubleMinVal { get; set; }

        public double? NullAble1 { get; set; }

        public double? NullAble2 { get; set; }

        public static DoubleNumbers GetDefault()
        {
            return new DoubleNumbers
            {
                DoubleMaxVal = double.MaxValue,
                DoubleMinVal = double.MinValue,
                NullAble2 = null,
                NullAble1 = double.MinValue
            };
        }

        public bool Equals(DoubleNumbers o)
        {
            if (o == null) return false;

            return Math.Abs(DoubleMaxVal - o.DoubleMaxVal) < 0.000001 &&
                   Math.Abs(DoubleMinVal - o.DoubleMinVal) < 0.000001 &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("DoubleEncryptedNumbers")]
    internal class DoubleEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public double DoubleMaxVal { get; set; }

        [Encrypted]
        public double DoubleMinVal { get; set; }

        [Encrypted]
        public double? NullAble1 { get; set; }

        [Encrypted]
        public double? NullAble2 { get; set; }

        public static DoubleEncryptedNumbers GetDefault()
        {
            return new DoubleEncryptedNumbers
            {
                DoubleMaxVal = double.MaxValue,
                DoubleMinVal = double.MinValue,
                NullAble2 = null,
                NullAble1 = double.MaxValue
            };
        }

        public bool Equals(DoubleEncryptedNumbers o)
        {
            if (o == null) return false;

            return Math.Abs(DoubleMaxVal - o.DoubleMaxVal) < 0.000001 &&
                   Math.Abs(DoubleMinVal - o.DoubleMinVal) < 0.000001 &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("DecimalNumbers")]
    internal class DecimalNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public decimal MaxVal { get; set; }

        public decimal MinVal { get; set; }

        public decimal? NullAble1 { get; set; }

        public decimal? NullAble2 { get; set; }

        public static DecimalNumbers GetDefault()
        {
            return new DecimalNumbers
            {
                MaxVal = decimal.MaxValue,
                MinVal = decimal.MinValue,
                NullAble2 = null,
                NullAble1 = decimal.MinValue
            };
        }

        public bool Equals(DecimalNumbers o)
        {
            if (o == null) return false;

            return MaxVal == o.MaxVal &&
                   MinVal == o.MinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("DecimalEncryptedNumbers")]
    internal class DecimalEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public decimal MaxVal { get; set; }

        [Encrypted]
        public decimal MinVal { get; set; }

        [Encrypted]
        public decimal? NullAble1 { get; set; }

        [Encrypted]
        public decimal? NullAble2 { get; set; }

        public static DecimalEncryptedNumbers GetDefault()
        {
            return new DecimalEncryptedNumbers
            {
                MaxVal = decimal.MaxValue,
                MinVal = decimal.MinValue,
                NullAble2 = null,
                NullAble1 = decimal.MinValue
            };
        }

        public bool Equals(DecimalEncryptedNumbers o)
        {
            if (o == null) return false;

            return MaxVal == o.MaxVal &&
                   MinVal == o.MinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("BoolEncryptedNumbers")]
    internal class BoolEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public bool B1 { get; set; }

        [Encrypted]
        public bool B2 { get; set; }

        public bool B3 { get; set; }

        public bool B4 { get; set; }

        public bool? B5 { get; set; }

        [Encrypted]
        public bool? B6 { get; set; }

        public bool? B7 { get; set; }

        [Encrypted]
        public bool? B8 { get; set; }

        public static BoolEncryptedNumbers GetDefault()
        {
            return new BoolEncryptedNumbers
            {
                B1 = false,
                B2 = true,
                B3 = false,
                B4 = true,
                B5 = null,
                B6 = null,
                B7 = false,
                B8 = true
            };
        }

        public bool Equals(BoolEncryptedNumbers o)
        {
            if (o == null) return false;

            return B1 == o.B1 &&
                   B2 == o.B2 &&
                   B3 == o.B3 &&
                   B4 == o.B4 &&
                   B5 == o.B5 &&
                   B6 == o.B6 &&
                   B7 == o.B7 &&
                   B8 == o.B8;
        }
    }

    [CryptoTable("ByteEncryptedNumbers")]
    internal class ByteEncryptedNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public byte ByteMaxVal { get; set; }

        [Encrypted]
        public byte ByteMinVal { get; set; }

        [Encrypted]
        public byte? NullAble1 { get; set; }

        [Encrypted]
        public byte? NullAble2 { get; set; }

        public static ByteEncryptedNumbers GetDefault()
        {
            return new ByteEncryptedNumbers
            {
                ByteMaxVal = byte.MaxValue,
                ByteMinVal = byte.MinValue,
                NullAble2 = null,
                NullAble1 = byte.MaxValue
            };
        }

        public bool Equals(ByteEncryptedNumbers o)
        {
            if (o == null) return false;

            return ByteMaxVal == o.ByteMaxVal &&
                   ByteMinVal == o.ByteMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("ByteNumbers")]
    internal class ByteNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public byte ByteMaxVal { get; set; }

        public byte ByteMinVal { get; set; }

        public byte? NullAble1 { get; set; }

        public byte? NullAble2 { get; set; }

        public static ByteNumbers GetDefault()
        {
            return new ByteNumbers
            {
                ByteMaxVal = byte.MaxValue,
                ByteMinVal = byte.MinValue,
                NullAble2 = null,
                NullAble1 = byte.MaxValue
            };
        }

        public bool Equals(ByteNumbers o)
        {
            if (o == null) return false;

            return ByteMaxVal == o.ByteMaxVal &&
                   ByteMinVal == o.ByteMinVal &&
                   NullAble1 == o.NullAble1 &&
                   NullAble2 == o.NullAble2;
        }
    }

    [CryptoTable("BlobData")]
    internal class BlobData
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public byte[] OpenBlob { get; set; }

        [Encrypted]
        public byte[] ClosedBlob { get; set; }

        public byte SingleByte { get; set; }
    }

    [CryptoTable("SecretTasks")]
#pragma warning disable 660,661
    public class SecretTask
#pragma warning restore 660,661
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string SecretToDo { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public bool IsDone { get; set; }

        public bool Equal(SecretTask s)
        {
            return SecretToDo == s.SecretToDo && Description == s.Description && Math.Abs(Price - s.Price) < 0.000001 &&
                   IsDone == s.IsDone;
        }

    }



    [CryptoTable("AccountsData")]
    public class AccountsData
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Encrypted, Column("Password")]
        public string Password { get; set; }

        public uint? SocialSecureId { get; set; }

        public double? Productivity { get; set; }

        public int Posts { get; set; }

        public ushort Age { get; set; }

        public ulong? Salary { get; set; }

        public bool IsAdministrator { get; set; }

        [Ignored]
        public string IgnoredString { get; set; }

        public bool Equal(AccountsData ac)
        {
            return IsAdministrator == ac.IsAdministrator && Name == ac.Name && Password == ac.Password &&
                   Age == ac.Age &&
                   SocialSecureId == ac.SocialSecureId;
        }
    }

   
}
