namespace PageSum
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnAdd = new Button();
            btnCalculate = new Button();
            btnClear = new Button();
            txtInput = new TextBox();
            lstRanges = new ListBox();
            lblMessage = new Label();
            SuspendLayout();
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(479, 61);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "加入段落";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnCalculate
            // 
            btnCalculate.Location = new Point(479, 90);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(75, 23);
            btnCalculate.TabIndex = 1;
            btnCalculate.Text = "計算總頁數";
            btnCalculate.UseVisualStyleBackColor = true;
            btnCalculate.Click += btnCalculate_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(479, 119);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 23);
            btnClear.TabIndex = 2;
            btnClear.Text = "清除所有";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // txtInput
            // 
            txtInput.Location = new Point(12, 76);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(452, 66);
            txtInput.TabIndex = 3;
            // 
            // lstRanges
            // 
            lstRanges.DrawMode = DrawMode.OwnerDrawVariable;
            lstRanges.FormattingEnabled = true;
            lstRanges.ItemHeight = 15;
            lstRanges.Location = new Point(12, 149);
            lstRanges.Name = "lstRanges";
            lstRanges.Size = new Size(1160, 289);
            lstRanges.TabIndex = 4;
            lstRanges.DrawItem += lstRanges_DrawItem;
            lstRanges.MeasureItem += lstRanges_MeasureItem;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(23, 19);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(67, 15);
            lblMessage.TabIndex = 5;
            lblMessage.Text = "累積總頁數";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1206, 558);
            Controls.Add(lblMessage);
            Controls.Add(lstRanges);
            Controls.Add(txtInput);
            Controls.Add(btnClear);
            Controls.Add(btnCalculate);
            Controls.Add(btnAdd);
            Name = "Form1";
            Text = "忠孝延吉輸出PDF助手";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnAdd;
        private Button btnCalculate;
        private Button btnClear;
        private TextBox txtInput;
        private ListBox lstRanges;
        private Label lblMessage;
    }
}
