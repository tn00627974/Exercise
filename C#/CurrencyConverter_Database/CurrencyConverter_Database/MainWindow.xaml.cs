using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions; // 使用正規表示式 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CurrencyConverter_Database
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary> 
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection(); // 創建 SQL 連線 
        SqlCommand cmd = new SqlCommand(); // 創建 SQL 指令
        SqlDataAdapter da = new SqlDataAdapter(); // 創建 SQL DataAdapter

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            lblCurrency.Content = "請選擇您要轉換的貨幣";
            //txtAmount.Content = "金額";
            BindCurrency(); // 呼叫 BindCurrency 方法
            ClearControls();
            GetData();


        }

        // 建立mycon 基本連接資料庫設定
        public void mycon()
        {
            //Database connection string 
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open(); //Connection Open
            //MessageBox.Show("資料庫連線成功");

        }

        // Private 是一個存取修飾符。類型或成員只能由同一類別或結構中的程式碼存取。
        // Void 關鍵字用於方法簽章來聲明不傳回任何值的方法。使用 void 傳回類型宣告的方法不能為其包含的任何傳回語句提供任何參數。
        public void BindCurrency()
        {
            mycon(); // 呼叫連接資料庫的函數
            DataTable dt = new DataTable(); // 創建一個新的資料表物件，使用 System.Data 命名空間 

            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con); // 創建 SQL 命令，查詢 Currency_Master 表中的 Id 和 CurrencyName 欄位
            cmd.CommandType = CommandType.Text; // 設定命令類型為文字 SQL 查詢

            da = new SqlDataAdapter(cmd); // 創建資料適配器，將 SQL 命令與資料庫連接
            da.Fill(dt); // 使用適配器填充資料表 dt

            DataRow newRow = dt.NewRow(); // 創建一個新的資料行

            // 設定新資料行的 Id 和 CurrencyName 欄位值
            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--SELECT--";

            // 將新資料行插入到資料表的第一行
            dt.Rows.InsertAt(newRow, 0);

            if (dt != null && dt.Rows.Count > 0) // 如果資料表 dt && dt.Rows 有資料
            {
                cmbFromCurrency.ItemsSource = dt.DefaultView; // 將 DataTable 指派給 Combobox
                cmbToCurrency.ItemsSource = dt.DefaultView;  // 將 DataTable 指派給 Combobox
            }
            con.Close();    // 關閉資料庫連接

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";  // DisplayMemberPath 是用來顯示Combobox的文字
            cmbFromCurrency.SelectedValuePath = "Id"; // SelectedValuePath是用來顯示Combobox的Value值
            cmbFromCurrency.SelectedValue = 0; // SelectedValue 是用來設定Combobox的預設選取項目，預設值為0，表示Select。

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedValue = 0;


            //dtCurrency.Columns.Add("Text"); //Add display column in DataTable
            //dtCurrency.Columns.Add("Value"); // Add value column in DataTable

            ////Add rows in Datatable with text and value
            //dtCurrency.Rows.Add("--SELECT--", 0);
            //dtCurrency.Rows.Add("TWD", 30);
            //dtCurrency.Rows.Add("INR", 1);
            //dtCurrency.Rows.Add("USD", 90);
            //dtCurrency.Rows.Add("EUR", 85);
            //dtCurrency.Rows.Add("SAR", 20);
            //dtCurrency.Rows.Add("POUND", 5);
            //dtCurrency.Rows.Add("DEM", 43);

            //// cmbFromCurrency
            //cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;   // 將 DataTable 指派給 Combobox  
            //cmbFromCurrency.DisplayMemberPath = "Text";             // DisplayMemberPath 是用來顯示Combobox的文字，SelectedValuePath是用來顯示Combobox的Value值
            //cmbFromCurrency.SelectedValuePath = "Value";            // SelectedValuePath 是用來顯示Combobox的Value值，SelectedValuePath 屬性的值是 DataTable 的 Value 值
            //cmbFromCurrency.SelectedIndex = 0;                      // SelectedIndex 是用來設定Combobox的預設選取項目，預設值為0，表示Select。

            //// cmbToCurrency
            //cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            //cmbToCurrency.DisplayMemberPath = "Text";
            //cmbToCurrency.SelectedValuePath = "Value";
            //cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            // 創建一個 double 類型的變數 ConvertedValue 用於存儲貨幣轉換後的值
            double ConvertedValue;

            // 檢查 amount 文本框是否為空或空白
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                // 如果 amount 文本框為空或空白，將顯示此消息框
                MessageBox.Show("請輸入金額", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                // 點擊消息框的 OK 後，將焦點設置到 amount 文本框
                txtCurrency.Focus();
                return;
            }
            // 如果 "從" 的貨幣未選擇或選擇了預設文本 --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                // 顯示消息框
                MessageBox.Show("請選擇來源貨幣", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                // 將焦點設置到 "從" 的貨幣選擇框
                cmbFromCurrency.Focus();
                return;
            }
            // 如果 "到" 的貨幣未選擇或選擇了預設文本 --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                // 顯示消息框
                MessageBox.Show("請選擇目標貨幣", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                // 將焦點設置到 "到" 的貨幣選擇框
                cmbToCurrency.Focus();
                return;
            }

            // 檢查 "從" 和 "到" 的貨幣選擇框選擇的值是否相同
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // 將 amount 文本框的值設置到 ConvertedValue。
                // double.Parse 用於將字串轉換為 double 類型。
                // 文本框的文本是字串，而 ConvertedValue 是 double 類型
                ConvertedValue = double.Parse(txtCurrency.Text);

                // 顯示轉換後的貨幣和貨幣名稱，ToString("N3") 用於在小數點後顯示三位數
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                // 貨幣轉換計算：將 "從" 貨幣的值乘以(*) amount 文本框的值，然後將總和除以(/) "到" 貨幣的值
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                // 顯示轉換後的貨幣和貨幣名稱
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }


        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();

        }

        // 方法: 清除所有控制項的內容
        private void ClearControls()
        {
            try
            {
                // 清空 txtCurrency 的文本內容
                txtCurrency.Text = string.Empty;

                // 重置 cmbFromCurrency 的選定項目為第一項（如果有項目）
                if (cmbFromCurrency.Items.Count > 0)
                    cmbFromCurrency.SelectedIndex = 0;

                // 重置 cmbToCurrency 的選定項目為第一項（如果有項目）
                if (cmbToCurrency.Items.Count > 0)
                    cmbToCurrency.SelectedIndex = 0;

                // 清空 lblCurrency 的內容
                lblCurrency.Content = "";

                // 將焦點設置到 txtCurrency
                txtCurrency.Focus();
            }
            catch (Exception ex)
            {
                // 如果發生異常，顯示錯誤信息
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetData()
        {
            //The method is used for connect with database and open database connection    
            mycon();

            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);

            da.Fill(dt); 

            if (dt != null && dt.Rows.Count > 0)
            {
                dgvCurrency.ItemsSource = dt.DefaultView; // 將 DataTable 指派給 DataGrid
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            con.Close();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // 非數字字符
            e.Handled = regex.IsMatch(e.Text);  // 如果輸入的文本包含非數字字符，阻止輸入
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCurrencyName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {   //Edit time and set that record Id in CurrencyId variable.
                    //Code to Update. If CurrencyId greater than zero than it is go for update.
                    if (CurrencyId > 0)
                    {
                        //Show the confirmation message
                        if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            DataTable dt = new DataTable();

                            //Update Query Record update using Id
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    // Code to Save
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            //Insert query to Save data in the table
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData(); 
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbFromCurrency.SelectedValue != null && int.Parse(cmbFromCurrency.SelectedValue.ToString()) != 0 && cmbFromCurrency.SelectedIndex != 0)
                {
                    int CurrencyFromId = int.Parse(cmbFromCurrency.SelectedValue.ToString());
                

                    mycon();
                    DataTable dt = new DataTable();


                    cmd = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyFromId", con);
                    cmd.CommandType = CommandType.Text;

                if (CurrencyFromId != null && CurrencyFromId != 0)
                {
                    cmd.Parameters.AddWithValue("@CurrencyFromId", CurrencyFromId);

                }


                da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0) 
                {
                    FromAmount = double.Parse(dt.Rows[0]["Amount"].ToString()); // 取得 "從" 貨幣的金額
                }

                con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;        // 將 sender 轉換為 DataGrid 類型
                DataRowView row_selected = grd.CurrentItem as DataRowView;      // 將當前選定的項目轉換為 DataRowView 類型

                if (row_selected != null) // 如果有選擇的項目
                {
                    if (dgvCurrency.Items.Count > 0) // 處理選擇 < DataGird Name = "dgvCurrency" > 的資料
                    {
                        if (grd.SelectedCells.Count > 0) // 如果有選擇的資料格
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString()); // 取得選擇的項目 Id

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0) // 如果選擇的資料格為第一欄
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();

                                //Get selected row CurrencyName column value and set it to CurrencyName textbox
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";     //Change save button text Save to Update
                            }

                            if (grd.SelectedCells[0].Column.DisplayIndex == 1) // 如果選擇的資料格為第二欄
                            {
                                mycon();
                                DataTable dt = new DataTable();

                                //Execute delete query to delete record from table using Id
                                cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con); // 刪除 Currency_Master 選取Id的項目
                                cmd.CommandType = CommandType.Text;

                                cmd.Parameters.AddWithValue("@Id", CurrencyId); // 設定 @Id 參數值
                                cmd.ExecuteNonQuery(); // 執行刪除命令
                                con.Close(); 

                                MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                ClearMaster();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //If cmbToCurrency selectedvalue is not equal to null and not equal to zero
                if (cmbToCurrency.SelectedValue != null && int.Parse(cmbToCurrency.SelectedValue.ToString()) != 0 && cmbToCurrency.SelectedIndex != 0)
                {
                    //cmbToCurrency selectedvalue is set to CurrencyToId variable
                    int CurrencyToId = int.Parse(cmbToCurrency.SelectedValue.ToString());

                    mycon();

                    DataTable dt = new DataTable();
                    //Select query for get Amount from database using id
                    cmd = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyToId", con);
                    cmd.CommandType = CommandType.Text;

                    if (CurrencyToId != null && CurrencyToId != 0)
                        //CurrencyToId set in @CurrencyToId parameter and send parameter in our query
                        cmd.Parameters.AddWithValue("@CurrencyToId", CurrencyToId);

                    da = new SqlDataAdapter(cmd);

                    //Set the data that the query returns in the data table
                    da.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                        //Get amount column value from datatable and set amount value in ToAmount variable which is declared globally
                        ToAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 當用戶在 cmbToCurrency 控件中按下按鍵時觸發此事件處理程序
        private void cmbToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)

        {
            // 如果用戶按下的是 Tab 鍵或 Enter 鍵，則執行 cmbToCurrency_SelectionChanged 事件
            if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
            {
                // 調用 cmbToCurrency_SelectionChanged 事件處理程序
                cmbToCurrency_SelectionChanged(sender, null);
            }
        }

        private void txtCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void dgvCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void dgvCurrency_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
