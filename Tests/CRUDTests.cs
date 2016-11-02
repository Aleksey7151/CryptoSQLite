using System;
using System.IO;
using CryptoSQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CrudTests : BaseTest
    {
        [TestMethod]
        public void CreateDataBaseFiles()
        {
            using (var db = GetGostConnection())
            {

            }
            Assert.IsTrue(File.Exists(GostDbFile));
            using (var db = GetAesConnection())
            {

            }
            Assert.IsTrue(File.Exists(AesDbFile));
        }
        
        [TestMethod]
        public void CreateCryptoTableInDatabase()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<AccountsData>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }

            using (var db = GetAesConnection())
            {
                try
                {
                    db.CreateTable<AccountsData>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }

        [TestMethod]
        public void InsertElementInCryptoTable()
        {
            var account = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = "Frodo Beggins",
                AccountPassword = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that will be ignored in table mapping"
            };

            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<AccountsData>();
                    db.InsertItem(account);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }

            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<AccountsData>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }
    }
}
