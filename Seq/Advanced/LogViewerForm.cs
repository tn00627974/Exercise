using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SeqWinFormApp
{
    /// <summary>
    /// 多專案日誌預覽器
    /// </summary>
    public partial class LogViewerForm : Form
    {
        private readonly string _seqUrl = "http://localhost:5341";
        private readonly HttpClient _httpClient = new HttpClient();
        private List<LogEntry> _allLogs = new List<LogEntry>();

        // 日誌項目類
        public class LogEntry
        {
            public string Project { get; set; }
            public string Service { get; set; }
            public string Timestamp { get; set; }
            public string Level { get; set; }
            public string Module { get; set; }
            public string Message { get; set; }
            public string CorrelationId { get; set; }
            public string Exception { get; set; }
        }

        public LogViewerForm()
        {
            InitializeComponent();
            this.Text = "多專案日誌預覽器";
            this.Size = new System.Drawing.Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            // 主容器
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 3;
            mainLayout.ColumnCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ===== 第一行: 篩選條件 =====
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Fill;
            filterPanel.Height = 80;
            filterPanel.BackColor = System.Drawing.Color.LightGray;

            // 專案選擇
            Label projectLabel = new Label();
            projectLabel.Text = "專案:";
            projectLabel.Location = new System.Drawing.Point(10, 10);
            projectLabel.Size = new System.Drawing.Size(50, 20);
            filterPanel.Controls.Add(projectLabel);

            ComboBox projectComboBox = new ComboBox();
            projectComboBox.Name = "projectComboBox";
            projectComboBox.Location = new System.Drawing.Point(70, 10);
            projectComboBox.Size = new System.Drawing.Size(150, 25);
            projectComboBox.Items.AddRange(new string[] { "全部", "ECommerceApp", "BlogApp", "AnalyticsApp" });
            projectComboBox.SelectedIndex = 0;
            projectComboBox.SelectedIndexChanged += (s, e) => RefreshLogs();
            filterPanel.Controls.Add(projectComboBox);

            // 服務選擇
            Label serviceLabel = new Label();
            serviceLabel.Text = "服務:";
            serviceLabel.Location = new System.Drawing.Point(240, 10);
            serviceLabel.Size = new System.Drawing.Size(50, 20);
            filterPanel.Controls.Add(serviceLabel);

            ComboBox serviceComboBox = new ComboBox();
            serviceComboBox.Name = "serviceComboBox";
            serviceComboBox.Location = new System.Drawing.Point(300, 10);
            serviceComboBox.Size = new System.Drawing.Size(150, 25);
            serviceComboBox.Items.AddRange(new string[] { "全部", "UserService", "OrderService", "PaymentService" });
            serviceComboBox.SelectedIndex = 0;
            serviceComboBox.SelectedIndexChanged += (s, e) => RefreshLogs();
            filterPanel.Controls.Add(serviceComboBox);

            // 日誌等級選擇
            Label levelLabel = new Label();
            levelLabel.Text = "等級:";
            levelLabel.Location = new System.Drawing.Point(470, 10);
            levelLabel.Size = new System.Drawing.Size(50, 20);
            filterPanel.Controls.Add(levelLabel);

            ComboBox levelComboBox = new ComboBox();
            levelComboBox.Name = "levelComboBox";
            levelComboBox.Location = new System.Drawing.Point(530, 10);
            levelComboBox.Size = new System.Drawing.Size(120, 25);
            levelComboBox.Items.AddRange(new string[] { "全部", "Information", "Warning", "Error" });
            levelComboBox.SelectedIndex = 0;
            levelComboBox.SelectedIndexChanged += (s, e) => RefreshLogs();
            filterPanel.Controls.Add(levelComboBox);

            // 重新載入按鈕
            Button refreshButton = new Button();
            refreshButton.Text = "重新載入";
            refreshButton.Location = new System.Drawing.Point(670, 10);
            refreshButton.Size = new System.Drawing.Size(100, 25);
            refreshButton.Click += async (s, e) => await LoadLogsFromSeq();
            filterPanel.Controls.Add(refreshButton);

            // 搜尋框
            Label searchLabel = new Label();
            searchLabel.Text = "搜尋:";
            searchLabel.Location = new System.Drawing.Point(10, 45);
            searchLabel.Size = new System.Drawing.Size(50, 20);
            filterPanel.Controls.Add(searchLabel);

            TextBox searchTextBox = new TextBox();
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Location = new System.Drawing.Point(70, 45);
            searchTextBox.Size = new System.Drawing.Size(380, 25);
            searchTextBox.TextChanged += (s, e) => RefreshLogs();
            filterPanel.Controls.Add(searchTextBox);

            // 狀態標籤
            Label statusLabel = new Label();
            statusLabel.Name = "statusLabel";
            statusLabel.Text = "準備就緒...";
            statusLabel.Location = new System.Drawing.Point(670, 45);
            statusLabel.Size = new System.Drawing.Size(300, 20);
            statusLabel.ForeColor = System.Drawing.Color.Green;
            filterPanel.Controls.Add(statusLabel);

            mainLayout.Controls.Add(filterPanel, 0, 0);

            // ===== 第二行: 統計信息 =====
            Panel statsPanel = new Panel();
            statsPanel.Dock = DockStyle.Fill;
            statsPanel.Height = 40;
            statsPanel.BackColor = System.Drawing.Color.White;

            Label statsLabel = new Label();
            statsLabel.Name = "statsLabel";
            statsLabel.Text = "總日誌: 0 | 資訊: 0 | 警告: 0 | 錯誤: 0";
            statsLabel.Location = new System.Drawing.Point(10, 5);
            statsLabel.Size = new System.Drawing.Size(400, 30);
            statsLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            statsPanel.Controls.Add(statsLabel);

            mainLayout.Controls.Add(statsPanel, 0, 1);

            // ===== 第三行: 日誌列表 =====
            DataGridView logGrid = new DataGridView();
            logGrid.Name = "logGrid";
            logGrid.Dock = DockStyle.Fill;
            logGrid.AllowUserToAddRows = false;
            logGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            logGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            logGrid.ReadOnly = true;

            // 添加列
            logGrid.Columns.Add("Project", "專案");
            logGrid.Columns.Add("Timestamp", "時間");
            logGrid.Columns.Add("Service", "服務");
            logGrid.Columns.Add("Level", "等級");
            logGrid.Columns.Add("Module", "模組");
            logGrid.Columns.Add("Message", "訊息");
            logGrid.Columns.Add("CorrelationId", "關聯ID");

            // 設置列顏色
            logGrid.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == logGrid.Columns["Level"].Index)
                {
                    string level = e.Value?.ToString();
                    if (level == "Error")
                        e.CellStyle.BackColor = System.Drawing.Color.LightCoral;
                    else if (level == "Warning")
                        e.CellStyle.BackColor = System.Drawing.Color.LightYellow;
                    else
                        e.CellStyle.BackColor = System.Drawing.Color.LightGreen;
                }
            };

            // 雙擊查看詳情
            logGrid.DoubleClick += (s, e) =>
            {
                if (logGrid.SelectedRows.Count > 0)
                {
                    var row = logGrid.SelectedRows[0];
                    var log = _allLogs[row.Index];
                    ShowLogDetails(log);
                }
            };

            mainLayout.Controls.Add(logGrid, 0, 2);

            this.Controls.Add(mainLayout);

            // 載入初始數據
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // 模擬數據（實際應從 Seq Server 讀取）
            _allLogs = new List<LogEntry>
            {
                new LogEntry { Project = "ECommerceApp", Service = "UserService", Timestamp = DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"), Level = "Information", Module = "Authentication", Message = "用戶登入成功", CorrelationId = "123-456" },
                new LogEntry { Project = "ECommerceApp", Service = "OrderService", Timestamp = DateTime.Now.AddMinutes(-4).ToString("yyyy-MM-dd HH:mm:ss"), Level = "Information", Module = "OrderCreation", Message = "訂單建立成功", CorrelationId = "123-457" },
                new LogEntry { Project = "ECommerceApp", Service = "PaymentService", Timestamp = DateTime.Now.AddMinutes(-3).ToString("yyyy-MM-dd HH:mm:ss"), Level = "Warning", Module = "Payment", Message = "支付延遲", CorrelationId = "123-458" },
                new LogEntry { Project = "BlogApp", Service = "UserService", Timestamp = DateTime.Now.AddMinutes(-2).ToString("yyyy-MM-dd HH:mm:ss"), Level = "Information", Module = "PostCreation", Message = "文章建立成功", CorrelationId = "456-789" },
                new LogEntry { Project = "BlogApp", Service = "UserService", Timestamp = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss"), Level = "Error", Module = "Database", Message = "數據庫連接失敗", CorrelationId = "456-790" },
                new LogEntry { Project = "AnalyticsApp", Service = "OrderService", Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Level = "Information", Module = "DataProcessing", Message = "數據分析完成", CorrelationId = "789-012" },
            };

            RefreshLogs();
        }

        private async Task LoadLogsFromSeq()
        {
            var statusLabel = (Label)this.Controls["statusLabel"];
            statusLabel.Text = "正在從 Seq Server 載入...";
            statusLabel.ForeColor = System.Drawing.Color.Blue;

            try
            {
                // 這裡應該調用 Seq API 來獲取真實日誌
                // 示例: GET http://localhost:5341/api/events?filter=@l='Error'
                
                statusLabel.Text = "載入完成";
                statusLabel.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"載入失敗: {ex.Message}";
                statusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void RefreshLogs()
        {
            var projectCombo = (ComboBox)this.Controls.Find("projectComboBox", true).FirstOrDefault();
            var serviceCombo = (ComboBox)this.Controls.Find("serviceComboBox", true).FirstOrDefault();
            var levelCombo = (ComboBox)this.Controls.Find("levelComboBox", true).FirstOrDefault();
            var searchBox = (TextBox)this.Controls.Find("searchTextBox", true).FirstOrDefault();
            var statsLabel = (Label)this.Controls.Find("statsLabel", true).FirstOrDefault();
            var logGrid = (DataGridView)this.Controls.Find("logGrid", true).FirstOrDefault();

            // 篩選日誌
            var filtered = _allLogs.AsEnumerable();

            if (projectCombo?.SelectedItem?.ToString() != "全部")
                filtered = filtered.Where(l => l.Project == projectCombo?.SelectedItem?.ToString());

            if (serviceCombo?.SelectedItem?.ToString() != "全部")
                filtered = filtered.Where(l => l.Service == serviceCombo?.SelectedItem?.ToString());

            if (levelCombo?.SelectedItem?.ToString() != "全部")
                filtered = filtered.Where(l => l.Level == levelCombo?.SelectedItem?.ToString());

            if (!string.IsNullOrEmpty(searchBox?.Text))
                filtered = filtered.Where(l => l.Message.Contains(searchBox.Text) || l.Module.Contains(searchBox.Text));

            // 更新統計
            var stats = filtered.GroupBy(l => l.Level).ToDictionary(g => g.Key, g => g.Count());
            statsLabel.Text = $"總日誌: {filtered.Count()} | 資訊: {stats.GetValueOrDefault("Information", 0)} | 警告: {stats.GetValueOrDefault("Warning", 0)} | 錯誤: {stats.GetValueOrDefault("Error", 0)}";

            // 更新表格
            logGrid.Rows.Clear();
            foreach (var log in filtered.OrderByDescending(l => l.Timestamp))
            {
                logGrid.Rows.Add(log.Project, log.Timestamp, log.Service, log.Level, log.Module, log.Message, log.CorrelationId);
            }
        }

        private void ShowLogDetails(LogEntry log)
        {
            var details = $@"
            專案: {log.Project}
            服務: {log.Service}
            時間: {log.Timestamp}
            等級: {log.Level}
            模組: {log.Module}
            關聯ID: {log.CorrelationId}

            訊息:
            {log.Message}

            {(string.IsNullOrEmpty(log.Exception) ? "" : $"例外:\n{log.Exception}")}";
                    MessageBox.Show(details, $"日誌詳情 - {log.Project}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
