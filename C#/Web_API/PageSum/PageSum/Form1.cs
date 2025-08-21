using System.Text.RegularExpressions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PageSum
{
    public partial class Form1 : Form
    {
        private const int ITEM_PADDING = 10 ;//各項之間的邊距

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text.Trim();

            try
            {
                int count = PageCalculator.CountPages(input);
                lstRanges.Items.Add($"（{count}頁）{input}");
                txtInput.Clear();
                lblMessage.Text = "";
            }
            catch (FormatException ex)
            {
                lblMessage.Text = ex.Message;
                return;
            }
            catch (ArgumentException ex)
            {
                lblMessage.Text = $"錯誤{ex.Message}";
                return;
            }


        }


        private void btnCalculate_Click(object sender, EventArgs e)
        {
            int total = 0;

            foreach (string item in lstRanges.Items)
            {
                string range = Regex.Replace(item, @"^（\d+頁）", "");
                total += PageCalculator.CountPages(range);
            }

            lblMessage.Text = $"總頁數：{total}頁";

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            lstRanges.Items.Clear();
            lblMessage.Text = "";
        }

        private void lstRanges_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            string text = lstRanges.Items[e.Index].ToString();
            Size size = TextRenderer.MeasureText(text, lstRanges.Font, lstRanges.Size, TextFormatFlags.WordBreak);
            e.ItemHeight = size.Height + ITEM_PADDING * 2;
        }

        private void lstRanges_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            string text = lstRanges.Items[e.Index].ToString();
            Rectangle textBounds = new Rectangle(e.Bounds.X, e.Bounds.Y + ITEM_PADDING, e.Bounds.Width, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, text, e.Font, textBounds, e.ForeColor, TextFormatFlags.WordBreak);
        }
    }
}
