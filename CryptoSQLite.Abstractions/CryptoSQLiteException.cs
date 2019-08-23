using System;

namespace CryptoSQLite
{
    /// <summary>
    /// Class represents CryptoSQLite Exception.
    /// </summary>
    public sealed class CryptoSQLiteException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message with the reason.</param>
        /// <param name="cause">Probable cause of exception.</param>
        /// <param name="innerException">The inner exception</param>
        public CryptoSQLiteException(
            string message,
            Exception innerException = null,
            string cause = null) : base(message, innerException)
        {
            ProbableCause = cause;
        }

        /// <summary>
        /// Probable cause of exception.
        /// </summary>
        public string ProbableCause { get; }
    }
}