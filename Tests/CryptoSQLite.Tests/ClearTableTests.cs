using System.Linq;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    
    public class ClearTableTests : BaseTest
    {
        [Fact]
        public void ClearTableContent()
        {
            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                Name = "Frodo Beggins",
                Password = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };

            var account2 = new AccountsData
            {
                Id = 66,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = uint.MaxValue,
                Name = "Gendalf Gray",
                Password = "I am master of Anor flame.",
                Age = 27,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(account1);
                    db.InsertItem(account2);

                    var table = db.Table<AccountsData>().ToArray();

                    Assert.Contains(table, e => e.Equals(account1));
                    Assert.Contains(table, e => e.Equals(account2));

                    db.ClearTable<AccountsData>();

                    table = db.Table<AccountsData>().ToArray();
                    Assert.True(table != null);
                    Assert.True(table.Length == 0);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
