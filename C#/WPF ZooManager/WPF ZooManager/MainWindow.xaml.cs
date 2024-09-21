using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Data.SqlClient;
using System.Data; // enable connection to SQL Server

namespace WPF_ZooManager
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection SqlConnection; // create a connection to SQL Server

        public MainWindow()
        {
            InitializeComponent();
            // 連接字串 
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_ZooManager.Properties.Settings.PanjutorialsDBConnectionString"].ConnectionString;
            MessageBox.Show(connectionString,"連接字串成功");
            SqlConnection = new SqlConnection(connectionString); // 
            ShowZooS();
        }

        //private void ShowZooS(object sender , RoutedEventArgs e)

        //{
        //    MessageBox.Show("連接字串成功");            
        //    string query = "SELECT * FROM Zoo";
        //    SqlCommand command = new SqlCommand(query, SqlConnection); // 連線 + 執行查詢
        //    SqlConnection.Open();


        //    SqlDataReader reader = command.ExecuteReader(); // 讀取資料
        //    while (reader.Read())
        //    {
        //        int id = (int)reader["Id"];
        //        string location = (string)reader["Location"];
        //        //MessageBox.Show($"{id}{location}");
        //        Console.WriteLine($"{id} | {location}");
        //    }

        //}
        private void ShowZooS()
        {
            string query = "SELECT * FROM Zoo";
            SqlDataAdapter sqlDataReader = new SqlDataAdapter(query, SqlConnection);

            using (sqlDataReader)
            {
                DataTable zooTable = new DataTable();

                sqlDataReader.Fill(zooTable);

                listZoos.DisplayMemberPath = "Location";
                listZoos.SelectedValuePath = "Id";

                listZoos.ItemsSource = zooTable.DefaultView;


            }



        }
    }
}
