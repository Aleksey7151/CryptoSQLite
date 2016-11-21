// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace CryptoSQLite.CrossTests.Droid
{
    [Activity(Label = "NUnit 3.0", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light",
         ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                SQLitePCL.Batteries_V2.Init();      // From NuGet Package SQLitePCLRaw.bundle_green by Eric Sink
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // This will load all tests within the current project
            var nunit = new NUnit.Runner.App();
            //nunit.AutoRun = true;

            // If you want to add tests in another assembly
            nunit.AddTestAssembly(typeof(CryptoSQLite.CrossTests.InsertItemTests).Assembly);

            // Do you want to automatically run tests when the app starts?

            LoadApplication(nunit);
        }
    }
}

