using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
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
using WPF_Data_binding.Data;

namespace WPFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 將 People 屬性改為公共屬性，使用 public 存取修飾詞
        public List<Person> People { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // 初始化 People 屬性
            People = new List<Person>
            {
                new Person { Name = "Jannick", Age = 30 },
                new Person { Name = "Marc", Age = 20 },
                new Person { Name = "Maria", Age = 40 },
                new Person { Name = "Scott", Age = 35 },
                new Person { Name = "Lucas", Age = 27 }
            };

            // 設定 ListBox 的資料來源為 People 屬性
            ListBoxPeople.ItemsSource = People;

            // 其他初始化邏輯...
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = ListBoxPeople.SelectedItems; // 取得目前選取的項目

            foreach (var item in selectedItems)
            {

                var person = (Person)item;
                MessageBox.Show(person.Name);
            }

        }
    }
}
