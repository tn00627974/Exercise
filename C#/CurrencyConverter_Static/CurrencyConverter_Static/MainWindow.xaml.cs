using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CurrencyConverter_Static
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary> 
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            lblCurrency.Content = "請選擇您要轉換的貨幣";
            //txtAmount.Content = "金額";
            BindCurrency(); // 呼叫 BindCurrency 方法
        }

        // Private 是一個存取修飾符。類型或成員只能由同一類別或結構中的程式碼存取。
        // Void 關鍵字用於方法簽章來聲明不傳回任何值的方法。使用 void 傳回類型宣告的方法不能為其包含的任何傳回語句提供任何參數。
        public void BindCurrency()
        {
            DataTable dtCurrency = new DataTable(); // using System.Data;
                                                   
            dtCurrency.Columns.Add("Text"); //Add display column in DataTable
            dtCurrency.Columns.Add("Value"); // Add value column in DataTable

            //Add rows in Datatable with text and value
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("TWD", 30);
            dtCurrency.Rows.Add("INR", 1);
            dtCurrency.Rows.Add("USD", 90);
            dtCurrency.Rows.Add("EUR", 85);
            dtCurrency.Rows.Add("SAR", 20);
            dtCurrency.Rows.Add("POUND", 5);
            dtCurrency.Rows.Add("DEM", 43);

            // cmbFromCurrency
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;   // 將 DataTable 指派給 Combobox  
            cmbFromCurrency.DisplayMemberPath = "Text";             // DisplayMemberPath 是用來顯示Combobox的文字，SelectedValuePath是用來顯示Combobox的Value值
            cmbFromCurrency.SelectedValuePath = "Value";            // SelectedValuePath 是用來顯示Combobox的Value值，SelectedValuePath 屬性的值是 DataTable 的 Value 值
            cmbFromCurrency.SelectedIndex = 0;                      // SelectedIndex 是用來設定Combobox的預設選取項目，預設值為0，表示Select。

            // cmbToCurrency
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
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


        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtCurrencyName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgvCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dgvCurrency_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void cmbToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtCurrency_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
