using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class ClearTableTests : BaseTest
    {
        [Test]
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

                    Assert.IsTrue(table.Any(e => e.Equals(account1)));
                    Assert.IsTrue(table.Any(e => e.Equals(account2)));

                    db.ClearTable<AccountsData>();

                    table = db.Table<AccountsData>().ToArray();
                    Assert.IsTrue(table != null);
                    Assert.IsTrue(table.Length == 0);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
