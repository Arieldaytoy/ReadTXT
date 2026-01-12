using System.Diagnostics;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json; // 或者使用 Newtonsoft.Json
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;



namespace ReadTXT
{
    public partial class ReadTXT : Form
    {
        private readonly SpeechSynthesizer synthesizer = new();
        private bool isSpeaking = false; // 用于跟踪是否正在朗读
        Dictionary<string, string> chapters = []; // 确保在循环前初始化字典       
        private string currentChapterTitle; // 当前正在编辑的章节标题
        private string oldChapter = "";

        private int currentLineIndex = 0; // 添加当前行索引字段
        private readonly bool isAutoLineMode = false;
        private bool waitForRealCompletion = false; // 新增：等待真实完成的标志
        private readonly System.Windows.Forms.Timer completionTimer;

        private int pauseLineIndex = -1; // 新增：记录暂停时的行位置
        private bool isResuming = false; // 新增：是否正在恢复朗读

        public class BlackColor
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte A { get; set; }
            // 其他属性可能不需要，除非您有特定的用途
            [JsonIgnore] // 如果您不希望这个属性被序列化回 JSON，可以使用 JsonIgnore
            public Color Color => Color.FromArgb(A, R, G, B);
        }
        // 定义与JSON结构相匹配的类
        public class PatternItem
        {
            public string? textBox1Pattern { get; set; }
            public string? textBox2Pattern { get; set; }
            public string? toolStripComboBox1Pattern { get; set; }
            public string? toolStripComboBox2Pattern { get; set; }
            public string? toolStripStatusLabel4Pattern { get; set; }
            public string? FontName { get; set; }
            public int FontStyle { get; set; }
            public double FontSize { get; set; }
            public required BlackColor BlackColor { get; set; }

        }
        // 添加一个方法来读取JSON文件
        public class PatternsContainer
        {
            public required PatternItem[] Patterns { get; set; }
        }
        private static PatternsContainer? ReadJsonPatterns(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<PatternsContainer>(jsonContent);
                // 如果使用Json.NET，则使用 JsonConvert.DeserializeObject<PatternsContainer>(jsonContent)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取JSON文件时出错: {ex.Message}");
                return null;
            }
        }
        // 静态的 JsonSerializerOptions 实例，用于缓存和重用
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true
        };
        // 公共方法，用于序列化对象
        public static string SerializeObject<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _serializerOptions);
        }

        public ReadTXT()
        {
            InitializeComponent();
            this.textBox1.AllowDrop = true;
            this.textBox1.DragEnter += textBox1_DragEnter;
            this.textBox1.DragDrop += textBox1_DragDrop;
            synthesizer = new SpeechSynthesizer();
            // 正确的事件订阅顺序
            synthesizer.SpeakStarted += Synthesizer_SpeakStarted;
            synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;

            // 设置音频输出设备
            synthesizer.SetOutputToDefaultAudioDevice();

            // 初始化 completionTimer
            completionTimer = new System.Windows.Forms.Timer
            {
                Interval = 10000 // 10秒超时
            };
            completionTimer.Tick += CompletionTimer_Tick; // 关联Tick事件
        }
        private void Synthesizer_SpeakStarted(object? sender, SpeakStartedEventArgs? e)
        {
            //LogStatus("朗读真正开始");
            waitForRealCompletion = true; // 标记开始等待真实完成
        }
        private void Synthesizer_SpeakProgress(object? sender, SpeakProgressEventArgs? e)
        {
            LogStatus($"朗读进度: '{e.Text}' (位置: {e.CharacterPosition})");
        }

        // Timer的Tick事件处理方法
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

        private void ReadTXT_Load(object sender, EventArgs e)
        {
            LoadTTSVoices();
            Load_set();//读取上次的设置
            Analyze_novel();//解析小说
            SetListBoxSelectedItemByToolStripLabelText();//定位到上次章节
            // 初始化时设置文本颜色
            SetRichTextBoxTextColor(richTextBox1, Color.Black, "0");
            //读取朗读速度，如果速度不合规设置为默认值=0
            if (int.TryParse(this.toolStripComboBox1.Text, out int rate))
            {
                synthesizer.Rate = rate;
            }
            else
            {
                // 处理转换失败的情况，例如使用默认语速
                this.toolStripComboBox1.Text = "0";
                synthesizer.Rate = 0; // 或者设置为其他你认为合适的默认值
                this.toolStripStatusLabel2.Text = "朗读速度只能设置-10~10的整数！！！";
                this.toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }
        //加载已安装语音包
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

        //定位到上次阅读章节
        private void SetListBoxSelectedItemByToolStripLabelText()
        {
            string ck = "0";
            if (oldChapter != "")
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == oldChapter)
                    {
                        listBox1.SelectedIndex = i;
                        ck = "1";
                        break; // 找到匹配项后退出循环
                    }
                }
                //如果没有找到上次的章节
                if (ck == "0") { this.toolStripStatusLabel2.Text = "你可能换了一本小说，找不到上次看的那章>_<"; }
            }
        }


        //DragEnter事件处理程序
        private void textBox1_DragEnter(object? sender, DragEventArgs e)
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
        // DragDrop事件处理程序
        private void textBox1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
                {
                    this.textBox1.Text = files[0];
                }
            }
        }

        //点击解析章节和内容
        private void button1_Click(object sender, EventArgs e)
        {
            Analyze_novel();
        }
        private void Analyze_novel()
        {
            this.toolStripStatusLabel2.Text = "";//当前状态
            this.toolStripStatusLabel4.Text="";//当前章节
            this.toolStripStatusLabel6.Text ="0";//当前行 
            this.listBox1.Items.Clear();//目录
            currentChapterTitle="";//目录
            this.richTextBox1.Text = "";//正文
            chapters= [];//正文            
            string filePath = this.textBox1.Text;// 指定TXT文件的路径
            if (filePath!="")
            {
                try
                {
                    LoadFileContent(filePath); // 加载文件内容
                    PopulateListBox(); // 填充ListBox
                    listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
                    if (this.listBox1.Items.Count > 0)
                    {
                        if (this.toolStripStatusLabel4.Text == "")
                        {
                            this.toolStripStatusLabel2.Text = "解析完成，开始阅读吧~";
                        }
                        else
                        {
                            this.toolStripStatusLabel2.Text = "已定位到上次的章节，继续阅读吧~";
                        }
                        this.toolStripStatusLabel2.ForeColor = Color.Black;
                    }
                    else
                    {
                        this.toolStripStatusLabel2.Text = "未能解析到相关章节！请确认文件中有与规则匹配的章节标题。";
                        this.toolStripStatusLabel2.ForeColor = Color.Red;
                    }

                }
                catch (Exception ex)
                {
                    this.toolStripStatusLabel2.Text = "读取文件时出错: " + ex.Message;
                    this.toolStripStatusLabel2.ForeColor = Color.Red;
                }
            }
        }
        //加载章节和对应的正文
        private void LoadFileContent(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (this.textBox2.Text != "")
            {
                for (int j = 0; j < lines.Length;)
                {
                    if (Regex.IsMatch(lines[j], this.textBox2.Text))
                    {
                        string chapterTitle = lines[j];
                        string chapterContent = "";
                        j++;
                        int contentIndex = j;
                        while (contentIndex < lines.Length && !Regex.IsMatch(lines[contentIndex], this.textBox2.Text))
                        {
                            chapterContent += lines[contentIndex] + Environment.NewLine;
                            contentIndex++;
                        }
                        chapters[chapterTitle] =chapterTitle+ Environment.NewLine+ chapterContent;
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
                richTextBox1.Text = "";
                richTextBox1.Text = string.Join(Environment.NewLine, lines);
            }
        }
        //将章节添加到目录
        private void PopulateListBox()
        {
            if (chapters != null)
            {
                listBox1.Items.Clear();
                foreach (var chapter in chapters.Keys)
                {
                    listBox1.Items.Add(chapter);
                }
            }
            else
            {
                this.toolStripStatusLabel2.Text = "未能匹配到相关章节！建议查看原始文档中章节标题是否与章节规则匹配！！！";
                this.toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }

        private void ListBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.toolStripStatusLabel4.Text != null)
            {
                chapters[this.toolStripStatusLabel4.Text] = this.richTextBox1.Text;
            }
            this.toolStripStatusLabel4.Text = listBox1.Items[listBox1.SelectedIndex].ToString() ?? "Default Value";
            UpdateText();

            // 重置暂停位置（切换到新章节时）
            pauseLineIndex = -1;
            isResuming = false;

            // 重置滚动条到最开始
            richTextBox1.Select(0, 0);
            richTextBox1.ScrollToCaret();
        }
        //点击设置：加载上次的阅读状态
        private void button2_Click(object sender, EventArgs e)
        {
            Load_set();
            if (oldChapter != "")
            {
                SetListBoxSelectedItemByToolStripLabelText();//定位到上次章节
                UpdateText();
            }
        }

        //保存设置：保存当前阅读设置
        private void button3_Click(object sender, EventArgs e)
        {
            // 定义要序列化为JSON的对象
            var patterns = new
            {
                Patterns = new[]
                {
                    new
                    {
                        textBox1Pattern = this.textBox1.Text,
                        textBox2Pattern = this.textBox2.Text,
                        toolStripComboBox1Pattern =this. toolStripComboBox1.Text,
                        toolStripComboBox2Pattern =this. toolStripComboBox2.Text,
                        toolStripStatusLabel4Pattern =this.toolStripStatusLabel4.Text,
                        FontName=this.richTextBox1.Font.Name,
                        FontStyle=this.richTextBox1.Font.Style,
                        FontSize=this.richTextBox1.Font.Size,
                        BlackColor=this.richTextBox1.BackColor
                    }
                }
            };
            // 获取运行目录
            string runningDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // 构建JSON文件路径
            string jsonFilePath = Path.Combine(runningDirectory, "mySet.json");
            // 将对象序列化为JSON字符串
            string jsonString = SerializeObject(patterns);
            // 将JSON字符串写入文件
            File.WriteAllText(jsonFilePath, jsonString);
        }

        private void Load_set()
        {
            string jsonPath = Path.Combine(Application.StartupPath, "mySet.json");
            bool fileExists = File.Exists(jsonPath);
            if (fileExists)
            {
                // 处理JSON文件的逻辑
                PatternsContainer? patterns = ReadJsonPatterns(jsonPath);
                if (patterns?.Patterns != null && patterns.Patterns.Length > 0)
                {
                    PatternItem firstPattern = patterns.Patterns[0];
                    this.textBox1.Text = firstPattern.textBox1Pattern;//上次阅读的小说路径
                    this.textBox2.Text = firstPattern.textBox2Pattern;//上次阅读的小说章节规则
                    this.toolStripComboBox1.Text = firstPattern.toolStripComboBox1Pattern;//语速
                    this.toolStripComboBox2.Text = firstPattern.toolStripComboBox2Pattern;//模式
                    this.toolStripStatusLabel4.Text = firstPattern.toolStripStatusLabel4Pattern;//上次阅读到的章节
                    oldChapter=firstPattern.toolStripStatusLabel4Pattern??"";
                    // 创建新的 Font 对象
                    FontStyle fontStyle = (FontStyle)firstPattern.FontStyle;
                    Font newFont = new(
                        familyName: firstPattern.FontName ?? "Microsoft YaHei UI",
                        (float)firstPattern.FontSize,
                        fontStyle
                    );
                    // 设置 RichTextBox 的字体
                    this.richTextBox1.Font = newFont;
                    // 设置正文背景色
                    this.richTextBox1.BackColor = firstPattern.BlackColor.Color;
                    this.listBox1.BackColor = firstPattern.BlackColor.Color;
                }
                else
                {
                    this.toolStripStatusLabel2.Text = "JSON文件中没有有效的规则设置。";
                    this.toolStripStatusLabel2.ForeColor = Color.Red;
                }
            }
            else
            {
                // 文件不存在，可以执行创建文件或其他操作
                this.toolStripStatusLabel2.Text = "文件不存在: " + jsonPath;
                this.toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }

        //朗读操作
        //暂停
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            StopRead();
        }
        //开始
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            StopRead();
            StartRead();
        }
        //下一章
        private void toolStripButton3_Click(object? sender, EventArgs? e)
        {
            StopRead();
            GoToNextChapter();
            StartReadFromBeginning();
        }
        // 从头开始朗读的方法
        private void StartReadFromBeginning()
        {
            pauseLineIndex = -1;
            isResuming = false;
            currentLineIndex = 0;
            this.toolStripStatusLabel6.Text = "0";
            StartRead();
        }
        //开始朗读
        private void StartRead()
        {
            if (int.TryParse(this.toolStripComboBox1.Text, out int rate))
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
                    pauseLineIndex = -1; // 重置暂停位置
                    LogStatus($"从暂停位置继续：第{currentLineIndex}行");
                }
                else if (!isResuming)
                {
                    // 不是恢复状态，从头开始
                    currentLineIndex = 0;
                    this.toolStripStatusLabel6.Text = "0";
                }

                this.toolStripStatusLabel2.Text = "正在启动朗读...";

                // 短暂延迟后开始，确保事件监听器就绪
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
                        StartReadingCurrentLine();
                    }));
                });
            }
        }

        //停止朗读
        private void StopRead()
        {
            // 记录暂停位置
            if (isSpeaking && currentLineIndex < richTextBox1.Lines.Length)
            {
                pauseLineIndex = currentLineIndex;
                LogStatus($"已暂停，当前位置：第{pauseLineIndex}行");
            }

            isSpeaking = false;
            waitForRealCompletion = false;
            completionTimer?.Stop();

            synthesizer.SpeakAsyncCancelAll();
            this.toolStripStatusLabel2.Text = "已暂停";
        }

        //开始朗读当前行
        private void StartReadingCurrentLine()
        {
            if (!isSpeaking) return;

            // 重置状态
            waitForRealCompletion = false;

            // 启动超时计时器
            completionTimer.Stop();
            completionTimer.Start();

            // 重置完成等待标志
            waitForRealCompletion = false;

            if (currentLineIndex >= richTextBox1.Lines.Length)
            {
                ChapterCompleted();
                return;
            }

            // 获取逻辑行的完整文本（注意：这是按Enter键分隔的行，不是显示的行）
            string lineText = richTextBox1.Lines[currentLineIndex];

            if (string.IsNullOrWhiteSpace(lineText))
            {
                // 空行处理
                currentLineIndex++;
                this.toolStripStatusLabel6.Text = currentLineIndex.ToString();

                // 短暂延迟后处理下一行
                Task.Delay(100).ContinueWith(_ =>
                {
                    this.BeginInvoke(new Action(StartReadingCurrentLine));
                });
                return;
            }

            // 高亮整个逻辑行
            HighlightCurrentLine(Color.Red);

            // 使用Prompt对象而不是直接传字符串，提高稳定性
            Prompt prompt = new(lineText);

            try
            {
                synthesizer.SpeakAsync(prompt);
                LogStatus($"开始朗读第{currentLineIndex + 1}行，字符数：{lineText.Length}");
            }
            catch (Exception ex)
            {
                LogStatus($"朗读出错: {ex.Message}");
                // 出错时也继续下一行，避免卡死
                ProcessRealCompletion();
            }
        }


        // 高亮当前行的方法
        private void HighlightCurrentLine(Color color)
        {
            if (currentLineIndex < richTextBox1.Lines.Length)
            {
                // 获取当前逻辑行的文本
                string currentLineText = richTextBox1.Lines[currentLineIndex];

                // 如果当前行是空行，不进行高亮
                if (string.IsNullOrWhiteSpace(currentLineText))
                {
                    return;
                }

                // 找到逻辑行的起始位置
                int startIndex = 0;
                for (int i = 0; i < currentLineIndex; i++)
                {
                    startIndex += richTextBox1.Lines[i].Length + 1; // +1 是为了换行符
                }

                // 逻辑行的长度（不包括换行符）
                int length = currentLineText.Length;

                // 清除之前的高亮（将所有文本设为黑色）
                richTextBox1.SelectAll();
                richTextBox1.SelectionColor = Color.Black;

                // 高亮整个逻辑行（无论它显示为多少行）
                if (startIndex >= 0 && startIndex + length <= richTextBox1.Text.Length)
                {
                    richTextBox1.Select(startIndex, length);
                    richTextBox1.SelectionColor = color;

                    // 滚动到高亮行的起始位置
                    richTextBox1.ScrollToCaret();
                }

                richTextBox1.DeselectAll();
            }
        }

        // 关键修复：正确的完成事件处理
        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs? e)
        {
            //LogStatus("SpeakCompleted事件触发");

            // 只有当我们真正开始朗读后才处理完成事件
            if (waitForRealCompletion && isSpeaking)
            {
                waitForRealCompletion = false;
                ProcessRealCompletion();
            }
            else if (!isSpeaking && pauseLineIndex >= 0)
            {
                // 如果是暂停状态，保持当前高亮
                HighlightCurrentLine(Color.Red);
            }
        }

        // 修改ProcessRealCompletion方法，确保正确计数
        private void ProcessRealCompletion()
        {
            completionTimer.Stop(); // 停止超时计时器
            if (!isSpeaking) return;

            LogStatus($"完成第{currentLineIndex + 1}行的朗读");

            if (this.toolStripComboBox2.Text == "整行")
            {
                // 整行模式：处理下一行
                currentLineIndex++;
                this.toolStripStatusLabel6.Text = currentLineIndex.ToString();

                if (currentLineIndex < richTextBox1.Lines.Length)
                {
                    // 短暂延迟后继续下一行
                    Task.Delay(300).ContinueWith(_ =>
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            // 记录下一行的信息（调试用）
                            LogCurrentLineInfo();
                            StartReadingCurrentLine();
                        }));
                    });
                }
                else
                {
                    // 章节结束
                    ChapterCompleted();
                }
            }
        }
        private void LogCurrentLineInfo()
        {
            if (currentLineIndex < richTextBox1.Lines.Length)
            {
                string lineText = richTextBox1.Lines[currentLineIndex];
                var (startIndex, length) = GetLogicalLinePosition(currentLineIndex);

                LogStatus($"行 {currentLineIndex + 1}: 起始位置={startIndex}, 长度={length}, 文本长度={lineText.Length}");
                LogStatus($"文本预览: {(lineText.Length > 50 ? lineText.Substring(0, 50) + "..." : lineText)}");
            }
        }
        // 切换到下一章
        private void GoToNextChapter()
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int selectedIndex = listBox1.SelectedIndex;
                if (selectedIndex < listBox1.Items.Count - 1)
                {
                    int nextItemIndex = selectedIndex + 1;
                    listBox1.SelectedIndex = nextItemIndex;
                    this.toolStripStatusLabel4.Text = listBox1.Items[nextItemIndex].ToString() ?? "Default Value";
                    UpdateText();
                }
                else
                {
                    StopRead();
                    this.toolStripStatusLabel2.Text = "全文完。";
                    this.toolStripStatusLabel2.ForeColor = Color.Black;
                }
            }
        }



        // 章节完成处理
        private void ChapterCompleted()
        {
            if (isAutoLineMode && isSpeaking)
            {
                // 自动进入下一章
                GoToNextChapter();
                if (isSpeaking)
                {
                    // 短暂延迟后开始新章节
                    Task.Delay(500).ContinueWith(_ =>
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            currentLineIndex = 0;
                            this.toolStripStatusLabel6.Text = "0";
                            StartReadingCurrentLine();
                        }));
                    });
                }
            }
            else
            {
                isSpeaking = false;
                this.toolStripStatusLabel2.Text = "朗读完成";
            }
        }

        // 修改UpdateText方法
        private void UpdateText()
        {
            if (listBox1.SelectedIndex != -1)
            {
                object selectedItem = listBox1.Items[listBox1.SelectedIndex];
                string selectedChapter = selectedItem.ToString() ?? "Default Value";
                currentChapterTitle = selectedChapter;
                if (chapters.TryGetValue(currentChapterTitle, out string value))
                {
                    this.richTextBox1.Text = value;

                    // 只在首次加载章节时重置行号
                    if (!isResuming)
                    {
                        currentLineIndex = 0;
                        this.toolStripStatusLabel6.Text = "0";
                        LogStatus($"新章节开始，共{richTextBox1.Lines.Length}行");
                    }

                    // 重置文本颜色
                    richTextBox1.SelectAll();
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.DeselectAll();

                    // 如果是恢复状态，高亮当前行
                    if (isResuming && currentLineIndex > 0 && currentLineIndex < richTextBox1.Lines.Length)
                    {
                        HighlightCurrentLine(Color.Red);
                        LogStatus($"恢复朗读，从第{currentLineIndex + 1}行继续");
                    }
                }
            }
        }

        // 添加一个方法来计算逻辑行的位置和长度
        private (int startIndex, int length) GetLogicalLinePosition(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= richTextBox1.Lines.Length)
                return (0, 0);

            int startIndex = 0;
            for (int i = 0; i < lineIndex; i++)
            {
                startIndex += richTextBox1.Lines[i].Length + 1; // +1 是为了换行符
            }

            int length = richTextBox1.Lines[lineIndex].Length;
            return (startIndex, length);
        }


        // 设置富文本朗读时的颜色
        private void SetRichTextBoxTextColor(RichTextBox richTextBox, Color color, string spk)
        {
            HighlightCurrentLine(color);

            if (spk == "1" && isSpeaking)
            {
                // 获取当前逻辑行的文本
                string lineText = richTextBox.Lines[currentLineIndex];
                if (!string.IsNullOrWhiteSpace(lineText))
                {
                    synthesizer.SpeakAsync(lineText);
                }
            }
        }
        //修改字体
        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 显示字体对话框
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的字体
                Font selectedFont = fontDialog1.Font;
                richTextBox1.Font = selectedFont;
            }
        }
        //修改背景色
        private void 背景色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 如果用户点击了确定                                                                        
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的颜色
                Color selectedColor = colorDialog1.Color;
                // 应用颜色到RichTextBox的背景
                this.richTextBox1.BackColor = selectedColor;
                this.listBox1.BackColor = selectedColor;
            }
        }
        //保存
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //原来的文件路径
            string originalFilePath = this.textBox1.Text;
            string directory = Path.GetDirectoryName(originalFilePath);
            string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = Path.GetExtension(originalFilePath);
            // 构建新的文件名
            string newFileName = $"{fileName}{extension}";
            // 获取当前时间，并格式化
            DateTime now = DateTime.Now;
            if (toolStripComboBox3.Text=="另存带日期戳")
            {
                string timestamp = now.ToString("yyyyMMdd");
                newFileName = $"{fileName}-{timestamp}{extension}";
            }
            else if (toolStripComboBox3.Text=="另存带时间戳")
            {
                string timestamp = now.ToString("yyyyMMddHHmmss");
                newFileName = $"{fileName}-{timestamp}{extension}";
            }
            string newFilePath = Path.Combine(directory, newFileName);
            Encoding selectedEncoding = this.comboBox1.SelectedItem.ToString() switch
            {
                "UTF-8" => Encoding.UTF8,
                "UTF-8 BOM" => new UTF8Encoding(true),// 包含 BOM 的 UTF-8 编码
                "ANSI" => Encoding.Default,// ANSI
                _ => Encoding.UTF8,
            };
            if (!string.IsNullOrEmpty(currentChapterTitle))
            {
                chapters[currentChapterTitle] = this.richTextBox1.Text;
                using StreamWriter writer = new(newFilePath, false, selectedEncoding);
                foreach (var chapter in chapters)
                {
                    writer.Write(chapter.Value); // 写入章节内容
                    writer.WriteLine(); // 在每个章节内容后添加一个空行作为分隔
                }
            }
            else
            {
                using StreamWriter writer = new(newFilePath, false, selectedEncoding);
                writer.Write(this.richTextBox1.Text); // 写入章节内容
            }
            this.toolStripStatusLabel2.Text = "保存成功，路径："+ newFilePath;
            this.toolStripStatusLabel2.ForeColor = Color.Black;
        }

        // 添加日志输出
        private void LogStatus(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logMessage = $"{timestamp} - {message}";
            Debug.WriteLine($"{logMessage}");

            // 可选：在界面显示最后几条日志
            this.BeginInvoke(new Action(() =>
            {
                if (this.toolStripStatusLabel2.Text.Length > 200)
                    this.toolStripStatusLabel2.Text = "";
                //this.toolStripStatusLabel2.Text += logMessage + Environment.NewLine;
                this.toolStripStatusLabel2.Text = logMessage;
            }));
        }


        // 确保在表单关闭时释放资源
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            synthesizer.Dispose();
            completionTimer?.Dispose(); // 添加这行
        }

    }
}