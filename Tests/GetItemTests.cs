using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tables;

namespace Tests
{
    [TestClass]
    public class GetItemTests : BaseTest
    {
        [TestMethod]
        public void GetItemUsingColumnName()
        {
            var item = new ShortEncryptedNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ShortEncryptedNumbers>();
                    db.CreateTable<ShortEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ShortEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ShortMaxVal == item.ShortMaxVal && element.ShortMinVal == item.ShortMinVal);

                    element.ShortMaxVal = 21379;
                    element.ShortMinVal = -1234;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<ShortEncryptedNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.ShortMaxVal == updated.ShortMaxVal && element.ShortMinVal == updated.ShortMinVal);
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

        [TestMethod]
        public void GetItemUsingIdColumn()
        {
            
        }

        public void GetItemUsingItem()
        {
            
        }
    }
}
