using Foundation;
using System;
using UIKit;

namespace CryptoSQLite.Tests.Native.iOS
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            BtnCheckSQLite.TouchUpInside += BtnCheckSQLiteOnTouchUpInside;
        }

        private void BtnCheckSQLiteOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            var tests = new SQLiteTester();

            tests.StartSQLiteTests("DataBase.db3");
        }
    }
}