using System.Speech.Synthesis;
using System.Text;
using System.Text.Json; // 或者使用 Newtonsoft.Json
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;


namespace ReadTXT
{
    public partial class ReadTXT : Form
    {
        private List<string>? fileLines; // 存储文件的所有行
        private Dictionary<string, int>? chapterIndices; // 存储章节标题到其行索引的映射
        private readonly SpeechSynthesizer synthesizer = new();
        private bool isSpeaking = false; // 用于跟踪是否正在朗读             


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
            public string? toolStripLabel6Pattern { get; set; }
            public string? FontName { get; set; }
            public int FontStyle { get; set; } // 注意：这里假设 FontStyle 是一个整数，您可能需要额外的逻辑来解析它
            public double FontSize { get; set; } // 注意：FontSize 是 double 类型
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
                this.toolStripLabel5.Text = "朗读速度只能设置-10~10的整数！！！";
                this.toolStripLabel5.ForeColor = Color.Red;
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
            string? searchText = this.toolStripLabel6.Text;
            string ck = "0";
            if (searchText != "")
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == searchText)
                    {
                        listBox1.SelectedIndex = i;
                        ck = "1";
                        break; // 找到匹配项后退出循环
                    }
                }
                //如果没有找到上次的章节
                if (ck == "0") { this.toolStripLabel5.Text = "你可能换了一本小说，找不到上次看的那章>_<"; }
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
            this.richTextBox1.Text = "";
            this.toolStripLabel5.Text = "";
            // 指定TXT文件的路径
            string filePath = this.textBox1.Text;
            try
            {
                LoadFileContent(filePath); // 加载文件内容
                PopulateListBox(); // 填充ListBox
                listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
                if (this.listBox1.Items.Count > 0)
                {
                    if (this.toolStripLabel6.Text == "")
                    {
                        this.toolStripLabel5.Text = "解析完成，开始阅读吧~";
                    }
                    else
                    {
                        this.toolStripLabel5.Text = "已定位到上次的章节，继续阅读吧~";
                    }
                    this.toolStripLabel5.ForeColor = Color.Black;
                }
                else
                {
                    this.toolStripLabel5.Text = "未能解析到相关章节！请确认文件中有与规则匹配的章节标题。";
                    this.toolStripLabel5.ForeColor = Color.Red;
                }

            }
            catch (Exception ex)
            {
                this.toolStripLabel5.Text = "读取文件时出错: " + ex.Message;
                this.toolStripLabel5.ForeColor = Color.Red;
            }
        }

        private void LoadFileContent(string filePath)
        {
            // 初始化 fileLines
            fileLines = [.. File.ReadAllLines(filePath)];
            if (this.textBox2.Text != "")
            {
                // 初始化 chapterIndices
                chapterIndices = [];
                // 遍历 fileLines 并填充 chapterIndices
                for (int i = 0; i < fileLines.Count; i++)
                {
                    // 根据 textBox2.Text 匹配章节标题的正则表达式模式
                    if (Regex.IsMatch(fileLines[i], this.textBox2.Text))
                    {
                        // 使用 Trim() 去除字符串两端的空白字符，并作为键添加到字典中
                        string trimmedLine = fileLines[i].Trim();
                        if (!chapterIndices.ContainsKey(trimmedLine)) // 避免重复键（虽然按您的逻辑这似乎不太可能发生）
                        {
                            chapterIndices[trimmedLine] = i;
                        }
                    }
                }
            }
            else
            {
                richTextBox1.Text = "";
                richTextBox1.Text = string.Join(Environment.NewLine, fileLines);
            }
        }
        private void PopulateListBox()
        {
            // 检查 chapterIndices 是否为 null
            if (chapterIndices != null)
            {
                listBox1.Items.Clear();
                foreach (var chapter in chapterIndices.Keys)
                {
                    listBox1.Items.Add(chapter);
                }
            }
            else
            {
                // 如果 chapterIndices 是 null，可以选择在 ListBox 中显示一条消息或什么都不做
                this.toolStripLabel5.Text = "未能匹配到相关章节！建议查看原始文档中章节标题是否与章节规则匹配！！！";
                this.toolStripLabel5.ForeColor = Color.Red;

            }
        }
        private void ListBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateTextBoxWithNextChapterText();
        }
        //点击设置：加载上次的阅读状态
        private void button2_Click(object sender, EventArgs e)
        {
            Load_set();
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
                        toolStripLabel6Pattern =this. toolStripLabel6.Text,
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
                    this.toolStripLabel6.Text = firstPattern.toolStripLabel6Pattern;//上次阅读到的章节
                    // 创建新的 Font 对象
                    FontStyle fontStyle = (FontStyle)firstPattern.FontStyle; // 这里假设 FontStyle 存储为整数，并且与 FontStyle 枚举的值匹配
                    Font newFont = new(
                        familyName: firstPattern.FontName ?? "Microsoft YaHei UI",
                        (float)firstPattern.FontSize, // 注意：FontSize 需要转换为 float
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
                    this.toolStripLabel5.Text = "JSON文件中没有有效的规则设置。";
                    this.toolStripLabel5.ForeColor = Color.Red;
                }
            }
            else
            {
                // 文件不存在，可以执行创建文件或其他操作
                this.toolStripLabel5.Text = "文件不存在: " + jsonPath;
                this.toolStripLabel5.ForeColor = Color.Red;
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
            StartRead();
        }
        //下一章
        private void toolStripButton3_Click(object? sender, EventArgs? e)
        {
            GoToNextChapter();
            StartRead();
        }

        private void StopRead()
        {
            isSpeaking = false; // 更新状态为不再朗读
            this.toolStripLabel5.Text = "已暂停朗读。";
            this.toolStripLabel5.ForeColor = Color.Black;
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // 取消订阅事件       
        }
        private void StartRead()
        {
            if (!isSpeaking)
            {
                isSpeaking = true; // 设置为正在朗读
                this.toolStripLabel5.Text = "正在朗读...";
                this.toolStripLabel5.ForeColor = Color.Black;
                if (this.toolStripComboBox2.Text == "手动整章")
                {
                    synthesizer.SpeakAsync(this.richTextBox1.Text);
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else if (this.toolStripComboBox2.Text == "手动整行")
                {
                    SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else if (this.toolStripComboBox2.Text == "自动整章")
                {
                    synthesizer.SpeakAsync(this.richTextBox1.Text);
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else if (this.toolStripComboBox2.Text == "自动整行")
                {
                    SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
            }
        }

        //获取下一章目录
        private void GoToNextChapter()
        {
            // 检查是否有选中的项
            if (listBox1.SelectedIndex >= 0)
            {
                // 获取选中的项
                string selectedItem = listBox1.Items[listBox1.SelectedIndex].ToString() ?? "Default Value";
                // 获取当前选中项的索引
                int selectedIndex = listBox1.SelectedIndex;
                // 检查是否不是最后一个项
                if (selectedIndex < listBox1.Items.Count - 1)
                {
                    // 获取下一个项的索引
                    int nextItemIndex = selectedIndex + 1;
                    listBox1.SelectedIndex = nextItemIndex;
                    this.toolStripLabel6.Text = selectedItem;
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else
                {
                    synthesizer.SpeakAsyncCancelAll();
                    isSpeaking = false; // 更新状态为不再朗读
                    this.toolStripLabel5.Text = "全文完。";
                    this.toolStripLabel5.ForeColor = Color.Black;
                    for (int i = 0; i < 5; i++)
                    {
                        synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted;
                    }
                }
            }
        }
        private void UpdateTextBoxWithNextChapterText()
        {
            if (chapterIndices != null && fileLines != null && listBox1.SelectedIndex >= 0)
            {
                object selectedItem = listBox1.Items[listBox1.SelectedIndex];
                string selectedChapter = selectedItem.ToString() ?? "Default Value";
                int selectedIndex = chapterIndices[selectedChapter];//本章起始行号                
                int nextIndex;
                // 尝试获取下一个章节的索引
                if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                {
                    string nextChapter = listBox1.Items[listBox1.SelectedIndex + 1].ToString() ?? "Default Value";
                    nextIndex = chapterIndices[nextChapter] - selectedIndex - 1;
                }
                else
                {
                    nextIndex = fileLines.Count - selectedIndex - 1;
                }
                // 提取两个章节之间的内容
                List<string> contentBetweenChapters = fileLines.GetRange(selectedIndex + 1, nextIndex);
                // 显示内容，这里以StringBuilder为例来合并多行内容
                StringBuilder contentBuilder = new();
                foreach (string line in contentBetweenChapters)
                {
                    contentBuilder.AppendLine(line);
                }
                // 在文本框中显示内容
                this.richTextBox1.Text = selectedChapter + "\r\n" + contentBuilder.ToString().Trim() + "\r\n";
                this.toolStripLabel6.Text = selectedChapter;
                this.toolStripLabel7.Text = "0";//本章起始行
            }
        }
        //朗读结束后的事件处理
        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {

            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // 取消订阅事件
            isSpeaking = false; // 重置为不在朗读状态

            if (this.toolStripComboBox2.Text == "手动整章")
            {
                this.toolStripLabel5.Text = "本章结束，请手动点下一章。";
            }
            else if (this.toolStripComboBox2.Text == "手动整行")
            {
                this.toolStripLabel5.Text = "本行结束，请手动点下一行。";
            }
            else if (this.toolStripComboBox2.Text == "自动整章")
            {
                GoToNextChapter();
                UpdateTextBoxWithNextChapterText();
                synthesizer.SpeakAsync(this.richTextBox1.Text);
            }
            else if (this.toolStripComboBox2.Text == "自动整行")
            {
                SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                if (this.toolStripLabel5.Text == "本章结束。")
                {
                    GoToNextChapter();
                    UpdateTextBoxWithNextChapterText();
                }
                else
                {
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }

            }
        }
        // 确保在表单关闭时释放资源
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            synthesizer.Dispose();
        }
        //改变正文字体
        private void SetRichTextBoxTextColor(RichTextBox richTextBox, Color color, string spk)
        {
            // 获取所有行的数量
            int lineCount = richTextBox.Lines.Length;
            this.toolStripLabel7.Text ??= "0";
            int i = int.Parse(this.toolStripLabel7.Text); // 将字符串转换为整数    
            int startIndex = richTextBox.GetFirstCharIndexFromLine(i);
            int length;
            if (i < lineCount - 1 && startIndex != -1)
            {
                int nextIndex = richTextBox.GetFirstCharIndexFromLine(i + 1);
                length = nextIndex - startIndex;// 获取当前行的长度（包括行尾的换行符）                
                i += 1;
            }
            else
            {
                startIndex = 0;
                length = richTextBox.Text.Length;// 获取当前行的长度（包括行尾的换行符）
            }
            this.toolStripLabel7.Text = i.ToString();
            richTextBox.Select(startIndex, length);
            string txt = richTextBox.SelectedText;
            if (length - richTextBox.Text.Length == 0 && spk == "1")//最后一行
            {
                txt = richTextBox.Lines[lineCount - 1];
            }
            richTextBox.SelectionColor = color;
            // 取消选择，避免用户看到选择区域 
            richTextBox.DeselectAll();
            if (spk == "1")
            {
                isSpeaking = false;
                if (!isSpeaking)
                {
                    isSpeaking = true; // 设置为正在朗读
                    this.toolStripLabel5.Text = "正在朗读...";
                    this.toolStripLabel5.ForeColor = Color.Black;
                    if (length - richTextBox.Text.Length == 0 && spk == "1")
                    {
                        this.toolStripLabel5.Text = "本章结束。";
                        this.toolStripLabel5.ForeColor = Color.Black;
                    }
                    synthesizer.SpeakAsync(txt);
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
            // 显示颜色对话框
            using ColorDialog colorDialog = this.colorDialog1 ?? new ColorDialog(); // 如果colorDialog1为空，则创建一个新的
                                                                                    // 如果用户点击了确定
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // 获取用户选择的颜色
                Color selectedColor = colorDialog.Color;
                // 应用颜色到RichTextBox的背景
                this.richTextBox1.BackColor = selectedColor;
                this.listBox1.BackColor = selectedColor;
            }
        }     
    }
}
