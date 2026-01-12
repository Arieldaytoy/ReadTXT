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
            button3 = new Button();
            textBox2 = new TextBox();
            label3 = new Label();
            comboBox1 = new ComboBox();
            label2 = new Label();
            listBox1 = new ListBox();
            toolStrip1 = new ToolStrip();
            toolStripSplitButton1 = new ToolStripSplitButton();
            背景色ToolStripMenuItem = new ToolStripMenuItem();
            字体ToolStripMenuItem = new ToolStripMenuItem();
            语音包ToolStripMenuItem = new ToolStripMenuItem();
            toolStripLabel1 = new ToolStripLabel();
            toolStripComboBox1 = new ToolStripComboBox();
            toolStripLabel2 = new ToolStripLabel();
            toolStripComboBox2 = new ToolStripComboBox();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripLabel3 = new ToolStripLabel();
            toolStripButton1 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            toolStripButton3 = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButton4 = new ToolStripButton();
            toolStripComboBox3 = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            fontDialog1 = new FontDialog();
            colorDialog1 = new ColorDialog();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            toolStripStatusLabel6 = new ToolStripStatusLabel();
            groupBox1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(423, 32);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 0;
            label1.Text = "小说地址：";
            // 
            // textBox1
            // 
            textBox1.AllowDrop = true;
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox1.Location = new Point(423, 55);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "输入txt路径或直接拖拽小说到此";
            textBox1.Size = new Size(369, 23);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button1.AutoSize = true;
            button1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button1.Location = new Point(750, 27);
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
            richTextBox1.Location = new Point(199, 92);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(593, 344);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button2.AutoSize = true;
            button2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button2.Location = new Point(285, 21);
            button2.Name = "button2";
            button2.Size = new Size(66, 27);
            button2.TabIndex = 11;
            button2.Text = "载入设置";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(12, 28);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(405, 58);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "设置规则";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button3.AutoSize = true;
            button3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button3.Location = new Point(357, 21);
            button3.Name = "button3";
            button3.Size = new Size(42, 27);
            button3.TabIndex = 18;
            button3.Text = "保存";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(68, 23);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "第([一二两三四五六七八九十零百千万\\d]+)章";
            textBox2.Size = new Size(96, 23);
            textBox2.TabIndex = 17;
            textBox2.Text = "第([一二两三四五六七八九十零百千万\\d]+)章";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 27);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 16;
            label3.Text = "章节规则";
            label3.Click += label3_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "ANSI", "UTF-8", "UTF-8 BOM" });
            comboBox1.Location = new Point(208, 21);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(71, 25);
            comboBox1.TabIndex = 15;
            comboBox1.Text = "UTF-8";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(170, 26);
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
            listBox1.Location = new Point(12, 92);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new Size(183, 344);
            listBox1.TabIndex = 24;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripSplitButton1, toolStripLabel1, toolStripComboBox1, toolStripLabel2, toolStripComboBox2, toolStripSeparator3, toolStripLabel3, toolStripButton1, toolStripButton2, toolStripButton3, toolStripSeparator1, toolStripButton4, toolStripComboBox3, toolStripSeparator2 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(804, 25);
            toolStrip1.TabIndex = 26;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { 背景色ToolStripMenuItem, 字体ToolStripMenuItem, 语音包ToolStripMenuItem });
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(32, 22);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            toolStripSplitButton1.ToolTipText = "设置";
            // 
            // 背景色ToolStripMenuItem
            // 
            背景色ToolStripMenuItem.Name = "背景色ToolStripMenuItem";
            背景色ToolStripMenuItem.Size = new Size(112, 22);
            背景色ToolStripMenuItem.Text = "背景色";
            背景色ToolStripMenuItem.Click += 背景色ToolStripMenuItem_Click;
            // 
            // 字体ToolStripMenuItem
            // 
            字体ToolStripMenuItem.Name = "字体ToolStripMenuItem";
            字体ToolStripMenuItem.Size = new Size(112, 22);
            字体ToolStripMenuItem.Text = "字体";
            字体ToolStripMenuItem.Click += 字体ToolStripMenuItem_Click;
            // 
            // 语音包ToolStripMenuItem
            // 
            语音包ToolStripMenuItem.Name = "语音包ToolStripMenuItem";
            语音包ToolStripMenuItem.Size = new Size(112, 22);
            语音包ToolStripMenuItem.Text = "语音包";
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
            toolStripComboBox1.ToolTipText = "-10~10的整数";
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
            toolStripComboBox2.Items.AddRange(new object[] { "整行" });
            toolStripComboBox2.Name = "toolStripComboBox2";
            toolStripComboBox2.Size = new Size(70, 25);
            toolStripComboBox2.Text = "整行";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
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
            // toolStripButton4
            // 
            toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton4.Image = (Image)resources.GetObject("toolStripButton4.Image");
            toolStripButton4.ImageTransparentColor = Color.Magenta;
            toolStripButton4.Name = "toolStripButton4";
            toolStripButton4.Size = new Size(23, 22);
            toolStripButton4.Text = "保存";
            toolStripButton4.Click += toolStripButton4_Click;
            // 
            // toolStripComboBox3
            // 
            toolStripComboBox3.Items.AddRange(new object[] { "覆盖保存", "另存带日期戳", "另存带时间戳" });
            toolStripComboBox3.Name = "toolStripComboBox3";
            toolStripComboBox3.Size = new Size(121, 25);
            toolStripComboBox3.Text = "覆盖保存";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3, toolStripStatusLabel4, toolStripStatusLabel5, toolStripStatusLabel6 });
            statusStrip1.Location = new Point(0, 449);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(804, 22);
            statusStrip1.TabIndex = 27;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(44, 17);
            toolStripStatusLabel1.Text = "状态：";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(12, 17);
            toolStripStatusLabel2.Text = " ";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(56, 17);
            toolStripStatusLabel3.Text = "当前章：";
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(0, 17);
            toolStripStatusLabel4.ToolTipText = "当前章";
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(56, 17);
            toolStripStatusLabel5.Text = "当前行：";
            // 
            // toolStripStatusLabel6
            // 
            toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            toolStripStatusLabel6.Size = new Size(15, 17);
            toolStripStatusLabel6.Text = "0";
            toolStripStatusLabel6.ToolTipText = "当前行";
            // 
            // ReadTXT
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 471);
            Controls.Add(statusStrip1);
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
            Load += ReadTXT_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
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
        private FontDialog fontDialog1;
        private ColorDialog colorDialog1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripMenuItem 背景色ToolStripMenuItem;
        private ToolStripMenuItem 字体ToolStripMenuItem;
        private Button button3;
        private ToolStripMenuItem 语音包ToolStripMenuItem;
        private ToolStripButton toolStripButton4;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private ToolStripStatusLabel toolStripStatusLabel6;
        private ToolStripComboBox toolStripComboBox3;
    }
}
