using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CryptoSQLite
{
    /// <summary>
    /// Interface of SQLite connection to database file with data encryption
    /// </summary>
    public interface ICryptoSQLite : IDisposable
    {
        /// <summary>
        /// Sets the encryption key for all tables in database file. That key will be used for encryption data for all tables, that don't have specific encryption key.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Sets the specific encryption key for specific table. This key will be used for encryption data only for specified table: <typeparamref name="TTable"></typeparamref>.
        /// That allows you to set up different encryption keys for different tables.
        /// This feature significantly increases data security.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <typeparam name="TTable">Table for which Encryption Key will be set</typeparam>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey<TTable>(byte[] key) where TTable : class;

        /// <summary>
        /// Creates a new ordinary table (if it not already exists) in database, that can contain encrypted columns.
        /// <para/>Warning! If table contains any Properties marked as [Encrypted], so 
        /// this table will be containing automatically generated column with name: "SoltColumn". 
        /// <para/>SoltColumn is used in encryption algoritms. If you change value of this column you
        /// won't be able to decrypt data.
        /// <para/>Warning! If you insert element in the table, and then change Properties order in table type (in your class),
        /// you won't be able to decrypt elements. Properties order in table is important thing.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        void CreateTable<TTable>() where TTable : class;

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteTable<TTable>() where TTable : class;

        /// <summary>
        /// Deletes all data inside the table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        void ClearTable<TTable>() where TTable : class;

        /// <summary>
        /// Returns the number of records in table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <returns></returns>
        int Count<TTable>() where TTable : class;

        /// <summary>
        /// Returns the number of records in table that satisfying the condition defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="predicate">Condition</param>
        /// <returns></returns>
        int Count<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the number of values (NULL values won't be counted) for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        int Count<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the number of distinct values for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        int CountDistinct<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Inserts new element (row) in table.
        /// The table must be already created.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void InsertItem<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Inserts or replaces the element if it exists in database.
        /// <para/>If you insert element with setted PrimaryKey value this element will replace element in database, that has same PrimaryKey value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void InsertOrReplaceItem<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Updates the row(s) in table <typeparamref name="TTable"/> with values from <paramref name="item"/>.
        /// <para/>LIMITATIONS: This function updates all columns in a row with values from <paramref name="item"/>. We can't specify names of columns which values we want to update. That's because some columns can be encrypted.
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="item">Element with new values for updating.</param>
        /// <param name="predicate">Condition which determines row(s) that must be updated.</param>
        void Update<TTable>(TTable item, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Removes from table <typeparamref name="TTable"/> all elements which column values satisfy conditions defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table in which elements will be removed</typeparam>
        /// <param name="predicate">condition for column values</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        void Delete<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void Delete<TTable>(string columnName, object columnValue) where TTable : class;

        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        IEnumerable<TTable> Table<TTable>() where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber, Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> contain value == <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all rows which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        IEnumerable<TTable> FindByValue<TTable>(string columnName, object columnValue) where TTable : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningOnCondition"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Left table type</typeparam>
        /// <typeparam name="TTable2">Right table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join's result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningOnCondition">The relationship between the two tables for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines view of returning result.</param>
        /// <returns>Results of Jioning request</returns>
        IEnumerable<TJoinResult> Join<TTable1, TTable2, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningOnCondition,
            Func<TTable1, TTable2, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the first table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the second table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        IEnumerable<TJoinResult> Join<TTable1, TTable2, TTable3, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Func<TTable1, TTable2, TTable3, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TTable4">Third joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the First table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the Second table for joining them.</param>
        /// <param name="joiningConditionWithTable4">The relationship between Main table and the Third table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        IEnumerable<TJoinResult> Join<TTable1, TTable2, TTable3, TTable4, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Expression<Func<TTable1, TTable4, bool>> joiningConditionWithTable4,
            Func<TTable1, TTable2, TTable3, TTable4, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new()
            where TTable4 : class, new();

        /// <summary>
        /// The LeftJoin function returns all rows from the left table <typeparamref name="TLeftTable"/>, with the matching rows in the right table <typeparamref name="TRightTable"/>. The result is NULL in the right side when there is no match
        /// </summary>
        /// <typeparam name="TLeftTable">Type of the left table</typeparam>
        /// <typeparam name="TRightTable">Type of the right table</typeparam>
        /// <typeparam name="TJoinResult">Type of joining result</typeparam>
        /// <param name="whereCondition">Where clause for left table</param>
        /// <param name="keysSelectorExpression">The relationship between Left table and the Right table for joining them.</param>
        /// <param name="joiningResult">Delegate that determines the view of the joining result.</param>
        /// <returns></returns>
        IEnumerable<TJoinResult> LeftJoin<TLeftTable, TRightTable, TJoinResult>(
            Expression<Predicate<TLeftTable>> whereCondition,
            Expression<Func<TLeftTable, TRightTable, bool>> keysSelectorExpression,
            Func<TLeftTable, TRightTable, TJoinResult> joiningResult)
            where TLeftTable : class, new()
            where TRightTable : class, new();


        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Properties whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Select<TTable>(Expression<Predicate<TTable>> predicate,
            params Expression<Func<TTable, object>>[] selectedProperties)
            where TTable : class, new();

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="count">Count of first elements, that will be returned.</param>
        /// <returns></returns>
        IEnumerable<TTable> SelectTop<TTable>(int count) where TTable : class, new();

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        double Max<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Max value.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        double Max<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        double Min<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Min value.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        double Min<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        double Sum<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Summary value.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        double Sum<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        double Avg<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining average value.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        double Avg<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;
    }
}