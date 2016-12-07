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

        public static ShortNumbers GetDefault()
        {
            return new ShortNumbers {ShortMaxVal = short.MaxValue, ShortMinVal = short.MinValue};
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

        public static ShortEncryptedNumbers GetDefault()
        {
            return new ShortEncryptedNumbers {ShortMaxVal = short.MaxValue, ShortMinVal = short.MinValue};
        }
    }

    [CryptoTable("UShortNumbers")]
    internal class UShortNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public ushort UShortMaxVal { get; set; }
        public ushort UShortMinVal { get; set; }

        public static UShortNumbers GetDefault()
        {
            return new UShortNumbers {UShortMaxVal = ushort.MaxValue, UShortMinVal = ushort.MinValue};
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

        public static UShortEncryptedNumbers GetDefault()
        {
            return new UShortEncryptedNumbers {UShortMaxVal = ushort.MaxValue, UShortMinVal = ushort.MinValue};
        }
    }

    [CryptoTable("IntNumbers")]
    internal class IntNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public int IntMaxVal { get; set; }

        public int IntMinVal { get; set; }

        public static IntNumbers GetDefault()
        {
            return new IntNumbers {IntMinVal = int.MinValue, IntMaxVal = int.MaxValue};
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

        public static IntEncryptedNumbers GetDefault()
        {
            return new IntEncryptedNumbers {IntMinVal = int.MinValue, IntMaxVal = int.MaxValue};
        }
    }

    [CryptoTable("UIntNumbers")]
    internal class UIntNumbers
    {
        [PrimaryKey, AutoIncremental]
        public uint Id { get; set; }

        public uint UIntMaxVal { get; set; }

        public uint UIntMinVal { get; set; }

        public static UIntNumbers GetDefault()
        {
            return new UIntNumbers {UIntMaxVal = uint.MaxValue, UIntMinVal = uint.MinValue};
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

        public static UIntEncryptedNumbers GetDefault()
        {
            return new UIntEncryptedNumbers {UIntMinVal = uint.MinValue, UIntMaxVal = uint.MaxValue};
        }
    }

    [CryptoTable("ULongNumbers")]
    internal class ULongNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public ulong ULongMaxVal { get; set; }

        public ulong ULongMinVal { get; set; }

        public static ULongNumbers GetDefault()
        {
            return new ULongNumbers {ULongMinVal = ulong.MinValue, ULongMaxVal = ulong.MaxValue};
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

        public static ULongEncryptedNumbers GetDefault()
        {
            return new ULongEncryptedNumbers {ULongMinVal = ulong.MinValue, ULongMaxVal = ulong.MaxValue};
        }
    }

    [CryptoTable("LongNumbers")]
    internal class LongNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public long LongMaxVal { get; set; }

        public long LongMinVal { get; set; }

        public static LongNumbers GetDefault()
        {
            return new LongNumbers {LongMaxVal = long.MaxValue, LongMinVal = long.MinValue};
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

        public static LongEncryptedNumbers GetDefault()
        {
            return new LongEncryptedNumbers {LongMaxVal = long.MaxValue, LongMinVal = long.MinValue};
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

        public static FloatNumbers GetDefault()
        {
            return new FloatNumbers {FloatMinVal = float.MinValue, FloatMaxVal = float.MaxValue};
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

        public static FloatEncryptedNumbers GetDefault()
        {
            return new FloatEncryptedNumbers { FloatMinVal = float.MinValue, FloatMaxVal = float.MaxValue };
        }
    }

    [CryptoTable("DoubleNumbers")]
    internal class DoubleNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public double DoubleMaxVal { get; set; }

        public double DoubleMinVal { get; set; }

        public static DoubleNumbers GetDefault()
        {
            return new DoubleNumbers {DoubleMaxVal = double.MaxValue, DoubleMinVal = double.MinValue};
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

        public static DoubleEncryptedNumbers GetDefault()
        {
            return new DoubleEncryptedNumbers { DoubleMaxVal = double.MaxValue, DoubleMinVal = double.MinValue };
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

        public static BoolEncryptedNumbers GetDefault()
        {
            return new BoolEncryptedNumbers {B1 = false, B2 = true, B3 = false, B4 = true};
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

        public static ByteEncryptedNumbers GetDefault()
        {
            return new ByteEncryptedNumbers {ByteMaxVal = byte.MaxValue, ByteMinVal = Byte.MinValue};
        }
    }

    [CryptoTable("ByteNumbers")]
    internal class ByteNumbers
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public byte ByteMaxVal { get; set; }

        public byte ByteMinVal { get; set; }

        public static ByteNumbers GetDefault()
        {
            return new ByteNumbers {ByteMaxVal = byte.MaxValue, ByteMinVal = byte.MinValue};
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

    }

    

    [CryptoTable("AccountsData")]
    public class AccountsData
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

    internal static class TestExtensions
    {
        public static bool IsTableEqualsTo(this AccountsData ac1, AccountsData ac2)
        {
            var namesAreEqual = false;

            if (ac1.AccountName == null && ac2.AccountName == null)
                namesAreEqual = true;
            else if (ac1.AccountName != null && ac2.AccountName != null)
                namesAreEqual = ac1.AccountName == ac2.AccountName;

            var passwordsAreEqual = false;

            if (ac1.AccountName == null && ac2.AccountName == null)
                passwordsAreEqual = true;
            else if (ac1.AccountPassword != null && ac2.AccountPassword != null)
                passwordsAreEqual = ac1.AccountPassword == ac2.AccountPassword;

            return ac1.IsAdministrator == ac2.IsAdministrator &&
                   namesAreEqual && passwordsAreEqual &&
                   ac1.Age == ac2.Age &&
                   ac1.SocialSecureId == ac2.SocialSecureId;
        }

        public static bool IsTaskEqualTo(this SecretTask left, SecretTask right)
        {
            if (left == null || right == null)
                return false;

            return left.IsDone == right.IsDone &&
                   left.Description == right.Description &&
                   left.SecretToDo == right.SecretToDo &&
                   Math.Abs(left.Price - right.Price) < 0.000005;
        }
    }
}
