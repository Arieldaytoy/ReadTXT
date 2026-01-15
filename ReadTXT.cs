using System.Diagnostics;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Timer = System.Windows.Forms.Timer;

namespace ReadTXT
{
    public partial class ReadTXT : Form
    {
        #region 自定义变量
        private Dictionary<string, string> chapters = [];

        private string currentChapterTitle;
        private int currentLineIndex = 0;

        private string previousChapter = "";
        private int pauseLineIndex = -1;

        //朗读相关
        private SpeechSynthesizer synthesizer = new();
        private bool isSpeaking = false;//是否开启朗读标识
        private bool waitForRealCompletion = false;//朗读状态
        private bool isResuming = false;//朗读暂停标识
        private Timer completionTimer;


        // 迷你模式窗口
        public MiniModeForm? miniModeForm = null;
        public bool isMinimizedToTray = false;
        public bool IsMinimizedToTray => isMinimizedToTray;

        private PatternItem patternConfig;
        private HotkeyManager hotkeyManager;

        // 添加全局键盘钩子
        private GlobalKeyboardHook? keyboardHook = null;

        // 默认快捷键字典
        private static readonly Dictionary<string, string> DefaultHotkeys = new()
        {
            { "ToggleMode", "Ctrl+Alt+M" },
            { "MinimizeOrClose", "Ctrl+Alt+X" },
            { "ToggleTopMost", "Ctrl+Alt+T" },
            { "StartReading", "Ctrl+K" },
            { "StopReading", "Ctrl+Z" },
            { "NextChapter", "Ctrl+N" },
            { "SaveDocument", "Ctrl+Alt+S" }
        };

        // 切换方法，添加状态标志避免竞态条件
        public bool isSwitchingMode = false;

        public class BlackColor
        {
            [JsonPropertyName("r")]
            public byte R { get; set; }

            [JsonPropertyName("g")]
            public byte G { get; set; }

            [JsonPropertyName("b")]
            public byte B { get; set; }

            [JsonPropertyName("a")]
            public byte A { get; set; }

            [JsonIgnore]
            public Color Color => Color.FromArgb(A, R, G, B);
        }

        public class PatternItem
        {
            [JsonPropertyName("txtPathTextBoxPattern")]
            public string? TXTPath_textBoxPattern { get; set; }

            [JsonPropertyName("ruleTextBoxPattern")]
            public string? Rule_textBoxPattern { get; set; }

            [JsonPropertyName("speedToolStripComboBoxPattern")]
            public string? Speed_toolStripComboBoxPattern { get; set; }

            [JsonPropertyName("readModeToolStripComboBoxPattern")]
            public string? ReadMode_toolStripComboBoxPattern { get; set; }

            [JsonPropertyName("currentChapterToolStripStatusLabelPattern")]
            public string? CurrentChapter_toolStripStatusLabelPattern { get; set; }

            [JsonPropertyName("currentLineToolStripStatusLabelPattern")]
            public string? CurrentLine_toolStripStatusLabelPattern { get; set; }

            [JsonPropertyName("fontName")]
            public string? FontName { get; set; }

            [JsonPropertyName("fontStyle")]
            public int FontStyle { get; set; }

            [JsonPropertyName("fontSize")]
            public double FontSize { get; set; }

            [JsonPropertyName("blackColor")]
            public required BlackColor BlackColor { get; set; }

            [JsonPropertyName("hotkeysForJson")]
            public Dictionary<string, string> HotkeysForJson { get; set; } = new(ReadTXT.DefaultHotkeys);

            [JsonIgnore]
            public Dictionary<string, string> Hotkeys
            {
                get => HotkeysForJson;
                set => HotkeysForJson = value;
            }
        }

        public class PatternsContainer
        {
            [JsonPropertyName("patterns")]
            public required PatternItem[] Patterns { get; set; }
        }

        private static PatternsContainer? ReadJsonPatterns(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                // 使用相同的序列化选项
                return JsonSerializer.Deserialize<PatternsContainer>(jsonContent, _serializerOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取JSON文件时出错: {ex.Message}");
                return null;
            }
        }

        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            // 使序列化时属性名使用驼峰命名
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // 使反序列化时不区分大小写
            PropertyNameCaseInsensitive = true
        };

