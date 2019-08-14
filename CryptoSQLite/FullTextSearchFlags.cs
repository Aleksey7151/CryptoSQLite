using System;

namespace CryptoSQLite
{
    /// <summary>
    /// FTS3 and FTS4 are SQLite virtual table modules that allows users to perform full-text searches.
    /// <para/>The FTS3 and FTS4 extension modules allows users to create special tables with a built-in full-text index (hereafter "FTS tables").
    /// <para/>The full-text index allows the user to efficiently query the database for all rows that contain one or more words (hereafter "tokens"), 
    /// even if the table contains many large documents.
    /// </summary>
    [Flags]
    internal enum FullTextSearchFlags
    {
        /// <summary>
        /// Create ordinary table
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Create virtual table using FTS3.
        /// <para/>Attention, for newer applications, FTS4 is recommended.
        /// </summary>
        FTS3 = 0x0001,

        /// <summary>
        /// Create virtual table using FTS4.
        /// <para/>For newer applications, FTS4 is recommended.
        /// <para/>FTS4 contains query performance optimizations that may significantly improve the performance of full-text queries that contain terms that are very common (present in a large percentage of table rows).
        /// </summary>
        FTS4 = 0x0002
    }
}