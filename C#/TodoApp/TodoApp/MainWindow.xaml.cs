using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TodoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Create_Todo_TextBox(object sender, RoutedEventArgs e)
        {
            string todoText = TodoInput.Text;  

            if (!string.IsNullOrEmpty(todoText))
            // !string.IsNullOrEmpty(todoText) 檢查的是 todoText 既不為 null 也不為空字串。
            // 也可以寫 if (todoText != null)： 若 todoText 不是 null（即便是空字串 ""），該條件也會返回 true。
            {
                TextBlock todoItem = new TextBlock() // TextBlock 是 WPF 的控制項類型，用來顯示文字。
                {
                    Text = todoText,
                    Margin = new Thickness(10),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                TodoList.Children.Add(todoItem); // 將 TextBlock 加入到 TodoList 當中。

                TodoInput.Clear(); 
            }
            //MessageBox.Show("Button Clicked");
        }
    }
}