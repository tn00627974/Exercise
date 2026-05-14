using System;
using System.Windows.Forms;
using Serilog;
using Serilog.Context;

namespace SeqWinFormApp
{
    public partial class MainForm : Form
    {
        private int messageCount = 0;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Seq Logger 練習 - C# WinForm";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            // 標題標籤
            Label titleLabel = new Label();
            titleLabel.Text = "Seq Server 日誌練習應用";
            titleLabel.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            titleLabel.Location = new System.Drawing.Point(20, 20);
            titleLabel.Size = new System.Drawing.Size(300, 30);
            this.Controls.Add(titleLabel);

            // 訊息輸入框
            Label inputLabel = new Label();
            inputLabel.Text = "輸入日誌訊息:";
            inputLabel.Location = new System.Drawing.Point(20, 70);
            inputLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(inputLabel);

            TextBox messageTextBox = new TextBox();
            messageTextBox.Name = "messageTextBox";
            messageTextBox.Location = new System.Drawing.Point(20, 95);
            messageTextBox.Size = new System.Drawing.Size(400, 30);
            messageTextBox.Multiline = true;
            this.Controls.Add(messageTextBox);

            // 日誌等級選擇
            Label levelLabel = new Label();
            levelLabel.Text = "日誌等級:";
            levelLabel.Location = new System.Drawing.Point(20, 140);
            levelLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(levelLabel);

            ComboBox levelComboBox = new ComboBox();
            levelComboBox.Name = "levelComboBox";
            levelComboBox.Location = new System.Drawing.Point(20, 165);
            levelComboBox.Size = new System.Drawing.Size(150, 30);
            levelComboBox.Items.AddRange(new string[] { "Information", "Warning", "Error", "Debug" });
            levelComboBox.SelectedIndex = 0;
            this.Controls.Add(levelComboBox);

            // 發送日誌按鈕
            Button sendButton = new Button();
            sendButton.Text = "發送日誌";
            sendButton.Location = new System.Drawing.Point(20, 210);
            sendButton.Size = new System.Drawing.Size(100, 40);
            sendButton.Click += (s, e) => SendLog(messageTextBox, levelComboBox);
            this.Controls.Add(sendButton);

            // 自動測試按鈕
            Button autoTestButton = new Button();
            autoTestButton.Text = "自動測試";
            autoTestButton.Location = new System.Drawing.Point(140, 210);
            autoTestButton.Size = new System.Drawing.Size(100, 40);
            autoTestButton.Click += (s, e) => AutoTest();
            this.Controls.Add(autoTestButton);

            // 輸出訊息框
            Label outputLabel = new Label();
            outputLabel.Text = "已發送日誌:";
            outputLabel.Location = new System.Drawing.Point(20, 270);
            outputLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(outputLabel);

            TextBox outputTextBox = new TextBox();
            outputTextBox.Name = "outputTextBox";
            outputTextBox.Location = new System.Drawing.Point(20, 295);
            outputTextBox.Size = new System.Drawing.Size(550, 150);
            outputTextBox.Multiline = true;
            outputTextBox.ReadOnly = true;
            outputTextBox.ScrollBars = ScrollBars.Vertical;
            this.Controls.Add(outputTextBox);

            // 狀態標籤
            Label statusLabel = new Label();
            statusLabel.Name = "statusLabel";
            statusLabel.Text = "已就緒 (Seq Server: http://localhost:5341)";
            statusLabel.Location = new System.Drawing.Point(20, 460);
            statusLabel.Size = new System.Drawing.Size(550, 20);
            statusLabel.ForeColor = System.Drawing.Color.Green;
            this.Controls.Add(statusLabel);
        }

        private void SendLog(TextBox messageTextBox, ComboBox levelComboBox)
        {
            string message = messageTextBox.Text;
            string level = levelComboBox.SelectedItem?.ToString() ?? "Information";

            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("請輸入日誌訊息", "提示");
                return;
            }

            try
            {
                messageCount++;
                
                // 使用 LogContext 添加自訂屬性
                using (LogContext.PushProperty("MessageCount", messageCount))
                using (LogContext.PushProperty("FormName", this.Text))
                {
                    switch (level)
                    {
                        case "Information":
                            Log.Information("用戶訊息: {Message}", message);
                            break;
                        case "Warning":
                            Log.Warning("警告訊息: {Message}", message);
                            break;
                        case "Error":
                            Log.Error("錯誤訊息: {Message}", message);
                            break;
                        case "Debug":
                            Log.Debug("調試訊息: {Message}", message);
                            break;
                    }
                }

                // 更新輸出框
                TextBox outputTextBox = (TextBox)this.Controls["outputTextBox"];
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                outputTextBox.AppendText($"[{timestamp}] [{level}] {message}\r\n");

                messageTextBox.Clear();
                messageTextBox.Focus();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "發送日誌時發生錯誤");
                MessageBox.Show($"發送日誌失敗: {ex.Message}", "錯誤");
            }
        }

        private void AutoTest()
        {
            Log.Information("開始自動測試");
            
            string[] testMessages = new string[]
            {
                "這是一條資訊日誌",
                "這是一條警告日誌",
                "這是一條錯誤日誌",
                "系統初始化完成",
                "用戶登入成功"
            };

            foreach (var msg in testMessages)
            {
                var textBox = (TextBox)this.Controls["messageTextBox"];
                textBox.Text = msg;

                var levelBox = (ComboBox)this.Controls["levelComboBox"];
                if (msg.Contains("警告"))
                    levelBox.SelectedIndex = 1;
                else if (msg.Contains("錯誤"))
                    levelBox.SelectedIndex = 2;
                else
                    levelBox.SelectedIndex = 0;

                SendLog(textBox, levelBox);
                System.Threading.Thread.Sleep(500);
            }

            Log.Information("自動測試完成");
            MessageBox.Show("自動測試完成！請檢查 Seq Server", "完成");
        }
    }
}
