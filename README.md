# CryptoSQLite Plugin

This component provides encryption and decryption of columns in SQLite database Table. 

## Current Status

This package is released and considered as stable.

## Authors

* Aliaksei Safonau, https://github.com/Aleksey7151

## Requirements

* Android 4.3+
* iOS 9.3+

## Quick Start

This package is constructed in a Cross way:

```C#
// ============================================================= //
//                      IMPORTANT                                //
// IN ORDER TO KEEP ENCRYPTION KEY IN SECRET YOU MUST            //
// CALL "DISPOSE" FUNCTION AFTER USING CryptoSQLiteConnection.   //
// ============================================================= //

// ================== INITIALIZATON

// ANDROID PLATFORM
public class MainActivity : Activity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);

		CryptoSQLiteFactory.Current.Init();  // in this case you will have to provide full path to your database file when you will be calling "Create" method
		// OR !!
		CryptoSQLiteFactory.Current.Init(Environment.ExternalStorageDirectory.Path); // You can provide any other path to your folder with database files.
	}
}

// iOS PLATFORM
public partial class AppDelegate
{
	public override bool FinishedLaunching(UIApplication app, NSDictionary options)
	{
		var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
		var libraryPath = Path.Combine(documentsPath, "..", "Library");                    // Library folder
		CryptoSQLiteFactory.Current.Init(libraryPath);

		return base.FinishedLaunching(app, options);
	}
}

// =================== USAGE IN CORE PROJECT

// Your random Key
private readonly byte[] _key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };

// Your Table
[CryptoTable("Employees")]
internal class Employee
{
	[PrimaryKey, AutoIncremental]
	public int Id { get; set; }

	public string FullName { get; set; }

	[Encrypted]
	public double Salary { get; set; }	// this field will be encrypted before storing in SQLite database

	[Encrypted]
	public string Address { get; set; } // this field will be encrypted before storing in SQLite database
}

var connection = CryptoSQLiteFactory.Current.Create("MyDatabase.db3", CryptoAlgorithms.AesWith256BitsKey);
connection.SetEncryptionKey(_key);

connection.CreateTable<Employee>();
connection.InsertItem(new Employe{FullName = "Patric"});

var result = connection.Find<Employee>(e => e.FullName == "Patric");

connection.Dispose(); // Clears the copy of encryption key that lies in CryptoStorage library

```

## Solution Structure
* Platform independent code: .NET Standart 2.0 library.
* Android specific code: Android class library.
* iOS specific code: iOS class library.

## Installation
You need to add the **CryptoSQLite** package to your iOS / Android projects and cross-platform projects

## Package dependencies
Android:
* SQLitePCL.pretty.netstandard version 2.0.1
* SQLitePCLRaw.bundle_green version 2.0.0
* SQLitePCLRaw.core version 2.0.0
* SQLitePCLRaw.lib.e_sqlite3.android version 2.0.0

iOS:
* SQLitePCL.pretty.netstandard version 2.0.1
* SQLitePCLRaw.bundle_green version 2.0.0

NET Standard:
* SQLitePCL.pretty.netstandard version 2.0.1
