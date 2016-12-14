using CryptoSQLite;

namespace Tests.Tables
{
    internal class TableWithoutCryptoTableAttribute
    {
        [PrimaryKey]
        public int Id { get; set; }
    }

    [CryptoTable("")]
    internal class TableWithEmptyName
    {
        [PrimaryKey]
        public int Id { get; set; }
    }

    [CryptoTable(null)]
    internal class TableWithNullName
    {
        [PrimaryKey]
        public int Id { get; set; }
    }

    [CryptoTable("TableWithoutPrimaryKeyColumn")]
    internal class TableWithoutPrimaryKeyColumn
    {
        [Column("ID")]
        public int Id { get; set; }
        
    }

    [CryptoTable("TableWithTwoPrimaryKeyColumns")]
    internal class TableWithTwoPrimaryKeyColumns
    {
        [PrimaryKey]
        public int Id { get; set; }

        [PrimaryKey]
        public int Age { get; set; }
    }

    [CryptoTable("TableWithSoltColumnNameFirst")]
    internal class TableWithSoltColumnNameFirst
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string SoltColumn { get; set; }
    }

    [CryptoTable("TableWithSoltColumnNameSecond")]
    internal class TableWithSoltColumnNameSecond
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Column("SoltColumn")]
        public string Solt { get; set; }
    }

    [CryptoTable("TableWithEncryptedPrimaryKey")]
    internal class TableWithEncryptedPrimaryKey
    {
        [PrimaryKey, Encrypted]
        public int Id { get; set; }
    }

    [CryptoTable("TableWithEncryptedAutoIncrementalKey")]
    internal class TableWithEncryptedAutoIncrementalKey
    {
        [PrimaryKey]
        public int Id { get; set; }

        [AutoIncremental, Encrypted]
        public int AnotherId { get; set; }
    }

    [CryptoTable("TableWithEncryptedDefaultValue")]
    internal class TableWithEncryptedDefaultValue
    {
        [PrimaryKey]
        public int Id { get; set; }

        [NotNull(123), Encrypted]
        public int AnotherId { get; set; }
    }

    [CryptoTable("TableWithEncryptedNotNullValue")]
    internal class TableWithEncryptedNotNullValue
    {
        [PrimaryKey]
        public int Id { get; set; }

        [NotNull, Encrypted]
        public int? AnotherId { get; set; }
    }

    [CryptoTable("TableWithEncryptedByteColumn")]
    internal class TableWithEncryptedByteColumn
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Encrypted]
        public byte ByteVal { get; set; }
    }

    [CryptoTable("TableWithEncryptedBoolColumn")]
    internal class TableWithEncryptedBoolColumn
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Encrypted]
        public bool BoolVal { get; set; }
    }

    [CryptoTable("TableWithTwoEqualColumnNames")]
    internal class TableWithTwoEqualColumnNames
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Column { get; set; }

        [Column("Column")]
        public string Col { get; set; }
    }

    [CryptoTable("TableWithNullColumnName")]
    internal class TableWithNullColumnName
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Column(null)]
        public int Column { get; set; }
    }

    [CryptoTable("TableWithEmptyColumnName")]
    internal class TableWithEmptyColumnName
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Column("")]
        public int Column { get; set; }
    }

    [CryptoTable("TableWithInCompatibleColumnType")]
    internal class TableWithInCompatibleColumnType
    {
        [PrimaryKey]
        public int Id { get; set; }

        public byte[] Data { get; set; }

        public int[] IncompatibleColumn { get; set; }
    }
}
