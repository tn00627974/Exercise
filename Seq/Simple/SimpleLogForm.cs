using System;
using System.Windows.Forms;
using SimpleLogger;

namespace SimpleLoggerApp
{
    public partial class SimpleLogForm : Form
    {
        private ProjectLogger _userLogger;
        private ProjectLogger _orderLogger;
        private ProjectLogger _paymentLogger;

        public SimpleLogForm()
        {
            _userLogger = new ProjectLogger("UserProject");
            _orderLogger = new ProjectLogger("OrderProject");
            _paymentLogger = new ProjectLogger("PaymentProject");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "簡單日誌示例";
            this.Size = new System.Drawing.Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label titleLabel = new Label
            {
                Text = "多專案日誌系統",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(300, 30),
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(titleLabel);

            Button userBtn = new Button
            {
                Text = "用戶專案",
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(140, 40)
            };
            userBtn.Click += (s, e) => TestUserProject();
            this.Controls.Add(userBtn);

            Button orderBtn = new Button
            {
                Text = "訂單專案",
                Location = new System.Drawing.Point(180, 70),
                Size = new System.Drawing.Size(140, 40)
            };
            orderBtn.Click += (s, e) => TestOrderProject();
            this.Controls.Add(orderBtn);

            Button paymentBtn = new Button
            {
                Text = "支付專案",
                Location = new System.Drawing.Point(340, 70),
                Size = new System.Drawing.Size(140, 40)
            };
            paymentBtn.Click += (s, e) => TestPaymentProject();
            this.Controls.Add(paymentBtn);

            TextBox outputBox = new TextBox
            {
                Name = "outputBox",
                Location = new System.Drawing.Point(20, 130),
                Size = new System.Drawing.Size(460, 170),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(outputBox);

            Button clearBtn = new Button
            {
                Text = "清空",
                Location = new System.Drawing.Point(20, 310),
                Size = new System.Drawing.Size(100, 30)
            };
            clearBtn.Click += (s, e) => ((TextBox)this.Controls["outputBox"]).Clear();
            this.Controls.Add(clearBtn);
        }

        private void TestUserProject()
        {
            Log("=== 用戶專案 ===");
            _userLogger.Info("用戶登入", "AuthService", "Login");
            _userLogger.Info("建立用戶: john", "UserService", "Register");
            Log("✅ 已發送\n");
        }

        private void TestOrderProject()
        {
            Log("=== 訂單專案 ===");
            _orderLogger.Info("訂單已建立: #12345", "OrderService", "Create");
            _orderLogger.Warning("金額異常", "OrderService", "Validate");
            Log("✅ 已發送\n");
        }

        private void TestPaymentProject()
        {
            Log("=== 支付專案 ===");
            _paymentLogger.Info("支付處理中", "PaymentService", "Process");
            try
            {
                throw new Exception("支付失敗");
            }
            catch (Exception ex)
            {
                _paymentLogger.Error(ex, "支付失敗", "PaymentService", "Error");
                Log("❌ 錯誤已記錄\n");
            }
        }

        private void Log(string msg)
        {
            TextBox box = (TextBox)this.Controls["outputBox"];
            box.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}");
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimpleLogForm());
        }
    }
}
