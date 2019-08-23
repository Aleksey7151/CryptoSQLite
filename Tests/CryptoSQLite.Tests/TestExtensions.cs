namespace CryptoSQLite.Tests
{
    internal static class TestExtensions
    {
        public static bool Compare(this string value, string target)
        {
            if (value == null && target == null)
            {
                return true;
            }

            return string.Equals(value, target);
        }
    }
}
