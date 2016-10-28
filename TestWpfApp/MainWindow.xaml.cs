using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CryptoSQLite;

namespace TestWpfApp
{
    [CryptoTable("Jobs")]
    public class Jobs
    {
        [Column("id"), PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string JobName { get; set; }

        [Encrypted]
        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CryptoSQLiteAsyncConnection _connection;
        public MainWindow()
        {
            

            InitializeComponent();

            

            BtnAdd.Click += BtnAddOnClick;

            
        }

        public override async void EndInit()
        {
            base.EndInit();

            _connection = new CryptoSQLiteAsyncConnection("myDb.db3");

            var key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };

            _connection.SetEncryptionKey(key);

            await _connection.CreateTableAsync<Jobs>();

            var items = await _connection.TableAsync<Jobs>();

            foreach (var item in items)
            {
                ListJobs.AppendText($"{item.Id} {item.JobName} {item.Description} {item.IsCompleted}\n");
            }
        }

        private async void BtnAddOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var item = new Jobs() {JobName = TbJob.Text, Description = TbDescr.Text, IsCompleted = false};
            await _connection.InsertItemAsync(item);

           
        }
    }
}
