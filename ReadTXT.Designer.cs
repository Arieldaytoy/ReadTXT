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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReadTXT));
            label1 = new Label();
            TXTPath_textBox = new TextBox();
            Analyze_button = new Button();
            ChapterContent_richTextBox = new RichTextBox();
            LoadSet_button = new Button();
            groupBox1 = new GroupBox();
            Set_button = new Button();
            Rule_textBox = new TextBox();
            Rule_label = new Label();
            Code_comboBox = new ComboBox();
            Code_label = new Label();
            Chapter_listBox = new ListBox();
            toolStrip1 = new ToolStrip();
            Set_toolStripSplitButton = new ToolStripSplitButton();
            背景色ToolStripMenuItem = new ToolStripMenuItem();
            字体ToolStripMenuItem = new ToolStripMenuItem();
            语音包ToolStripMenuItem = new ToolStripMenuItem();
            快捷键ToolStripMenuItem = new ToolStripMenuItem();
            窗体切换ToolStripMenuItem = new ToolStripMenuItem();
            Winswitch_shortcut_toolStripTextBox = new ToolStripTextBox();
            最小化ToolStripMenuItem = new ToolStripMenuItem();
            Minimize_shortcut_toolStripTextBox = new ToolStripTextBox();
            迷你窗体置顶ToolStripMenuItem = new ToolStripMenuItem();
            MinTop_shortcut_toolStripTextBox = new ToolStripTextBox();
            开始朗读ToolStripMenuItem = new ToolStripMenuItem();
            Startreading_shortcut_toolStripTextBox = new ToolStripTextBox();
            暂停朗读ToolStripMenuItem = new ToolStripMenuItem();
            Pausereading_shortcut_toolStripTextBox = new ToolStripTextBox();
            下一章ToolStripMenuItem = new ToolStripMenuItem();
            Nextchapterreading_shortcut_toolStripTextBox = new ToolStripTextBox();
            toolStripSeparator6 = new ToolStripSeparator();
            重置默认ToolStripMenuItem = new ToolStripMenuItem();
            Speed_toolStripLabel = new ToolStripLabel();
            Speed_toolStripComboBox = new ToolStripComboBox();
            ReadMode_toolStripLabel = new ToolStripLabel();
            ReadMode_toolStripComboBox = new ToolStripComboBox();
            toolStripSeparator3 = new ToolStripSeparator();
            ReadAction_toolStripLabel = new ToolStripLabel();
            Pause_toolStripButton = new ToolStripButton();
            Start_toolStripButton = new ToolStripButton();
            NextChapter_toolStripButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            Save_toolStripButton = new ToolStripButton();
            Save_toolStripComboBox = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            fontDialog1 = new FontDialog();
            colorDialog1 = new ColorDialog();
            statusStrip1 = new StatusStrip();
            Info_toolStripStatusLabel = new ToolStripStatusLabel();
            LogStatus_toolStripStatusLabel = new ToolStripStatusLabel();
            CurrentChapterLabel_toolStripStatusLabel = new ToolStripStatusLabel();
            CurrentChapter_toolStripStatusLabel = new ToolStripStatusLabel();
            Currentlinelabel_toolStripStatusLabel = new ToolStripStatusLabel();
            CurrentLine_toolStripStatusLabel = new ToolStripStatusLabel();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            迷你模式ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            显示主窗口ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 94);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 0;
            label1.Text = "小说地址：";
            // 
            // TXTPath_textBox
            // 
            TXTPath_textBox.AllowDrop = true;
            TXTPath_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTPath_textBox.Location = new Point(92, 92);
            TXTPath_textBox.Name = "TXTPath_textBox";
            TXTPath_textBox.PlaceholderText = "输入txt路径或直接拖拽小说到此";
            TXTPath_textBox.Size = new Size(646, 23);
            TXTPath_textBox.TabIndex = 1;
            TXTPath_textBox.DragDrop += TXTPath_textBox_DragDrop;
            TXTPath_textBox.DragEnter += TXTPath_textBox_DragEnter;
            // 
            // Analyze_button
            // 
            Analyze_button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            Analyze_button.AutoSize = true;
            Analyze_button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Analyze_button.Location = new Point(744, 90);
            Analyze_button.Name = "Analyze_button";
            Analyze_button.Size = new Size(42, 27);
            Analyze_button.TabIndex = 2;
            Analyze_button.Text = "解析";
            Analyze_button.UseVisualStyleBackColor = true;
            Analyze_button.Click += Analyze_button_Click;
            // 
            // ChapterContent_richTextBox
            // 
            ChapterContent_richTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ChapterContent_richTextBox.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ChapterContent_richTextBox.Location = new Point(199, 126);
            ChapterContent_richTextBox.Name = "ChapterContent_richTextBox";
            ChapterContent_richTextBox.Size = new Size(593, 310);
            ChapterContent_richTextBox.TabIndex = 4;
            ChapterContent_richTextBox.Text = "";
            // 
            // LoadSet_button
            // 
            LoadSet_button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            LoadSet_button.AutoSize = true;
            LoadSet_button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            LoadSet_button.Location = new Point(660, 21);
            LoadSet_button.Name = "LoadSet_button";
            LoadSet_button.Size = new Size(66, 27);
            LoadSet_button.TabIndex = 11;
            LoadSet_button.Text = "载入设置";
            LoadSet_button.UseVisualStyleBackColor = true;
            LoadSet_button.Click += LoadSet_button_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(Set_button);
            groupBox1.Controls.Add(LoadSet_button);
            groupBox1.Controls.Add(Rule_textBox);
            groupBox1.Controls.Add(Rule_label);
            groupBox1.Controls.Add(Code_comboBox);
            groupBox1.Controls.Add(Code_label);
            groupBox1.Location = new Point(12, 28);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(780, 58);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "设置规则";
            // 
            // Set_button
            // 
            Set_button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            Set_button.AutoSize = true;
            Set_button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Set_button.Location = new Point(732, 22);
            Set_button.Name = "Set_button";
            Set_button.Size = new Size(42, 27);
            Set_button.TabIndex = 18;
            Set_button.Text = "保存";
            Set_button.UseVisualStyleBackColor = true;
            Set_button.Click += Set_button_Click;
            // 
            // Rule_textBox
            // 
            Rule_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Rule_textBox.Font = new Font("Microsoft YaHei UI", 8F);
            Rule_textBox.Location = new Point(68, 24);
            Rule_textBox.Name = "Rule_textBox";
            Rule_textBox.PlaceholderText = "第([一二两三四五六七八九十零百千万\\d]+)章|番外 ([一二两三四五六七八九十零百千万\\d]+)";
            Rule_textBox.Size = new Size(477, 21);
            Rule_textBox.TabIndex = 17;
            Rule_textBox.Tag = "第([一二两三四五六七八九十零百千万\\d]+)章|番外 ([一二两三四五六七八九十零百千万\\d]+)";
            Rule_textBox.Text = "第([一二两三四五六七八九十零百千万\\d]+)章|番外 ([一二两三四五六七八九十零百千万\\d]+)";
            // 
            // Rule_label
            // 
            Rule_label.AutoSize = true;
            Rule_label.Location = new Point(6, 27);
            Rule_label.Name = "Rule_label";
            Rule_label.Size = new Size(56, 17);
            Rule_label.TabIndex = 16;
            Rule_label.Text = "章节规则";
            Rule_label.Click += Rule_label_Click;
            // 
            // Code_comboBox
            // 
            Code_comboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Code_comboBox.FormattingEnabled = true;
            Code_comboBox.Items.AddRange(new object[] { "ANSI", "UTF-8", "UTF-8 BOM" });
            Code_comboBox.Location = new Point(593, 23);
            Code_comboBox.Name = "Code_comboBox";
            Code_comboBox.Size = new Size(61, 25);
            Code_comboBox.TabIndex = 15;
            Code_comboBox.Text = "UTF-8";
            // 
            // Code_label
            // 
            Code_label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Code_label.AutoSize = true;
            Code_label.Location = new Point(555, 26);
            Code_label.Name = "Code_label";
            Code_label.Size = new Size(32, 17);
            Code_label.TabIndex = 14;
            Code_label.Text = "编码";
            // 
            // Chapter_listBox
            // 
            Chapter_listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Chapter_listBox.FormattingEnabled = true;
            Chapter_listBox.ItemHeight = 17;
            Chapter_listBox.Location = new Point(12, 126);
            Chapter_listBox.Name = "Chapter_listBox";
            Chapter_listBox.ScrollAlwaysVisible = true;
            Chapter_listBox.Size = new Size(183, 310);
            Chapter_listBox.TabIndex = 24;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { Set_toolStripSplitButton, Speed_toolStripLabel, Speed_toolStripComboBox, ReadMode_toolStripLabel, ReadMode_toolStripComboBox, toolStripSeparator3, ReadAction_toolStripLabel, Pause_toolStripButton, Start_toolStripButton, NextChapter_toolStripButton, toolStripSeparator1, Save_toolStripButton, Save_toolStripComboBox, toolStripSeparator2 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(804, 25);
            toolStrip1.TabIndex = 26;
            toolStrip1.Text = "toolStrip1";
            // 
            // Set_toolStripSplitButton
            // 
            Set_toolStripSplitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Set_toolStripSplitButton.DropDownItems.AddRange(new ToolStripItem[] { 背景色ToolStripMenuItem, 字体ToolStripMenuItem, 语音包ToolStripMenuItem, 快捷键ToolStripMenuItem });
            Set_toolStripSplitButton.Image = (Image)resources.GetObject("Set_toolStripSplitButton.Image");
            Set_toolStripSplitButton.ImageTransparentColor = Color.Magenta;
            Set_toolStripSplitButton.Name = "Set_toolStripSplitButton";
            Set_toolStripSplitButton.Size = new Size(32, 22);
            Set_toolStripSplitButton.Text = "设置";
            Set_toolStripSplitButton.ToolTipText = "设置";
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
            // 快捷键ToolStripMenuItem
            // 
            快捷键ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 窗体切换ToolStripMenuItem, 最小化ToolStripMenuItem, 迷你窗体置顶ToolStripMenuItem, 开始朗读ToolStripMenuItem, 暂停朗读ToolStripMenuItem, 下一章ToolStripMenuItem, toolStripSeparator6, 重置默认ToolStripMenuItem });
            快捷键ToolStripMenuItem.Name = "快捷键ToolStripMenuItem";
            快捷键ToolStripMenuItem.Size = new Size(112, 22);
            快捷键ToolStripMenuItem.Text = "快捷键";
            // 
            // 窗体切换ToolStripMenuItem
            // 
            窗体切换ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Winswitch_shortcut_toolStripTextBox });
            窗体切换ToolStripMenuItem.Name = "窗体切换ToolStripMenuItem";
            窗体切换ToolStripMenuItem.Size = new Size(148, 22);
            窗体切换ToolStripMenuItem.Text = "窗体切换";
            窗体切换ToolStripMenuItem.Click += 窗体切换ToolStripMenuItem_Click;
            // 
            // Winswitch_shortcut_toolStripTextBox
            // 
            Winswitch_shortcut_toolStripTextBox.Name = "Winswitch_shortcut_toolStripTextBox";
            Winswitch_shortcut_toolStripTextBox.ReadOnly = true;
            Winswitch_shortcut_toolStripTextBox.Size = new Size(100, 23);
            Winswitch_shortcut_toolStripTextBox.Tag = "Ctrl+Alt+M";
            Winswitch_shortcut_toolStripTextBox.Text = "Ctrl+Alt+M";
            // 
            // 最小化ToolStripMenuItem
            // 
            最小化ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Minimize_shortcut_toolStripTextBox });
            最小化ToolStripMenuItem.Name = "最小化ToolStripMenuItem";
            最小化ToolStripMenuItem.Size = new Size(148, 22);
            最小化ToolStripMenuItem.Text = "最小化";
            最小化ToolStripMenuItem.Click += 最小化ToolStripMenuItem_Click;
            // 
            // Minimize_shortcut_toolStripTextBox
            // 
            Minimize_shortcut_toolStripTextBox.Name = "Minimize_shortcut_toolStripTextBox";
            Minimize_shortcut_toolStripTextBox.ReadOnly = true;
            Minimize_shortcut_toolStripTextBox.Size = new Size(100, 23);
            Minimize_shortcut_toolStripTextBox.Tag = "Ctrl+Alt+X";
            Minimize_shortcut_toolStripTextBox.Text = "Ctrl+Alt+X";
            // 
            // 迷你窗体置顶ToolStripMenuItem
            // 
            迷你窗体置顶ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MinTop_shortcut_toolStripTextBox });
            迷你窗体置顶ToolStripMenuItem.Name = "迷你窗体置顶ToolStripMenuItem";
            迷你窗体置顶ToolStripMenuItem.Size = new Size(148, 22);
            迷你窗体置顶ToolStripMenuItem.Text = "迷你窗体置顶";
            迷你窗体置顶ToolStripMenuItem.Click += 迷你窗体置顶ToolStripMenuItem_Click;
            // 
            // MinTop_shortcut_toolStripTextBox
            // 
            MinTop_shortcut_toolStripTextBox.Name = "MinTop_shortcut_toolStripTextBox";
            MinTop_shortcut_toolStripTextBox.ReadOnly = true;
            MinTop_shortcut_toolStripTextBox.Size = new Size(100, 23);
            MinTop_shortcut_toolStripTextBox.Tag = "Ctrl+Alt+T";
            MinTop_shortcut_toolStripTextBox.Text = "Ctrl+Alt+T";
            // 
            // 开始朗读ToolStripMenuItem
            // 
            开始朗读ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Startreading_shortcut_toolStripTextBox });
            开始朗读ToolStripMenuItem.Name = "开始朗读ToolStripMenuItem";
            开始朗读ToolStripMenuItem.Size = new Size(148, 22);
            开始朗读ToolStripMenuItem.Text = "开始朗读";
            开始朗读ToolStripMenuItem.Click += 开始朗读ToolStripMenuItem_Click;
            // 
            // Startreading_shortcut_toolStripTextBox
            // 
            Startreading_shortcut_toolStripTextBox.Name = "Startreading_shortcut_toolStripTextBox";
            Startreading_shortcut_toolStripTextBox.ReadOnly = true;
            Startreading_shortcut_toolStripTextBox.Size = new Size(100, 23);
            Startreading_shortcut_toolStripTextBox.Tag = "Ctrl+K";
            Startreading_shortcut_toolStripTextBox.Text = "Ctrl+K";
            // 
            // 暂停朗读ToolStripMenuItem
            // 
            暂停朗读ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Pausereading_shortcut_toolStripTextBox });
            暂停朗读ToolStripMenuItem.Name = "暂停朗读ToolStripMenuItem";
            暂停朗读ToolStripMenuItem.Size = new Size(148, 22);
            暂停朗读ToolStripMenuItem.Text = "暂停朗读";
            暂停朗读ToolStripMenuItem.Click += 暂停朗读ToolStripMenuItem_Click;
            // 
            // Pausereading_shortcut_toolStripTextBox
            // 
            Pausereading_shortcut_toolStripTextBox.Name = "Pausereading_shortcut_toolStripTextBox";
            Pausereading_shortcut_toolStripTextBox.ReadOnly = true;
            Pausereading_shortcut_toolStripTextBox.Size = new Size(100, 23);
            Pausereading_shortcut_toolStripTextBox.Tag = "Ctrl+Z";
            Pausereading_shortcut_toolStripTextBox.Text = "Ctrl+Z";
            // 
            // 下一章ToolStripMenuItem
            // 
            下一章ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { Nextchapterreading_shortcut_toolStripTextBox });
            下一章ToolStripMenuItem.Name = "下一章ToolStripMenuItem";
            下一章ToolStripMenuItem.Size = new Size(148, 22);
            下一章ToolStripMenuItem.Text = "下一章";
            下一章ToolStripMenuItem.Click += 下一章ToolStripMenuItem_Click;
            // 
            // Nextchapterreading_shortcut_toolStripTextBox
            // 
            Nextchapterreading_shortcut_toolStripTextBox.Name = "Nextchapterreading_shortcut_toolStripTextBox";
            Nextchapterreading_shortcut_toolStripTextBox.ReadOnly = true;
            Nextchapterreading_shortcut_toolStripTextBox.Size = new Size(100, 23);
            Nextchapterreading_shortcut_toolStripTextBox.Tag = "Ctrl+N";
            Nextchapterreading_shortcut_toolStripTextBox.Text = "Ctrl+N";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(145, 6);
            // 
            // 重置默认ToolStripMenuItem
            // 
            重置默认ToolStripMenuItem.Name = "重置默认ToolStripMenuItem";
            重置默认ToolStripMenuItem.Size = new Size(148, 22);
            重置默认ToolStripMenuItem.Text = "重置默认";
            重置默认ToolStripMenuItem.Click += 重置默认ToolStripMenuItem_Click;
            // 
            // Speed_toolStripLabel
            // 
            Speed_toolStripLabel.Name = "Speed_toolStripLabel";
            Speed_toolStripLabel.Size = new Size(32, 22);
            Speed_toolStripLabel.Text = "语速";
            // 
            // Speed_toolStripComboBox
            // 
            Speed_toolStripComboBox.AutoSize = false;
            Speed_toolStripComboBox.Items.AddRange(new object[] { "-10", "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2", "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            Speed_toolStripComboBox.Name = "Speed_toolStripComboBox";
            Speed_toolStripComboBox.Size = new Size(40, 25);
            Speed_toolStripComboBox.Text = "5";
            Speed_toolStripComboBox.ToolTipText = "-10~10的整数";
            // 
            // ReadMode_toolStripLabel
            // 
            ReadMode_toolStripLabel.Name = "ReadMode_toolStripLabel";
            ReadMode_toolStripLabel.Size = new Size(56, 22);
            ReadMode_toolStripLabel.Text = "朗读模式";
            // 
            // ReadMode_toolStripComboBox
            // 
            ReadMode_toolStripComboBox.AutoSize = false;
            ReadMode_toolStripComboBox.Items.AddRange(new object[] { "整行" });
            ReadMode_toolStripComboBox.Name = "ReadMode_toolStripComboBox";
            ReadMode_toolStripComboBox.Size = new Size(70, 25);
            ReadMode_toolStripComboBox.Text = "整行";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // ReadAction_toolStripLabel
            // 
            ReadAction_toolStripLabel.Name = "ReadAction_toolStripLabel";
            ReadAction_toolStripLabel.Size = new Size(56, 22);
            ReadAction_toolStripLabel.Text = "朗读操作";
            // 
            // Pause_toolStripButton
            // 
            Pause_toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Pause_toolStripButton.Image = (Image)resources.GetObject("Pause_toolStripButton.Image");
            Pause_toolStripButton.ImageTransparentColor = Color.Magenta;
            Pause_toolStripButton.Name = "Pause_toolStripButton";
            Pause_toolStripButton.Size = new Size(23, 22);
            Pause_toolStripButton.Text = "暂停";
            Pause_toolStripButton.Click += Pause_toolStripButton_Click;
            // 
            // Start_toolStripButton
            // 
            Start_toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Start_toolStripButton.Image = (Image)resources.GetObject("Start_toolStripButton.Image");
            Start_toolStripButton.ImageTransparentColor = Color.Magenta;
            Start_toolStripButton.Name = "Start_toolStripButton";
            Start_toolStripButton.Size = new Size(23, 22);
            Start_toolStripButton.Text = "开始";
            Start_toolStripButton.Click += Start_toolStripButton_Click;
            // 
            // NextChapter_toolStripButton
            // 
            NextChapter_toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NextChapter_toolStripButton.Image = (Image)resources.GetObject("NextChapter_toolStripButton.Image");
            NextChapter_toolStripButton.ImageTransparentColor = Color.Magenta;
            NextChapter_toolStripButton.Name = "NextChapter_toolStripButton";
            NextChapter_toolStripButton.Size = new Size(23, 22);
            NextChapter_toolStripButton.Text = "下一章";
            NextChapter_toolStripButton.Click += NextChapter_toolStripButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // Save_toolStripButton
            // 
            Save_toolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Save_toolStripButton.Image = (Image)resources.GetObject("Save_toolStripButton.Image");
            Save_toolStripButton.ImageTransparentColor = Color.Magenta;
            Save_toolStripButton.Name = "Save_toolStripButton";
            Save_toolStripButton.Size = new Size(23, 22);
            Save_toolStripButton.Text = "保存";
            Save_toolStripButton.ToolTipText = "保存(Ctrl+Alt+S)";
            Save_toolStripButton.Click += Save_toolStripButton_Click;
            // 
            // Save_toolStripComboBox
            // 
            Save_toolStripComboBox.Items.AddRange(new object[] { "覆盖保存", "另存带日期戳", "另存带时间戳" });
            Save_toolStripComboBox.Name = "Save_toolStripComboBox";
            Save_toolStripComboBox.Size = new Size(121, 25);
            Save_toolStripComboBox.Text = "覆盖保存";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { Info_toolStripStatusLabel, LogStatus_toolStripStatusLabel, CurrentChapterLabel_toolStripStatusLabel, CurrentChapter_toolStripStatusLabel, Currentlinelabel_toolStripStatusLabel, CurrentLine_toolStripStatusLabel });
            statusStrip1.Location = new Point(0, 449);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(804, 22);
            statusStrip1.TabIndex = 27;
            statusStrip1.Text = "statusStrip1";
            // 
            // Info_toolStripStatusLabel
            // 
            Info_toolStripStatusLabel.Name = "Info_toolStripStatusLabel";
            Info_toolStripStatusLabel.Size = new Size(44, 17);
            Info_toolStripStatusLabel.Text = "状态：";
            // 
            // LogStatus_toolStripStatusLabel
            // 
            LogStatus_toolStripStatusLabel.Name = "LogStatus_toolStripStatusLabel";
            LogStatus_toolStripStatusLabel.Size = new Size(606, 17);
            LogStatus_toolStripStatusLabel.Spring = true;
            LogStatus_toolStripStatusLabel.Text = " ";
            LogStatus_toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // CurrentChapterLabel_toolStripStatusLabel
            // 
            CurrentChapterLabel_toolStripStatusLabel.Name = "CurrentChapterLabel_toolStripStatusLabel";
            CurrentChapterLabel_toolStripStatusLabel.Size = new Size(56, 17);
            CurrentChapterLabel_toolStripStatusLabel.Text = "当前章：";
            // 
            // CurrentChapter_toolStripStatusLabel
            // 
            CurrentChapter_toolStripStatusLabel.Name = "CurrentChapter_toolStripStatusLabel";
            CurrentChapter_toolStripStatusLabel.Size = new Size(12, 17);
            CurrentChapter_toolStripStatusLabel.Text = " ";
            CurrentChapter_toolStripStatusLabel.ToolTipText = "当前章";
            // 
            // Currentlinelabel_toolStripStatusLabel
            // 
            Currentlinelabel_toolStripStatusLabel.Name = "Currentlinelabel_toolStripStatusLabel";
            Currentlinelabel_toolStripStatusLabel.Size = new Size(56, 17);
            Currentlinelabel_toolStripStatusLabel.Text = "当前行：";
            // 
            // CurrentLine_toolStripStatusLabel
            // 
            CurrentLine_toolStripStatusLabel.Name = "CurrentLine_toolStripStatusLabel";
            CurrentLine_toolStripStatusLabel.Size = new Size(15, 17);
            CurrentLine_toolStripStatusLabel.Text = "0";
            CurrentLine_toolStripStatusLabel.ToolTipText = "当前行";
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "ReadTXT";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 迷你模式ToolStripMenuItem, toolStripSeparator4, 显示主窗口ToolStripMenuItem, toolStripSeparator5, 退出ToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(137, 82);
            // 
            // 迷你模式ToolStripMenuItem
            // 
            迷你模式ToolStripMenuItem.Name = "迷你模式ToolStripMenuItem";
            迷你模式ToolStripMenuItem.Size = new Size(136, 22);
            迷你模式ToolStripMenuItem.Text = "迷你模式";
            迷你模式ToolStripMenuItem.Click += 迷你模式ToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(133, 6);
            // 
            // 显示主窗口ToolStripMenuItem
            // 
            显示主窗口ToolStripMenuItem.Name = "显示主窗口ToolStripMenuItem";
            显示主窗口ToolStripMenuItem.Size = new Size(136, 22);
            显示主窗口ToolStripMenuItem.Text = "显示主窗口";
            显示主窗口ToolStripMenuItem.Click += 显示主窗口ToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(133, 6);
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(136, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // ReadTXT
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 471);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(Chapter_listBox);
            Controls.Add(groupBox1);
            Controls.Add(ChapterContent_richTextBox);
            Controls.Add(Analyze_button);
            Controls.Add(TXTPath_textBox);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(820, 510);
            Name = "ReadTXT";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ReadTXT";
            Load += ReadTXT_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox TXTPath_textBox;
        private Button Analyze_button;
        private RichTextBox ChapterContent_richTextBox;
        private Button LoadSet_button;
        private GroupBox groupBox1;
        private TextBox Rule_textBox;
        private Label Rule_label;
        private ComboBox Code_comboBox;
        private Label Code_label;
        private ListBox Chapter_listBox;
        private ToolStrip toolStrip1;
        private ToolStripLabel Speed_toolStripLabel;
        private ToolStripComboBox Speed_toolStripComboBox;
        private ToolStripLabel ReadMode_toolStripLabel;
        private ToolStripComboBox ReadMode_toolStripComboBox;
        private ToolStripLabel ReadAction_toolStripLabel;
        private ToolStripButton Pause_toolStripButton;
        private ToolStripButton Start_toolStripButton;
        private ToolStripButton NextChapter_toolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private FontDialog fontDialog1;
        private ColorDialog colorDialog1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSplitButton Set_toolStripSplitButton;
        private ToolStripMenuItem 背景色ToolStripMenuItem;
        private ToolStripMenuItem 字体ToolStripMenuItem;
        private Button Set_button;
        private ToolStripMenuItem 语音包ToolStripMenuItem;
        private ToolStripButton Save_toolStripButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel Info_toolStripStatusLabel;
        private ToolStripStatusLabel LogStatus_toolStripStatusLabel;
        private ToolStripStatusLabel CurrentChapterLabel_toolStripStatusLabel;
        private ToolStripStatusLabel CurrentChapter_toolStripStatusLabel;
        private ToolStripStatusLabel Currentlinelabel_toolStripStatusLabel;
        private ToolStripStatusLabel CurrentLine_toolStripStatusLabel;
        private ToolStripComboBox Save_toolStripComboBox;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 迷你模式ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem 显示主窗口ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private ToolStripMenuItem 快捷键ToolStripMenuItem;
        private ToolStripMenuItem 窗体切换ToolStripMenuItem;
        private ToolStripMenuItem 最小化ToolStripMenuItem;
        private ToolStripMenuItem 迷你窗体置顶ToolStripMenuItem;
        private ToolStripMenuItem 开始朗读ToolStripMenuItem;
        private ToolStripTextBox Winswitch_shortcut_toolStripTextBox;
        private ToolStripTextBox Minimize_shortcut_toolStripTextBox;
        private ToolStripTextBox MinTop_shortcut_toolStripTextBox;
        private ToolStripTextBox Startreading_shortcut_toolStripTextBox;
        private ToolStripMenuItem 暂停朗读ToolStripMenuItem;
        private ToolStripTextBox Pausereading_shortcut_toolStripTextBox;
        private ToolStripMenuItem 下一章ToolStripMenuItem;
        private ToolStripTextBox Nextchapterreading_shortcut_toolStripTextBox;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem 重置默认ToolStripMenuItem;
    }
}
