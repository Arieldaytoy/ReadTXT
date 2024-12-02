namespace ReadTXT
{
    partial class ReadTXT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReadTXT));
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            richTextBox1 = new RichTextBox();
            button2 = new Button();
            groupBox1 = new GroupBox();
            textBox2 = new TextBox();
            label3 = new Label();
            comboBox1 = new ComboBox();
            label2 = new Label();
            listBox1 = new ListBox();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            toolStripComboBox1 = new ToolStripComboBox();
            toolStripLabel2 = new ToolStripLabel();
            toolStripComboBox2 = new ToolStripComboBox();
            toolStripLabel3 = new ToolStripLabel();
            toolStripButton1 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            toolStripButton3 = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel4 = new ToolStripLabel();
            toolStripLabel5 = new ToolStripLabel();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripLabel6 = new ToolStripLabel();
            toolStripLabel7 = new ToolStripLabel();
            toolStripSeparator3 = new ToolStripSeparator();
            fontDialog1 = new FontDialog();
            colorDialog1 = new ColorDialog();
            toolStripSplitButton1 = new ToolStripSplitButton();
            字体ToolStripMenuItem = new ToolStripMenuItem();
            背景色ToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(367, 12);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 0;
            label1.Text = "小说地址：";
            // 
            // textBox1
            // 
            textBox1.AllowDrop = true;
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox1.Location = new Point(367, 35);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "输入txt路径或直接拖拽小说到此";
            textBox1.Size = new Size(425, 23);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button1.AutoSize = true;
            button1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button1.Location = new Point(750, 7);
            button1.Name = "button1";
            button1.Size = new Size(42, 27);
            button1.TabIndex = 2;
            button1.Text = "解析";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 134);
            richTextBox1.Location = new Point(199, 65);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(593, 378);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button2.AutoSize = true;
            button2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button2.Location = new Point(267, 21);
            button2.Name = "button2";
            button2.Size = new Size(66, 27);
            button2.TabIndex = 11;
            button2.Text = "载入设置";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(12, 8);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(339, 58);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "设置规则";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(68, 23);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "《(.*?)》";
            textBox2.Size = new Size(78, 23);
            textBox2.TabIndex = 17;
            textBox2.Text = "第\\d{4}章";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 27);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 16;
            label3.Text = "章节规则";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "ANSI", "UTF-8", "UTF-8 BOM" });
            comboBox1.Location = new Point(190, 23);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(71, 25);
            comboBox1.TabIndex = 15;
            comboBox1.Text = "UTF-8";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(152, 27);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 14;
            label2.Text = "编码";
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(12, 65);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new Size(183, 378);
            listBox1.TabIndex = 24;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, toolStripComboBox1, toolStripLabel2, toolStripComboBox2, toolStripLabel3, toolStripButton1, toolStripButton2, toolStripButton3, toolStripSeparator1, toolStripLabel4, toolStripLabel5, toolStripSeparator2, toolStripLabel6, toolStripLabel7, toolStripSeparator3, toolStripSplitButton1 });
            toolStrip1.Location = new Point(0, 446);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(804, 25);
            toolStrip1.TabIndex = 26;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(32, 22);
            toolStripLabel1.Text = "语速";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.AutoSize = false;
            toolStripComboBox1.Items.AddRange(new object[] { "-10", "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2", "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(40, 25);
            toolStripComboBox1.Text = "5";
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(56, 22);
            toolStripLabel2.Text = "朗读模式";
            // 
            // toolStripComboBox2
            // 
            toolStripComboBox2.AutoSize = false;
            toolStripComboBox2.Items.AddRange(new object[] { "自动整行", "自动整章", "手动整行", "手动整章" });
            toolStripComboBox2.Name = "toolStripComboBox2";
            toolStripComboBox2.Size = new Size(70, 25);
            toolStripComboBox2.Text = "自动整行";
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Size = new Size(56, 22);
            toolStripLabel3.Text = "朗读操作";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "暂停";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(23, 22);
            toolStripButton2.Text = "开始";
            toolStripButton2.Click += toolStripButton2_Click;
            // 
            // toolStripButton3
            // 
            toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
            toolStripButton3.ImageTransparentColor = Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new Size(23, 22);
            toolStripButton3.Text = "下一章";
            toolStripButton3.Click += toolStripButton3_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripLabel4
            // 
            toolStripLabel4.Name = "toolStripLabel4";
            toolStripLabel4.Size = new Size(44, 22);
            toolStripLabel4.Text = "状态：";
            // 
            // toolStripLabel5
            // 
            toolStripLabel5.Name = "toolStripLabel5";
            toolStripLabel5.Size = new Size(12, 22);
            toolStripLabel5.Text = " ";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStripLabel6
            // 
            toolStripLabel6.Name = "toolStripLabel6";
            toolStripLabel6.Size = new Size(12, 22);
            toolStripLabel6.Text = " ";
            toolStripLabel6.ToolTipText = "上次阅读章";
            // 
            // toolStripLabel7
            // 
            toolStripLabel7.Name = "toolStripLabel7";
            toolStripLabel7.Size = new Size(19, 22);
            toolStripLabel7.Text = " 0";
            toolStripLabel7.ToolTipText = "按行阅读行号标记";
            toolStripLabel7.Visible = false;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { 背景色ToolStripMenuItem, 字体ToolStripMenuItem });
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(32, 22);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // 字体ToolStripMenuItem
            // 
            字体ToolStripMenuItem.Name = "字体ToolStripMenuItem";
            字体ToolStripMenuItem.Size = new Size(180, 22);
            字体ToolStripMenuItem.Text = "字体";
            // 
            // 背景色ToolStripMenuItem
            // 
            背景色ToolStripMenuItem.Name = "背景色ToolStripMenuItem";
            背景色ToolStripMenuItem.Size = new Size(180, 22);
            背景色ToolStripMenuItem.Text = "背景色";
            // 
            // ReadTXT
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 471);
            Controls.Add(toolStrip1);
            Controls.Add(listBox1);
            Controls.Add(groupBox1);
            Controls.Add(richTextBox1);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            MinimumSize = new Size(820, 510);
            Name = "ReadTXT";
            Text = "ReadTXT";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private RichTextBox richTextBox1;
        private Button button2;
        private GroupBox groupBox1;
        private TextBox textBox2;
        private Label label3;
        private ComboBox comboBox1;
        private Label label2;
        private ListBox listBox1;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripLabel toolStripLabel2;
        private ToolStripComboBox toolStripComboBox2;
        private ToolStripLabel toolStripLabel3;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton3;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripLabel toolStripLabel4;
        private ToolStripLabel toolStripLabel5;
        private ToolStripLabel toolStripLabel6;
        private ToolStripLabel toolStripLabel7;
        private FontDialog fontDialog1;
        private ColorDialog colorDialog1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripMenuItem 背景色ToolStripMenuItem;
        private ToolStripMenuItem 字体ToolStripMenuItem;
    }
}
