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
            ShowAnimals();
        }

        // 顯示動物園清單 (點擊按鈕後執行)
        //private void ShowZooS(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        MessageBox.Show("連接字串成功");
        //        string query = "SELECT * FROM Zoo";
        //        SqlCommand command = new SqlCommand(query, SqlConnection); // 連線 + 執行查詢
        //        SqlConnection.Open();


        //        SqlDataReader reader = command.ExecuteReader(); // 讀取資料
        //        while (reader.Read())
        //        {
        //            int id = (int)reader["Id"];
        //            string location = (string)reader["Location"];
        //            //MessageBox.Show($"{id}{location}");
        //            //Console.WriteLine($"{id} | {location}");
        //        }
        //    }
        //    catch (Exception ex)
        //    { 
        //        MessageBox.Show(ex.Message);
        //    }

        //}

        // 顯示動物園清單 (自動執行)
        //private void ShowZooS()
        //{
        //    string query = "SELECT * FROM Zoo";
        //    SqlDataAdapter sqlDataReader = new SqlDataAdapter(query, SqlConnection);

        //    using (sqlDataReader)
        //    {
        //        DataTable zooTable = new DataTable();
        //        sqlDataReader.Fill(zooTable);

        //        // 使用自動生成的列
        //        listZoos.Items.Clear();
        //        foreach (DataRow row in zooTable.Rows)
        //        {
        //            // 每一項的顯示格式: Id - Location
        //            listZoos.Items.Add($"{row["Id"]}  {row["Location"]}");
        //        }
        //    }
        //}

        // 顯示動物園清單 (自動執行)
        private void ShowZooS()
        {
            try 
            {
                string query = "SELECT * FROM Zoo";
                SqlDataAdapter sqlDataReader = new SqlDataAdapter(query, SqlConnection);

                using (sqlDataReader)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataReader.Fill(zooTable);

                    listZoos.DisplayMemberPath = "Location"; // 設定顯示格式
                    listZoos.SelectedValuePath = "Id"; // 設定顯示格式


                    // 使用自動生成的列
                    listZoos.ItemsSource = zooTable.DefaultView; // 設定資料來源
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowAnimals()
        {
            try
            {
                string query = "SELECT * FROM Animal";
                SqlDataAdapter sqlDataReader = new SqlDataAdapter(query, SqlConnection);

                using (sqlDataReader)
                {
                    DataTable AnimelTable = new DataTable();
                    sqlDataReader.Fill(AnimelTable);

                    ListAnimals.DisplayMemberPath = "Name"; // 設定顯示格式
                    ListAnimals.SelectedValuePath = "Id"; // 設定顯示格式


                    // 使用自動生成的列
                    ListAnimals.ItemsSource = AnimelTable.DefaultView; // 設定資料來源
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowAssocatedAnimals()
        {
            try
            {
                //  
                string query = "SELECT * FROM Animal a " +
                    "INNER JOIN ZooAnimal za ON a.Id = za.AnimalId WHERE za.ZooId = @ZooId";
                SqlCommand sqlcommand = new SqlCommand(query, SqlConnection);
                SqlDataAdapter sqlDataReader = new SqlDataAdapter(sqlcommand);

                using (sqlDataReader)
                {
                    sqlcommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue); // 設定參數

                    DataTable AnimelTable = new DataTable();

                    sqlDataReader.Fill(AnimelTable);

                    Animalsaaa.DisplayMemberPath = "Name"; // 設定顯示格式
                    Animalsaaa.SelectedValuePath = "Id"; // 設定顯示格式

                    // 使用自動生成的列
                    Animalsaaa.ItemsSource = AnimelTable.DefaultView; // 設定資料來源
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssocatedAnimals();
        }

        private void Animels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

