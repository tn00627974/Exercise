using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

namespace InvoiceManagement
{
    /// <summary>
    /// LoginView.xaml 的互動邏輯
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void OnLoginbtnclicked(object sender, RoutedEventArgs e)
        {
            string passwordEntered = PasswordBox.Password;
            string? envPw = Environment.GetEnvironmentVariable("InvoiceManagement");

            if (string.IsNullOrEmpty(envPw))
            {
                if (passwordEntered == envPw)
                {
                    MessageBox.Show("登入成功");
                }
                else
                {
                    MessageBox.Show("登入失敗");
                }
            }
            else
            {
                MessageBox.Show("密碼不能為空白");
            }




        }
    }
}