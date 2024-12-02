using System.Speech.Synthesis;
using System.Text;
using System.Text.Json; // 或者使用 Newtonsoft.Json
using System.Text.RegularExpressions;
using Application = System.Windows.Forms.Application;


namespace ReadTXT
{
    public partial class ReadTXT : Form
    {
        private List<string> fileLines; // 存储文件的所有行
        private Dictionary<string, int> chapterIndices; // 存储章节标题到其行索引的映射
        private readonly SpeechSynthesizer synthesizer = new();
        private bool isSpeaking = false; // 用于跟踪是否正在朗读               



        // 定义与JSON结构相匹配的类
        public class PatternItem
        {
            public required string textBox1Pattern { get; set; }
            public required string textBox2Pattern { get; set; }
            public required string toolStripComboBox1Pattern { get; set; }
            public required string toolStripComboBox2Pattern { get; set; }
            public required string toolStripLabel6Pattern { get; set; }

        }
        // 添加一个方法来读取JSON文件
        public class PatternsContainer
        {
            public required PatternItem[] Patterns { get; set; }
        }
        private static PatternsContainer ReadJsonPatterns(string filePath)
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
            textBox1.AllowDrop = true;
            textBox1.DragEnter += new DragEventHandler(textBox1_DragEnter);
            textBox1.DragDrop += new DragEventHandler(textBox1_DragDrop);
            this.FormClosed += new FormClosedEventHandler(Read_TXTClosed);
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
                this.toolStripLabel5.Text = "朗读速度只能设置-10~10的整数！！";
            }
        }
        private void SetListBoxSelectedItemByToolStripLabelText()
        {
            string searchText = this.toolStripLabel6.Text;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.Items[i].ToString() == searchText)
                {
                    listBox1.SelectedIndex = i;
                    break; // 找到匹配项后退出循环
                }
            }
            // 如果没有找到匹配项，可以考虑将 SelectedIndex 设置为 -1（表示没有选定项）
            // 或者执行其他逻辑
            // if (listBox1.SelectedIndex == -1)
            // {
            //     // 没有找到匹配项时的处理逻辑
            // }
        }

        //退出时保存当前设置以及章节
        private void Read_TXTClosed(object? sender, FormClosedEventArgs e)
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
                        toolStripComboBox1Pattern = toolStripComboBox1.Text,
                        toolStripComboBox2Pattern = toolStripComboBox2.Text,
                        toolStripLabel6Pattern = toolStripLabel6.Text

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

        // DragEnter事件处理程序
        private void textBox1_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // 表示接受拖拽
            }
            else
            {
                e.Effect = DragDropEffects.None; // 表示不接受拖拽
            }
        }
        // DragDrop事件处理程序
        private void textBox1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string filePath = files[0];
                    textBox1.Text = filePath; // 或者你可以使用Path.GetFileName(filePath)来获取文件名
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
                this.toolStripLabel5.Text = "解析完成，开始阅读吧~";
                this.toolStripLabel6.Text = "";
            }
            catch (Exception ex)
            {
                this.toolStripLabel5.Text = "读取文件时出错: " + ex.Message;
            }
        }

        private void LoadFileContent(string filePath)
        {
            fileLines = [.. File.ReadAllLines(filePath)];
            chapterIndices = [];

            for (int i = 0; i < fileLines.Count; i++)
            {
                if (Regex.IsMatch(fileLines[i], this.textBox2.Text))
                {
                    chapterIndices[fileLines[i].Trim()] = i;
                }
            }
        }
        private void PopulateListBox()
        {
            listBox1.Items.Clear();
            foreach (var chapter in chapterIndices.Keys)
            {
                listBox1.Items.Add(chapter);
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

        private void Load_set()
        {
            string jsonPath = Path.Combine(Application.StartupPath, "mySet.json");
            bool fileExists = File.Exists(jsonPath);
            if (fileExists)
            {
                // 处理JSON文件的逻辑
                PatternsContainer patterns = ReadJsonPatterns(jsonPath);
                if (patterns?.Patterns != null && patterns.Patterns.Length > 0)
                {
                    PatternItem firstPattern = patterns.Patterns[0];
                    this.textBox1.Text = firstPattern.textBox1Pattern;//上次阅读的小说路径
                    this.textBox2.Text = firstPattern.textBox2Pattern;//上次阅读的小说章节规则
                    this.toolStripComboBox1.Text = firstPattern.toolStripComboBox1Pattern;//语速
                    this.toolStripComboBox2.Text = firstPattern.toolStripComboBox2Pattern;//模式
                    this.toolStripLabel6.Text = firstPattern.toolStripLabel6Pattern;//上次阅读到的章节
                }
                else
                {
                    this.toolStripLabel5.Text = "JSON文件中没有有效的规则设置。";
                }
            }
            else
            {
                // 文件不存在，可以执行创建文件或其他操作
                this.toolStripLabel5.Text = "文件不存在: " + jsonPath;
            }
        }

        //朗读操作
        //暂停
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            isSpeaking = false; // 更新状态为不再朗读
            this.toolStripLabel5.Text = "已暂停朗读。";
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // 取消订阅事件            
        }
        //开始
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //isSpeaking = false;
            // 确保不在朗读时才开始新的朗读
            if (!isSpeaking)
            {
                isSpeaking = true; // 设置为正在朗读
                this.toolStripLabel5.Text = "正在朗读...";
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
        //下一章
        private void toolStripButton3_Click(object? sender, EventArgs? e)
        {
            GoToNextChapter();
        }

        //获取下一章目录
        private void GoToNextChapter()
        {
            // 检查是否有选中的项
            if (listBox1.SelectedIndex >= 0)
            {
                // 获取选中的项
                string selectedItem = listBox1.SelectedItem.ToString();
                // 获取当前选中项的索引
                int selectedIndex = listBox1.SelectedIndex;
                // 检查是否不是最后一个项
                if (selectedIndex < listBox1.Items.Count - 1)
                {
                    // 获取下一个项的索引
                    int nextItemIndex = selectedIndex + 1;
                    listBox1.SelectedIndex = nextItemIndex;
                    this.toolStripLabel6.Text = listBox1.SelectedItem.ToString();
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else
                {
                    synthesizer.SpeakAsyncCancelAll();
                    isSpeaking = false; // 更新状态为不再朗读
                    this.toolStripLabel5.Text = "全文完。";
                    for (int i = 0; i < 5; i++)
                    {
                        synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted;
                    }
                }
            }
        }
        private void UpdateTextBoxWithNextChapterText()
        {
            if (listBox1.SelectedIndex >= 0)
            {
                string selectedChapter = listBox1.SelectedItem.ToString();
                int selectedIndex = chapterIndices[selectedChapter];//本章起始行号                
                int nextIndex;
                // 尝试获取下一个章节的索引
                if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                {
                    string nextChapter = listBox1.Items[listBox1.SelectedIndex + 1].ToString();
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
                this.richTextBox1.Text = listBox1.SelectedItem.ToString() + "\r\n" + contentBuilder.ToString().Trim() + "\r\n";
                this.toolStripLabel6.Text = listBox1.SelectedItem.ToString();
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

        private void SetRichTextBoxTextColor(RichTextBox richTextBox, Color color, string spk)
        {
            // 获取所有行的数量
            int lineCount = richTextBox.Lines.Length;
            int i = int.Parse(this.toolStripLabel7.Text); // 将字符串转换为整数     
            int startIndex = richTextBox.GetFirstCharIndexFromLine(i);
            int length;
            if (i < lineCount - 1 && startIndex != -1)
            {
                int nextIndex = richTextBox.GetFirstCharIndexFromLine(i + 1);
                length = nextIndex - startIndex;//richTextBox.Lines[i].Length;// 获取当前行的长度（包括行尾的换行符）                
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
            if (length - richTextBox.Text.Length == 0 && spk=="1")//最后一行
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
                    if (length - richTextBox.Text.Length == 0 && spk == "1")
                    {
                        this.toolStripLabel5.Text = "本章结束。";
                    }
                    synthesizer.SpeakAsync(txt);
                }
            }
        }
        //其他设置
        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.fontDialog1.ShowDialog();

        }

        private void 背景色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.colorDialog1=new ColorDialog();
        }
        


    }
}