        public static string SerializeObject<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _serializerOptions);
        }

        #endregion

        #region 窗体初始化配置
        public ReadTXT()
        {
            InitializeComponent();
            InitializeSynthesizer();
        }

        /// <summary>
        /// 初始化窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadTXT_Load(object sender, EventArgs e)
        {
            this.Resize += ReadTXT_Resize;
            LoadTTSVoices();
            Load_set();
            InitializeHotkeys(); // 初始化热键管理器            
            Analyze_novel();
            RestorePreviousChapterSelection();
            SetRichTextBoxTextColor(ChapterContent_richTextBox, Color.Black, "0");

            if (int.TryParse(this.Speed_toolStripComboBox.Text, out int rate))
            {
                synthesizer.Rate = rate;
            }
            else
            {
                this.Speed_toolStripComboBox.Text = "0";
                synthesizer.Rate = 0;
                this.LogStatus_toolStripStatusLabel.Text = "朗读速度只能设置-10~10的整数！！！";
                this.LogStatus_toolStripStatusLabel.ForeColor = Color.Red;
            }
        }

        #endregion

        #region 热键管理器初始化
        private void InitializeHotkeys()
        {
            // 确保patternConfig已加载
            if (patternConfig == null)
            {
                // 如果配置为空，创建默认配置
                patternConfig = new PatternItem
                {
                    BlackColor = new BlackColor { R = 255, G = 255, B = 255, A = 255 },
                    Hotkeys = new Dictionary<string, string>(DefaultHotkeys)
                };
            }

            // 初始化热键管理器
            hotkeyManager = new HotkeyManager(this, patternConfig);

            // 初始化全局键盘钩子
            keyboardHook = new GlobalKeyboardHook();
            keyboardHook.HotkeyPressed += (s, e) =>
            {
                if (hotkeyManager != null)
                {
                    bool handled = hotkeyManager.ProcessHotkey(e.HotkeyString);
                    e.Handled = handled;

                    if (handled)
                    {
                        Debug.WriteLine($"热键触发: {e.HotkeyString}");
                    }
                }
            };

            // 注册应用程序关闭时清理钩子
            Application.ApplicationExit += (s, e) =>
            {
                keyboardHook?.Dispose();
            };
        }
        #endregion



        #region 工具栏按钮事件

        #region 工具栏按钮-设置相关事件
        //设置背景色
        private void 背景色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color selectedColor = colorDialog1.Color;
                this.ChapterContent_richTextBox.BackColor = selectedColor;
                this.Chapter_listBox.BackColor = selectedColor;
            }
        }
        //设置字体
        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                Font selectedFont = fontDialog1.Font;
                ChapterContent_richTextBox.Font = selectedFont;
            }
        }
        #endregion

        #region 工具栏-朗读操作-暂停、开始、下一章
        //暂停朗读
        private void Pause_toolStripButton_Click(object sender, EventArgs e)
        {
            StopRead();
        }
        //开始朗读
        private void Start_toolStripButton_Click(object sender, EventArgs e)
        {
            StopRead();
            StartRead();
        }
        // 下一章朗读
        private void NextChapter_toolStripButton_Click(object? sender, EventArgs? e)
        {
            StopRead();
            GoToNextChapter();
            StartReadFromBeginning(); // 切换到新章节时从头开始
        }
        #endregion

        #region 工具栏-保存按钮
        //保存编辑后全文
        public void Save_toolStripButton_Click(object? sender, EventArgs? e)
        {
            string originalFilePath = this.TXTPath_textBox.Text;
            string directory = Path.GetDirectoryName(originalFilePath);
            string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = Path.GetExtension(originalFilePath);

            string newFileName = $"{fileName}{extension}";
            DateTime now = DateTime.Now;

            if (Save_toolStripComboBox.Text == "另存带日期戳")
            {
                string timestamp = now.ToString("yyyyMMdd");
                newFileName = $"{fileName}-{timestamp}{extension}";
            }
            else if (Save_toolStripComboBox.Text == "另存带时间戳")
            {
                string timestamp = now.ToString("yyyyMMddHHmmss");
                newFileName = $"{fileName}-{timestamp}{extension}";
            }

            string newFilePath = Path.Combine(directory, newFileName);
            Encoding selectedEncoding = this.Code_comboBox.SelectedItem.ToString() switch
            {
                "UTF-8" => Encoding.UTF8,
                "UTF-8 BOM" => new UTF8Encoding(true),
                "ANSI" => Encoding.Default,
                _ => Encoding.UTF8,
            };

            if (!string.IsNullOrEmpty(currentChapterTitle))
            {
                chapters[currentChapterTitle] = this.ChapterContent_richTextBox.Text;
                using StreamWriter writer = new(newFilePath, false, selectedEncoding);
                foreach (var chapter in chapters)
                {
                    writer.Write(chapter.Value);
                    writer.WriteLine();
                }
            }
            else
            {
                using StreamWriter writer = new(newFilePath, false, selectedEncoding);
                writer.Write(this.ChapterContent_richTextBox.Text);
            }
            this.LogStatus_toolStripStatusLabel.Text = "保存成功，路径：" + newFilePath;
            this.LogStatus_toolStripStatusLabel.ForeColor = Color.Black;
        }
        #endregion

        #endregion

        #region 设置规则组按钮事件
        //点击标签加载默认规则
        private void Rule_label_Click(object sender, EventArgs e)
        {
            this.Rule_textBox.Text = this.Rule_textBox.PlaceholderText;
        }

        //加载用户设置
        private void LoadSet_button_Click(object sender, EventArgs e)
        {
            Load_set();
            if (previousChapter != "")
            {
                RestorePreviousChapterSelection();
                UpdateText();
            }
        }

        // 保存用户设置
        private void Set_button_Click(object sender, EventArgs e)
        {
            // 如果patternConfig为空，创建新的
            if (patternConfig == null)
            {
                patternConfig = new PatternItem
                {
                    BlackColor = new BlackColor { R = 255, G = 255, B = 255, A = 255 }
                };
            }

            // 更新配置
            patternConfig.TXTPath_textBoxPattern = this.TXTPath_textBox.Text;
            patternConfig.Rule_textBoxPattern = this.Rule_textBox.Text;
            patternConfig.Speed_toolStripComboBoxPattern = this.Speed_toolStripComboBox.Text;
            patternConfig.ReadMode_toolStripComboBoxPattern = this.ReadMode_toolStripComboBox.Text;
            patternConfig.CurrentChapter_toolStripStatusLabelPattern = this.CurrentChapter_toolStripStatusLabel.Text;
            patternConfig.CurrentLine_toolStripStatusLabelPattern = this.CurrentLine_toolStripStatusLabel.Text;
            patternConfig.FontName = this.ChapterContent_richTextBox.Font.Name;
            patternConfig.FontStyle = (int)this.ChapterContent_richTextBox.Font.Style;
            patternConfig.FontSize = this.ChapterContent_richTextBox.Font.Size;
            patternConfig.BlackColor.R = this.ChapterContent_richTextBox.BackColor.R;
            patternConfig.BlackColor.G = this.ChapterContent_richTextBox.BackColor.G;
            patternConfig.BlackColor.B = this.ChapterContent_richTextBox.BackColor.B;
            patternConfig.BlackColor.A = this.ChapterContent_richTextBox.BackColor.A;

            // 确保HotkeysForJson字典存在
            if (patternConfig.HotkeysForJson == null)
            {
                patternConfig.HotkeysForJson = new Dictionary<string, string>();
            }

            // 保存热键配置（从文本框获取当前设置）
            patternConfig.HotkeysForJson["ToggleMode"] = Winswitch_shortcut_toolStripTextBox.Text;
            patternConfig.HotkeysForJson["MinimizeOrClose"] = Minimize_shortcut_toolStripTextBox.Text;
            patternConfig.HotkeysForJson["ToggleTopMost"] = MinTop_shortcut_toolStripTextBox.Text;
            patternConfig.HotkeysForJson["StartReading"] = Startreading_shortcut_toolStripTextBox.Text;
            patternConfig.HotkeysForJson["StopReading"] = Pausereading_shortcut_toolStripTextBox.Text;
            patternConfig.HotkeysForJson["NextChapter"] = Nextchapterreading_shortcut_toolStripTextBox.Text;

            // 保存到文件
            var patterns = new PatternsContainer
            {
                Patterns = new[] { patternConfig }
            };

            string runningDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(runningDirectory, "mySet.json");
            string jsonString = JsonSerializer.Serialize(patterns, _serializerOptions);
            File.WriteAllText(jsonFilePath, jsonString);

            this.LogStatus_toolStripStatusLabel.Text = "设置已保存，路径：" + jsonFilePath;
            this.LogStatus_toolStripStatusLabel.ForeColor = Color.Black;
        }

        #endregion

        #region 解析按钮相关事件
        //小说路径文本框拖拽输入
        private void TXTPath_textBox_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void TXTPath_textBox_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
                {
                    this.TXTPath_textBox.Text = files[0];
                }
            }
        }
        //解析章节和正文
        private void Analyze_button_Click(object sender, EventArgs e)
        {
            Analyze_novel();
        }
        #endregion

        #region 选中章节，加载对应正文        
        private void Chapter_listBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.CurrentChapter_toolStripStatusLabel.Text != null)
            {
                chapters[this.CurrentChapter_toolStripStatusLabel.Text] = this.ChapterContent_richTextBox.Text;
            }
            this.CurrentChapter_toolStripStatusLabel.Text = Chapter_listBox.Items[Chapter_listBox.SelectedIndex].ToString() ?? "Default Value";
            UpdateText();

            pauseLineIndex = -1;
            isResuming = false;

            ChapterContent_richTextBox.Select(0, 0);
            ChapterContent_richTextBox.ScrollToCaret();
        }
        #endregion

        #region 托盘双击事件
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreFromTray();
        }
        #endregion

        #region 托盘右键菜单事件

        private void 迷你模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMiniMode();
        }

        private void 显示主窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("确定要退出小说朗读器吗？",
                "确认退出",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                notifyIcon1.Visible = false;
                keyboardHook?.Dispose();
                Application.Exit();
            }
        }

        #endregion

        #region 程序退出事件
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 释放键盘钩子
            keyboardHook?.Dispose();
            keyboardHook = null;

            // 只有当用户点击关闭按钮时才最小化到托盘
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MinimizeToTray();
                return;
            }

            // 其他关闭原因（如系统关闭、任务管理器结束等）正常关闭
            base.OnFormClosing(e);
            notifyIcon1.Visible=false;
        }

        #endregion

        #region 辅助方法-初始化相关
        //加载系统语言包
        private void LoadTTSVoices()
        {
            语音包ToolStripMenuItem.DropDownItems.Clear();
            foreach (var voice in synthesizer.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                ToolStripMenuItem voiceItem = new()
                {
                    Text = $"{info.Name} ({info.Culture.Name}) - {info.Gender}, {info.Age}",
                    CheckOnClick = true,
                    Tag = info.Name
                };
                语音包ToolStripMenuItem.DropDownItems.Add(voiceItem);
                voiceItem.Click += (sender, e) =>
                {
                    if (sender is ToolStripMenuItem { Tag: string voiceName })
                    {
                        SelectVoice(voiceName);
                    }
                };
            }
            if (语音包ToolStripMenuItem.DropDownItems.Count == 0)
            {
                语音包ToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("没有安装的语音包"));
            }
        }
        //设置语言包
        private void SelectVoice(string voiceName)
        {
            if (!string.IsNullOrEmpty(voiceName))
            {
                try
                {
                    synthesizer.SelectVoice(voiceName);
                    if (语音包ToolStripMenuItem.DropDownItems.Count > 0)
                    {
                        foreach (ToolStripMenuItem dropitem in 语音包ToolStripMenuItem.DropDownItems)
                        {
                            if (dropitem.Tag != null && dropitem.Tag.ToString() == voiceName)
                            {
                                dropitem.Checked = true;
                            }
                            else
                            {
                                dropitem.Checked = false;
                            }
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show($"无法选择语音：{ex.Message}");
                }
            }
        }

        // 加载上次的配置文件设置
        private void Load_set()
        {
            string jsonPath = Path.Combine(Application.StartupPath, "mySet.json");
            bool fileExists = File.Exists(jsonPath);
            if (fileExists)
            {
                PatternsContainer? patterns = ReadJsonPatterns(jsonPath);
                if (patterns?.Patterns != null && patterns.Patterns.Length > 0)
                {
                    PatternItem firstPattern = patterns.Patterns[0];
                    this.TXTPath_textBox.Text = firstPattern.TXTPath_textBoxPattern;
                    this.Rule_textBox.Text = firstPattern.Rule_textBoxPattern;
                    this.Speed_toolStripComboBox.Text = firstPattern.Speed_toolStripComboBoxPattern;
                    this.ReadMode_toolStripComboBox.Text = firstPattern.ReadMode_toolStripComboBoxPattern;
                    this.CurrentChapter_toolStripStatusLabel.Text = firstPattern.CurrentChapter_toolStripStatusLabelPattern;
                    previousChapter = firstPattern.CurrentChapter_toolStripStatusLabelPattern ?? "";

                    // 加载行号并设置到currentLineIndex
                    if (!string.IsNullOrEmpty(firstPattern.CurrentLine_toolStripStatusLabelPattern))
                    {
                        if (int.TryParse(firstPattern.CurrentLine_toolStripStatusLabelPattern, out int savedLineIndex))
                        {
                            currentLineIndex = savedLineIndex;
                            this.CurrentLine_toolStripStatusLabel.Text = savedLineIndex.ToString();
                        }
                    }

                    FontStyle fontStyle = (FontStyle)firstPattern.FontStyle;
                    Font newFont = new(
                        familyName: firstPattern.FontName ?? "Microsoft YaHei UI",
                        (float)firstPattern.FontSize,
                        fontStyle
                    );
                    this.ChapterContent_richTextBox.Font = newFont;
                    this.ChapterContent_richTextBox.BackColor = firstPattern.BlackColor.Color;
                    this.Chapter_listBox.BackColor = firstPattern.BlackColor.Color;

                    // 保存到patternConfig字段
                    patternConfig = firstPattern;

                    // 确保Hotkeys不为空 - 修改这里
                    if (patternConfig.Hotkeys == null || patternConfig.Hotkeys.Count == 0)
                    {
                        patternConfig.Hotkeys = new Dictionary<string, string>(DefaultHotkeys);
                    }

                    // 更新文本框显示
                    UpdateHotkeyTextBoxes();
                }
                else
                {
                    this.LogStatus_toolStripStatusLabel.Text = "JSON文件中没有有效的规则设置，正在创建默认配置文件...";                    
                    CreateDefaultConfig();
                }
            }
            else
            {
                this.LogStatus_toolStripStatusLabel.Text = "文件不存在: " + jsonPath;
                this.LogStatus_toolStripStatusLabel.ForeColor = Color.Red;
            }
        }

        //恢复上一次的章节选择
        private void RestorePreviousChapterSelection()
        {
            if (string.IsNullOrEmpty(previousChapter)) return;

            bool found = false;
            for (int i = 0; i < Chapter_listBox.Items.Count; i++)
            {
                if (Chapter_listBox.Items[i].ToString() == previousChapter)
                {
                    Chapter_listBox.SelectedIndex = i;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                LogStatus_toolStripStatusLabel.Text = "你可能换了一本小说，找不到上次看的那章>_<";
            }
        }
        #endregion

        #region 辅助方法-解析小说结构
        //解析小说章节
        private void Analyze_novel()
        {
            this.LogStatus_toolStripStatusLabel.Text = "";
            this.CurrentChapter_toolStripStatusLabel.Text = "";
            this.CurrentLine_toolStripStatusLabel.Text = currentLineIndex.ToString(); // 使用保存的行号
            this.Chapter_listBox.Items.Clear();
            currentChapterTitle = "";
            this.ChapterContent_richTextBox.Text = "";
            chapters = [];
            string filePath = this.TXTPath_textBox.Text;
            if (filePath != "")
            {
                try
                {
                    LoadFileContent(filePath);
                    PopulateListBox();
                    Chapter_listBox.SelectedIndexChanged += Chapter_listBox_SelectedIndexChanged;
                    if (this.Chapter_listBox.Items.Count > 0)
                    {
                        if (this.CurrentChapter_toolStripStatusLabel.Text == "")
                        {
                            this.LogStatus_toolStripStatusLabel.Text = "解析完成，开始阅读吧~";
                        }
                        else
                        {
                            this.LogStatus_toolStripStatusLabel.Text = "已定位到上次的章节，继续阅读吧~";
                        }
                        this.LogStatus_toolStripStatusLabel.ForeColor = Color.Black;
                    }
                    else
                    {
                        this.LogStatus_toolStripStatusLabel.Text = "未能解析到相关章节！请确认文件中有与规则匹配的章节标题。";
                        this.LogStatus_toolStripStatusLabel.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    this.LogStatus_toolStripStatusLabel.Text = "读取文件时出错: " + ex.Message;
                    this.LogStatus_toolStripStatusLabel.ForeColor = Color.Red;
                }
            }
        }
        /// <summary>
        /// 加载小说内容，章节chapterTitle+正文chapterContent
        /// </summary>
        /// <param name="filePath">小说路径</param>
        private void LoadFileContent(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (this.Rule_textBox.Text != "")
            {
                for (int j = 0; j < lines.Length;)
                {
                    if (Regex.IsMatch(lines[j], this.Rule_textBox.Text))
                    {
                        string chapterTitle = lines[j];
                        string chapterContent = "";
                        j++;
                        int contentIndex = j;
                        while (contentIndex < lines.Length && !Regex.IsMatch(lines[contentIndex], this.Rule_textBox.Text))
                        {
                            chapterContent += lines[contentIndex] + Environment.NewLine;
                            contentIndex++;
                        }
                        chapters[chapterTitle] = chapterTitle + Environment.NewLine + chapterContent;
                        j = contentIndex;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            else
            {
                ChapterContent_richTextBox.Text = "";
                ChapterContent_richTextBox.Text = string.Join(Environment.NewLine, lines);
            }
        }
        /// <summary>
        /// 将章节清单加载到listBox1
        /// </summary>
        private void PopulateListBox()
        {
            if (chapters != null)
            {
                Chapter_listBox.Items.Clear();
                foreach (var chapter in chapters.Keys)
                {
                    Chapter_listBox.Items.Add(chapter);
                }
            }
            else
            {
                this.LogStatus_toolStripStatusLabel.Text = "未能匹配到相关章节！建议查看原始文档中章节标题是否与章节规则匹配！！！";
                this.LogStatus_toolStripStatusLabel.ForeColor = Color.Red;
            }
        }
        #endregion

        #region 辅助方法-其他

        //设置富文本-高亮朗读文本段为红色
        private void HighlightCurrentLine(Color color)
        {
            if (currentLineIndex < ChapterContent_richTextBox.Lines.Length)
            {
                string currentLineText = ChapterContent_richTextBox.Lines[currentLineIndex];

                if (string.IsNullOrWhiteSpace(currentLineText))
                {
                    return;
                }
                int startIndex = 0;
                for (int i = 0; i < currentLineIndex; i++)
                {
                    startIndex += ChapterContent_richTextBox.Lines[i].Length + 1;
                }

                int length = currentLineText.Length;

                ChapterContent_richTextBox.SelectAll();
                ChapterContent_richTextBox.SelectionColor = Color.Black;

                if (startIndex >= 0 && startIndex + length <= ChapterContent_richTextBox.Text.Length)
                {
                    ChapterContent_richTextBox.Select(startIndex, length);
                    ChapterContent_richTextBox.SelectionColor = color;
                    ChapterContent_richTextBox.ScrollToCaret();
                }
                ChapterContent_richTextBox.DeselectAll();
            }
        }

        //设置富文本-背景色、字体等
        private void SetRichTextBoxTextColor(RichTextBox richTextBox, Color color, string spk)
        {
            HighlightCurrentLine(color);

            if (spk == "1" && isSpeaking)
            {
                string lineText = richTextBox.Lines[currentLineIndex];
                if (!string.IsNullOrWhiteSpace(lineText))
                {
                    synthesizer.SpeakAsync(lineText);
                }
            }
        }

        //窗体尺寸变化触发事件
        private void ReadTXT_Resize(object? sender, EventArgs? e)
        {
            // 只有当用户点击最小化按钮时才最小化到托盘
            if (this.WindowState == FormWindowState.Minimized && !isMinimizedToTray)
            {
                // 延迟执行，避免与快捷键冲突
                Task.Delay(100).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (this.WindowState == FormWindowState.Minimized && !isMinimizedToTray)
                        {
                            MinimizeToTray();
                        }
                    }));
                });
            }
        }

        private void CreateDefaultConfig()
        {
            patternConfig = new PatternItem
            {
                TXTPath_textBoxPattern = "",
                Rule_textBoxPattern = "",
                Speed_toolStripComboBoxPattern = "0",
                ReadMode_toolStripComboBoxPattern = "整行",
                CurrentChapter_toolStripStatusLabelPattern = "",
                CurrentLine_toolStripStatusLabelPattern = "0",
                FontName = "Microsoft YaHei UI",
                FontStyle = 0,
                FontSize = 12.0,
                BlackColor = new BlackColor { R = 199, G = 237, B = 204, A = 255 },
                HotkeysForJson = new Dictionary<string, string>(DefaultHotkeys)
            };

            // 更新文本框
            UpdateHotkeyTextBoxes();

            LogStatus("已创建默认配置文件");
        }

        // 更新热键文本框显示
        private void UpdateHotkeyTextBoxes()
        {
            if (patternConfig?.Hotkeys == null)
                return;

            // 更新每个文本框
            if (patternConfig.Hotkeys.TryGetValue("ToggleMode", out string toggleModeHotkey))
                Winswitch_shortcut_toolStripTextBox.Text = toggleModeHotkey;

            if (patternConfig.Hotkeys.TryGetValue("MinimizeOrClose", out string minimizeHotkey))
                Minimize_shortcut_toolStripTextBox.Text = minimizeHotkey;

            if (patternConfig.Hotkeys.TryGetValue("ToggleTopMost", out string topMostHotkey))
                MinTop_shortcut_toolStripTextBox.Text = topMostHotkey;

            if (patternConfig.Hotkeys.TryGetValue("StartReading", out string startReadingHotkey))
                Startreading_shortcut_toolStripTextBox.Text = startReadingHotkey;

            if (patternConfig.Hotkeys.TryGetValue("StopReading", out string stopReadingHotkey))
                Pausereading_shortcut_toolStripTextBox.Text = stopReadingHotkey;

            if (patternConfig.Hotkeys.TryGetValue("NextChapter", out string nextChapterHotkey))
                Nextchapterreading_shortcut_toolStripTextBox.Text = nextChapterHotkey;

        }

        //日志信息
        public void LogStatus(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logMessage = $"{timestamp} - {message}";
            Debug.WriteLine($"{logMessage}");

            this.BeginInvoke(new Action(() =>
            {
                if (this.LogStatus_toolStripStatusLabel.Text.Length > 200)
                    this.LogStatus_toolStripStatusLabel.Text = "";
                this.LogStatus_toolStripStatusLabel.Text = logMessage;
            }));
        }

        #endregion

        #region 辅助方法-朗读相关
        //初始化朗读引擎
        private void InitializeSynthesizer()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.SpeakStarted += Synthesizer_SpeakStarted;
            synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
            synthesizer.SetOutputToDefaultAudioDevice();

            //初始化计时器
            completionTimer = new System.Windows.Forms.Timer
            {
                Interval = 10000
            };
            completionTimer.Tick += CompletionTimer_Tick;
        }

        // 开始朗读方法，从保存的行号开始
        private void StartRead()
        {
            if (int.TryParse(this.Speed_toolStripComboBox.Text, out int rate))
            {
                synthesizer.Rate = rate;
            }

            if (!isSpeaking)
            {
                isSpeaking = true;
                waitForRealCompletion = false;

                // 检查是否有暂停位置
                if (pauseLineIndex >= 0)
                {
                    currentLineIndex = pauseLineIndex;
                    isResuming = true;
                    pauseLineIndex = -1;
                    LogStatus($"从暂停位置继续：第{currentLineIndex}行");
                }
                else if (!isResuming)
                {
                    // 不是恢复状态，使用保存的行号或从头开始
                    // currentLineIndex已经在Load_set中设置了，这里不需要
                    LogStatus($"从保存位置开始：第{currentLineIndex}行");
                }

                this.LogStatus_toolStripStatusLabel.Text = "正在启动朗读...";

                Task.Delay(100).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        // 如果是恢复状态，重新高亮当前行
                        if (isResuming)
                        {
                            HighlightCurrentLine(Color.Red);
                            isResuming = false;
                        }
                        else
                        {
                            // 从头开始或从保存位置开始，先高亮当前行
                            HighlightCurrentLine(Color.Red);
                        }

                        StartReadingCurrentLine();
                    }));
                });
            }
        }

        // 从头开始朗读的方法
        private void StartReadFromBeginning()
        {
            pauseLineIndex = -1;
            isResuming = false;
            currentLineIndex = 0; // 重置为0，从头开始
            this.CurrentLine_toolStripStatusLabel.Text = "0";
            StartRead();
        }
        //开始朗读当前行
        private void StartReadingCurrentLine()
        {
            if (!isSpeaking) return;

            waitForRealCompletion = false;

            completionTimer.Stop();
            completionTimer.Start();

            waitForRealCompletion = false;

            if (currentLineIndex >= ChapterContent_richTextBox.Lines.Length)
            {
                ChapterCompleted();
                return;
            }

            string lineText = ChapterContent_richTextBox.Lines[currentLineIndex];

            if (string.IsNullOrWhiteSpace(lineText))
            {
                currentLineIndex++;
                this.CurrentLine_toolStripStatusLabel.Text = currentLineIndex.ToString();

                Task.Delay(100).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(StartReadingCurrentLine));
                });
                return;
            }

            HighlightCurrentLine(Color.Red);

            Prompt prompt = new(lineText);

            try
            {
                synthesizer.SpeakAsync(prompt);
                LogStatus($"开始朗读第{currentLineIndex + 1}行，字符数：{lineText.Length}");
            }
            catch (Exception ex)
            {
                LogStatus($"朗读出错: {ex.Message}");
                ProcessRealCompletion();
            }
        }

        // 朗读进度
        private void ProcessRealCompletion()
        {
            completionTimer.Stop();
            if (!isSpeaking) return;

            LogStatus($"完成第{currentLineIndex + 1}行的朗读");

            if (this.ReadMode_toolStripComboBox.Text == "整行")
            {
                bool isLastLine = currentLineIndex >= ChapterContent_richTextBox.Lines.Length - 1;

                if (isLastLine)
                {
                    ChapterCompleted();
                }
                else
                {
                    currentLineIndex++;
                    this.CurrentLine_toolStripStatusLabel.Text = currentLineIndex.ToString(); // 更新行号显示

                    Task.Delay(300).ContinueWith(_ =>
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            HighlightCurrentLine(Color.Red);
                            StartReadingCurrentLine();
                        }));
                    });
                }
            }
        }

        // 更新章节正文，清除富文本高亮，初始化/加载上次行号
        private void UpdateText()
        {
            if (Chapter_listBox.SelectedIndex != -1)
            {
                object selectedItem = Chapter_listBox.Items[Chapter_listBox.SelectedIndex];
                string selectedChapter = selectedItem.ToString() ?? "Default Value";
                currentChapterTitle = selectedChapter;
                if (chapters.TryGetValue(currentChapterTitle, out string value))
                {
                    this.ChapterContent_richTextBox.Text = value;

                    // 只在首次加载章节时重置行号（如果当前行号大于文本行数，则重置为0）
                    if (!isResuming)
                    {
                        // 如果当前行号大于文本行数，重置为0
                        if (currentLineIndex >= ChapterContent_richTextBox.Lines.Length)
                        {
                            currentLineIndex = 0;
                            this.CurrentLine_toolStripStatusLabel.Text = "0";
                        }
                        else
                        {
                            // 保持当前行号，只更新显示
                            this.CurrentLine_toolStripStatusLabel.Text = currentLineIndex.ToString();
                        }
                        LogStatus($"新章节开始，共{ChapterContent_richTextBox.Lines.Length}行，从第{currentLineIndex + 1}行开始");
                    }

                    // 重置文本颜色
                    ChapterContent_richTextBox.SelectAll();
                    ChapterContent_richTextBox.SelectionColor = Color.Black;
                    ChapterContent_richTextBox.DeselectAll();

                    // 如果是恢复状态，高亮当前行
                    if (isResuming && currentLineIndex > 0 && currentLineIndex < ChapterContent_richTextBox.Lines.Length)
                    {
                        HighlightCurrentLine(Color.Red);
                        LogStatus($"恢复朗读，从第{currentLineIndex + 1}行继续");
                    }
                }
            }
        }

        // 判断是否进入下一章
        private bool GoToNextChapter()
        {
            if (Chapter_listBox.SelectedIndex >= 0)
            {
                int selectedIndex = Chapter_listBox.SelectedIndex;
                if (selectedIndex < Chapter_listBox.Items.Count - 1)
                {
                    int nextItemIndex = selectedIndex + 1;
                    Chapter_listBox.SelectedIndex = nextItemIndex;
                    this.CurrentChapter_toolStripStatusLabel.Text = Chapter_listBox.Items[nextItemIndex].ToString() ?? "Default Value";
                    UpdateText();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        //根据GoToNextChapter的值，切换下一章或者全文完结束朗读。
        private void ChapterCompleted()
        {
            if (this.ReadMode_toolStripComboBox.Text == "整行")
            {
                if (GoToNextChapter())
                {
                    Task.Delay(300).ContinueWith(_ =>
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            isSpeaking = false;
                            waitForRealCompletion = false;
                            completionTimer?.Stop();
                            StartReadFromBeginning();
                        }));
                    });
                }
                else
                {
                    isSpeaking = false;
                    this.LogStatus_toolStripStatusLabel.Text = "全文朗读完成";
                }
            }
            else
            {
                isSpeaking = false;
                this.LogStatus_toolStripStatusLabel.Text = "本章朗读完成";
            }
        }

        //朗读状态
        private void Synthesizer_SpeakStarted(object? sender, SpeakStartedEventArgs? e)
        {
            waitForRealCompletion = true;
        }

        //朗读进度
        private void Synthesizer_SpeakProgress(object? sender, SpeakProgressEventArgs? e)
        {
            LogStatus($"朗读进度: '{e.Text}' (位置: {e.CharacterPosition})");
        }

        //朗读结束
        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs? e)
        {
            if (waitForRealCompletion && isSpeaking)
            {
                waitForRealCompletion = false;
                ProcessRealCompletion();
            }
            else if (!isSpeaking && pauseLineIndex >= 0)
            {
                HighlightCurrentLine(Color.Red);
            }
        }

        //朗读完成时间
        private void CompletionTimer_Tick(object? sender, EventArgs? e)
        {
            completionTimer.Stop();
            if (waitForRealCompletion && isSpeaking)
            {
                LogStatus("朗读超时，强制继续下一行");
                waitForRealCompletion = false;
                ProcessRealCompletion();
            }
        }
        //停止朗读
        private void StopRead()
        {
            if (isSpeaking && currentLineIndex < ChapterContent_richTextBox.Lines.Length)
            {
                pauseLineIndex = currentLineIndex;
                LogStatus($"已暂停，当前位置：第{pauseLineIndex}行");
            }

            isSpeaking = false;
            waitForRealCompletion = false;
            completionTimer?.Stop();

            synthesizer.SpeakAsyncCancelAll();
            this.LogStatus_toolStripStatusLabel.Text = "已暂停";
        }
        #endregion

        #region 添加热键管理相关方法
        // 提供修改热键的方法
        public bool SetHotkey(string actionName, string hotkeyString)
        {
            return hotkeyManager?.SetHotkey(actionName, hotkeyString) ?? false;
        }

        // 获取当前热键配置
        public Dictionary<string, string> GetHotkeys()
        {
            return hotkeyManager?.GetCurrentHotkeys() ?? new Dictionary<string, string>();
        }

        // 重新加载热键配置
        public void ReloadHotkeys()
        {
            // 重新加载配置文件
            string jsonPath = Path.Combine(Application.StartupPath, "mySet.json");
            if (File.Exists(jsonPath))
            {
                PatternsContainer? patterns = ReadJsonPatterns(jsonPath);
                if (patterns?.Patterns != null && patterns.Patterns.Length > 0)
                {
                    patternConfig = patterns.Patterns[0];
                    if (hotkeyManager != null)
                    {
                        hotkeyManager = new HotkeyManager(this, patternConfig);
                    }
                }
            }
        }
        #endregion

        #region 辅助方法-窗体切换
        // 显示迷你模式窗体
        public void ShowMiniMode()
        {
            try
            {
                // 如果迷你模式窗口已经存在且未关闭，则激活它
                if (miniModeForm != null && !miniModeForm.IsDisposed)
                {
                    miniModeForm.Show();
                    miniModeForm.Activate();
                    miniModeForm.Focus();
                    return;
                }
                // 创建新的迷你模式窗口
                miniModeForm = new MiniModeForm(this)
                {
                    Text = "ReadTXT - 迷你模式",
                    FormBorderStyle = FormBorderStyle.SizableToolWindow,
                    Size = new Size(400, 150),
                    StartPosition = FormStartPosition.Manual,
                    Location = new Point(
                        Screen.PrimaryScreen.WorkingArea.Width - 400,
                        Screen.PrimaryScreen.WorkingArea.Height - 150
                    ),
                    ShowInTaskbar = false,
                    TopMost = true
                };

                // 使用命名的事件处理方法，便于移除
                miniModeForm.FormClosing += MiniModeForm_FormClosing;
                miniModeForm.FormClosed += MiniModeForm_FormClosed;
                miniModeForm.Show();
                miniModeForm.Activate();
                miniModeForm.Focus();
                LogStatus("迷你模式窗口已创建并显示");
            }
            catch (Exception ex)
            {
                LogStatus($"显示迷你模式失败: {ex.Message}");
            }
        }

        //判断迷你窗体是否活动
        public bool IsMiniModeActive()
        {
            return miniModeForm != null &&
                   !miniModeForm.IsDisposed &&
                   miniModeForm.Visible &&
                   miniModeForm.WindowState != FormWindowState.Minimized;
        }
        //判断主窗体是否活动
        public bool IsMainWindowActive()
        {
            return this.Visible &&
                   this.WindowState == FormWindowState.Normal &&
                   !isMinimizedToTray;
        }


        // 添加状态修复方法
        public void FixWindowStates()
        {
            LogStatus("开始修复窗口状态...");

            bool miniActive = IsMiniModeActive();
            bool mainActive = IsMainWindowActive();

            // 如果两个窗口都激活或都未激活，进行修复
            if (miniActive && mainActive)
            {
                LogStatus("检测到两个窗口都激活，关闭迷你模式");
            }
            else if (!miniActive && !mainActive)
            {
                LogStatus("检测到两个窗口都未激活，恢复主窗体");
                RestoreFromTray();
            }
            else if (miniActive && !mainActive)
            {
                LogStatus("迷你模式激活但主窗体未激活，状态正常");
            }
            else if (!miniActive && mainActive)
            {
                LogStatus("主窗体激活但迷你模式未激活，状态正常");
            }
        }

        //从主窗体切换到迷你窗体
        public void SwitchToMiniFromMain()
        {
            try
            {
                LogStatus("开始切换到迷你模式...");

                // 第一步：显示迷你模式窗口
                ShowMiniMode();

                // 等待窗口完全显示
                Task.Delay(200).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // 第二步：最小化主窗体到托盘
                            if (this.Visible && this.WindowState != FormWindowState.Minimized)
                            {
                                MinimizeToTray();
                            }

                            // 再次激活迷你模式窗口，确保它在前台
                            if (miniModeForm != null && !miniModeForm.IsDisposed)
                            {
                                miniModeForm.Activate();
                                miniModeForm.TopMost = true;
                            }

                            LogStatus("成功切换到迷你模式");
                        }
                        catch (Exception ex)
                        {
                            LogStatus($"切换过程中出错: {ex.Message}");
                            isSwitchingMode = false;
                        }
                    }));
                });
            }
            catch (Exception ex)
            {
                LogStatus($"切换到迷你模式失败: {ex.Message}");
                isSwitchingMode = false;
            }
        }

        // 从迷你模式切换回主窗体        
        public void SwitchToMainFromMini()
        {
            try
            {
                LogStatus("开始切换回主窗体...");

                //// 确保不在切换过程中再次触发切换
                //if (isSwitchingMode) return;

                // 设置切换标志
                isSwitchingMode = true;

                // 先保存迷你窗体的引用
                var miniFormToClose = miniModeForm;

                // 恢复主窗体
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        // 确保主窗体恢复
                        if (isMinimizedToTray || !this.Visible)
                        {
                            RestoreFromTray();
                        }

                        // 激活主窗体
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                        this.Activate();
                        this.Focus();

                        LogStatus("主窗体已恢复");
                    }
                    catch (Exception ex)
                    {
                        LogStatus($"恢复主窗体失败: {ex.Message}");
                    }
                }));

                // 关闭迷你窗体（延迟执行，确保主窗体先恢复）
                Task.Delay(300).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (miniFormToClose != null && !miniFormToClose.IsDisposed)
                            {
                                // 解除迷你窗体的关闭事件，避免循环
                                miniFormToClose.FormClosing -= MiniModeForm_FormClosing;

                                // 直接关闭迷你窗体
                                miniFormToClose.Close();
                                miniFormToClose.Dispose();
                                miniModeForm = null;

                                LogStatus("迷你模式窗口已关闭");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogStatus($"关闭迷你模式窗口失败: {ex.Message}");
                        }
                        finally
                        {
                            // 重置切换标志
                            Task.Delay(300).ContinueWith(__ =>
                            {
                                this.BeginInvoke(new Action(() => { isSwitchingMode = false; }));
                            });
                        }
                    }));
                });
            }
            catch (Exception ex)
            {
                LogStatus($"切换回主窗体失败: {ex.Message}");
                isSwitchingMode = false;
            }
        }

        // 迷你窗体关闭中事件
        public void MiniModeForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                if (sender is MiniModeForm miniForm)
                {
                    // 如果是用户点击关闭按钮，切换到主窗体
                    if (e.CloseReason == CloseReason.UserClosing && !isSwitchingMode)
                    {
                        LogStatus("用户关闭迷你模式，切换到主窗体");

                        // 取消默认关闭行为
                        e.Cancel = true;

                        // 切换到主窗体
                        Task.Run(() => ToggleMainMiniMode());
                    }
                    else if (isSwitchingMode)
                    {
                        // 切换过程中，允许关闭
                        LogStatus("切换过程中关闭迷你窗口");
                    }
                }
            }
            catch (Exception ex)
            {
                LogStatus($"迷你模式关闭事件处理失败: {ex.Message}");
            }
        }

        // 迷你窗体已关闭事件
        public void MiniModeForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            try
            {
                if (sender is MiniModeForm miniForm)
                {
                    // 清理事件绑定
                    miniForm.FormClosing -= MiniModeForm_FormClosing;
                    miniForm.FormClosed -= MiniModeForm_FormClosed;

                    // 清理引用
                    if (miniModeForm == miniForm)
                    {
                        miniModeForm = null;
                    }

                    LogStatus("迷你模式窗口已完全关闭");
                }
            }
            catch (Exception ex)
            {
                LogStatus($"迷你模式关闭后清理失败: {ex.Message}");
            }
        }

        // 切换方法
        private void ToggleMainMiniMode()
        {
            if (isSwitchingMode) return;

            isSwitchingMode = true;

            try
            {
                // 更准确的状态检查
                bool miniModeActive = miniModeForm != null &&
                                      !miniModeForm.IsDisposed &&
                                      miniModeForm.Visible;

                bool mainWindowActive = this.Visible &&
                                       this.WindowState != FormWindowState.Minimized &&
                                       !isMinimizedToTray;

                LogStatus($"切换检查: 迷你模式={miniModeActive}, 主窗口={mainWindowActive}, 托盘={isMinimizedToTray}");

                if (miniModeActive && !mainWindowActive)
                {
                    // 从迷你模式切换回主窗体
                    LogStatus("正在从迷你模式切换到主窗体...");
                    SwitchToMainFromMini();
                }
                else if (mainWindowActive && !miniModeActive)
                {
                    // 从主窗体切换到迷你模式
                    LogStatus("正在从主窗体切换到迷你模式...");
                    SwitchToMiniFromMain();
                }
                else if (isMinimizedToTray)
                {
                    // 如果只在托盘中，恢复主窗体
                    LogStatus("从托盘恢复主窗体...");
                    RestoreFromTray();
                }
                else
                {
                    // 状态异常，进行修复
                    LogStatus("检测到异常状态，进行修复...");
                    FixWindowStates();
                }
            }
            finally
            {
                // 延迟重置标志，确保切换完成
                Task.Delay(500).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        isSwitchingMode = false;
                        LogStatus("切换过程完成");
                    }));
                });
            }
        }

        // 主窗体最小化到托盘
        public void MinimizeToTray()
        {
            try
            {
                if (this.Visible && this.WindowState != FormWindowState.Minimized)
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    isMinimizedToTray = true;
                    LogStatus("主窗体已最小化到托盘");
                }
            }
            catch (Exception ex)
            {
                LogStatus($"最小化到托盘失败: {ex.Message}");
            }
        }
        ////迷你窗体关闭
        //public void CloseMiniForm()
        //{
        //    this.BeginInvoke(new Action(() =>
        //    {
        //        try
        //        {
        //            if (this.miniModeForm != null && !this.miniModeForm.IsDisposed)
        //            {
        //                LogStatus("正在关闭迷你模式窗口...");

        //                // 停止迷你窗体的定时器
        //                if (this.miniModeForm is MiniModeForm miniForm)
        //                {
        //                    miniForm.StopTimer();
        //                    miniForm.AllowClose();
        //                }

        //                // 解除事件绑定
        //                this.miniModeForm.FormClosing -= MiniModeForm_FormClosing;
        //                this.miniModeForm.FormClosed -= MiniModeForm_FormClosed;

        //                // 关闭窗体
        //                this.miniModeForm.Close();

        //                // 清理资源
        //                this.miniModeForm.Dispose();
        //                this.miniModeForm = null;

        //                LogStatus("迷你模式窗口已关闭");
        //            }
        //        }
        //        catch (ObjectDisposedException)
        //        {
        //            // 忽略已释放对象的异常
        //            LogStatus("迷你模式窗口已释放");
        //            miniModeForm = null;
        //        }
        //        catch (Exception ex)
        //        {
        //            LogStatus($"关闭迷你模式窗口失败: {ex.Message}");
        //            // 强制清理引用
        //            miniModeForm = null;
        //        }
        //    }));
        //}
        //// 在更新迷你窗体状态的地方添加检查
        //public void UpdateMiniModeStatus()
        //{
        //    try
        //    {
        //        if (miniModeForm != null && !miniModeForm.IsDisposed && miniModeForm.Visible)
        //        {
        //            // 更新迷你窗体的状态
        //            // ...
        //        }
        //    }
        //    catch (ObjectDisposedException)
        //    {
        //        // 如果对象已释放，清理引用
        //        miniModeForm = null;
        //    }
        //}
        //public void QuickCloseMiniForm()
        //{
        //    this.BeginInvoke(new Action(() =>
        //    {
        //        if (miniModeForm != null && !miniModeForm.IsDisposed)
        //        {
        //            try
        //            {
        //                // 直接强制关闭，不处理事件
        //                miniModeForm.FormClosing -= MiniModeForm_FormClosing;
        //                miniModeForm.FormClosed -= MiniModeForm_FormClosed;

        //                // 使用更强制的方式关闭
        //                miniModeForm.Dispose();
        //                miniModeForm = null;

        //                LogStatus("迷你模式窗口已强制关闭");
        //            }
        //            catch (Exception ex)
        //            {
        //                LogStatus($"强制关闭迷你模式窗口失败: {ex.Message}");
        //                miniModeForm = null;
        //            }
        //        }
        //    }));
        //}
        // 主窗体恢复可见
        public void RestoreFromTray()
        {
            try
            {
                if (isMinimizedToTray || !this.Visible)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                    isMinimizedToTray = false;
                    LogStatus("主窗体已从托盘恢复");
                }
            }
            catch (Exception ex)
            {
                LogStatus($"从托盘恢复失败: {ex.Message}");
            }
        }

        #endregion

        #region 传递状态给迷你窗口

        // 公开的获取朗读状态的方法，供迷你模式窗口使用        
        public bool GetSpeakingStatus() => isSpeaking;

        // 公开的获取当前章节的方法，供迷你模式窗口使用
        public string GetCurrentChapter() => CurrentChapter_toolStripStatusLabel.Text;

        // 公开的获取当前朗读文本的方法，供迷你模式窗口使用
        public string GetCurrentReadingText()
        {
            if (currentLineIndex >= 0 && currentLineIndex < ChapterContent_richTextBox.Lines.Length)
            {
                string lineText = ChapterContent_richTextBox.Lines[currentLineIndex];
                if (lineText.Length > 50)
                {
                    return lineText.Substring(0, 47) + "...";
                }
                return lineText;
            }
            return "等待开始...";
        }

        // 公开的开始朗读方法，供迷你模式窗口使用
        public void StartReadingFromMini() => StartRead();

        // 公开的暂停朗读方法，供迷你模式窗口使用
        public void StopReadingFromMini() => StopRead();

        // 公开的下一章方法，供迷你模式窗口使用
        public void NextChapterFromMini() => NextChapter_toolStripButton_Click(null, null);

        // 切换迷你模式的置顶状态
        public void ToggleMiniModeTopMost()
        {
            if (miniModeForm != null && !miniModeForm.IsDisposed)
            {
                miniModeForm.ToggleTopMost();
                LogStatus($"迷你模式窗口已{(miniModeForm.IsTopMost() ? "置顶" : "取消置顶")}");
            }
            else
            {
                LogStatus("迷你模式窗口未打开");
            }
        }
        #endregion

        #region 快捷键设置相关方法

        // 为每个菜单项添加点击事件处理程序
        private void 窗体切换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHotkeyInputDialog("ToggleMode", "窗体切换", Winswitch_shortcut_toolStripTextBox);
        }

        private void 最小化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHotkeyInputDialog("MinimizeOrClose", "最小化", Minimize_shortcut_toolStripTextBox);
        }

        private void 迷你窗体置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHotkeyInputDialog("ToggleTopMost", "迷你窗体置顶", MinTop_shortcut_toolStripTextBox);
        }

        private void 开始朗读ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHotkeyInputDialog("StartReading", "开始朗读", Startreading_shortcut_toolStripTextBox);
        }

        private void 暂停朗读ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHotkeyInputDialog("StopReading", "暂停朗读", Pausereading_shortcut_toolStripTextBox);
        }

        private void 下一章ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHotkeyInputDialog("NextChapter", "下一章", Nextchapterreading_shortcut_toolStripTextBox);
        }

        private void 重置默认ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 显示确认对话框
                DialogResult result = MessageBox.Show(
                    "确定要将所有快捷键重置为默认值吗？",
                    "重置快捷键",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ResetAllHotkeysToDefault();
                    LogStatus("快捷键已重置为默认值");
                }
            }
            catch (Exception ex)
            {
                LogStatus($"重置快捷键时出错: {ex.Message}");
                MessageBox.Show($"重置快捷键时出错: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        // 重置所有快捷键为默认值
        private void ResetAllHotkeysToDefault()
        {
            // 1. 更新各个文本框
            if (DefaultHotkeys.TryGetValue("ToggleMode", out string toggleModeHotkey))
                Winswitch_shortcut_toolStripTextBox.Text = toggleModeHotkey;

            if (DefaultHotkeys.TryGetValue("MinimizeOrClose", out string minimizeHotkey))
                Minimize_shortcut_toolStripTextBox.Text = minimizeHotkey;

            if (DefaultHotkeys.TryGetValue("ToggleTopMost", out string topMostHotkey))
                MinTop_shortcut_toolStripTextBox.Text = topMostHotkey;

            if (DefaultHotkeys.TryGetValue("StartReading", out string startReadingHotkey))
                Startreading_shortcut_toolStripTextBox.Text = startReadingHotkey;

            if (DefaultHotkeys.TryGetValue("StopReading", out string stopReadingHotkey))
                Pausereading_shortcut_toolStripTextBox.Text = stopReadingHotkey;

            if (DefaultHotkeys.TryGetValue("NextChapter", out string nextChapterHotkey))
                Nextchapterreading_shortcut_toolStripTextBox.Text = nextChapterHotkey;

            if (DefaultHotkeys.TryGetValue("SaveDocument", out string saveDocumentHotkey))
            {
                // 如果有保存文档的热键文本框
                // Save_shortcut_toolStripTextBox.Text = saveDocumentHotkey;
            }

            // 2. 更新热键管理器
            if (hotkeyManager != null)
            {
                foreach (var hotkey in DefaultHotkeys)
                {
                    hotkeyManager.SetHotkey(hotkey.Key, hotkey.Value);
                }
            }

            // 3. 更新patternConfig
            if (patternConfig != null)
            {
                if (patternConfig.HotkeysForJson == null)
                {
                    patternConfig.HotkeysForJson = new Dictionary<string, string>();
                }

                foreach (var hotkey in DefaultHotkeys)
                {
                    patternConfig.HotkeysForJson[hotkey.Key] = hotkey.Value;
                }
            }
        }

        // 通用的显示热键输入对话框方法
        private void ShowHotkeyInputDialog(string actionName, string actionDescription, ToolStripTextBox targetTextBox)
        {
            try
            {
                // 获取当前的热键设置
                string currentHotkey = targetTextBox.Text;

                // 显示热键输入对话框
                using (HotkeyInputForm form = new HotkeyInputForm(currentHotkey))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string newHotkey = form.HotkeyString;

                        // 更新文本框
                        targetTextBox.Text = newHotkey;

                        // 更新热键管理器和配置
                        UpdateHotkeyConfiguration(actionName, newHotkey, actionDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                LogStatus($"设置快捷键时出错: {ex.Message}");
                MessageBox.Show($"设置快捷键时出错: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 更新配置方法
        private void UpdateHotkeyConfiguration(string actionName, string hotkeyString, string actionDescription)
        {
            if (hotkeyManager != null)
            {
                bool success = hotkeyManager.SetHotkey(actionName, hotkeyString);
                if (success)
                {
                    // 更新patternConfig
                    if (patternConfig != null)
                    {
                        if (patternConfig.HotkeysForJson == null)
                            patternConfig.HotkeysForJson = new Dictionary<string, string>();

                        patternConfig.HotkeysForJson[actionName] = hotkeyString;
                    }

                    LogStatus($"{actionDescription}快捷键已设置为: {hotkeyString}");
                }
                else
                {
                    LogStatus($"设置{actionDescription}快捷键失败");
                }
            }
            else
            {
                LogStatus("热键管理器未初始化");
            }
        }

        #endregion

    }
}