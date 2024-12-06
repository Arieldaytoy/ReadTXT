using System.Speech.Synthesis;
using System.Text;
using System.Text.Json; // ����ʹ�� Newtonsoft.Json
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;



namespace ReadTXT
{
    public partial class ReadTXT : Form
    {
        private readonly SpeechSynthesizer synthesizer = new();
        private bool isSpeaking = false; // ���ڸ����Ƿ������ʶ�
        Dictionary<string, string> chapters = []; // ȷ����ѭ��ǰ��ʼ���ֵ�       
        private string currentChapterTitle; // ��ǰ���ڱ༭���½ڱ���

        public class BlackColor
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public byte A { get; set; }
            // �������Կ��ܲ���Ҫ�����������ض�����;
            [JsonIgnore] // �������ϣ��������Ա����л��� JSON������ʹ�� JsonIgnore
            public Color Color => Color.FromArgb(A, R, G, B);
        }
        // ������JSON�ṹ��ƥ�����
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
        // ���һ����������ȡJSON�ļ�
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
                // ���ʹ��Json.NET����ʹ�� JsonConvert.DeserializeObject<PatternsContainer>(jsonContent)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��ȡJSON�ļ�ʱ����: {ex.Message}");
                return null;
            }
        }
        // ��̬�� JsonSerializerOptions ʵ�������ڻ��������
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true
        };
        // �����������������л�����
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
            Load_set();//��ȡ�ϴε�����
            Analyze_novel();//����С˵
            SetListBoxSelectedItemByToolStripLabelText();//��λ���ϴ��½�
            // ��ʼ��ʱ�����ı���ɫ
            SetRichTextBoxTextColor(richTextBox1, Color.Black, "0");
            //��ȡ�ʶ��ٶȣ�����ٶȲ��Ϲ�����ΪĬ��ֵ=0
            if (int.TryParse(this.toolStripComboBox1.Text, out int rate))
            {
                synthesizer.Rate = rate;
            }
            else
            {
                // ����ת��ʧ�ܵ����������ʹ��Ĭ������
                this.toolStripComboBox1.Text = "0";
                synthesizer.Rate = 0; // ��������Ϊ��������Ϊ���ʵ�Ĭ��ֵ
                this.toolStripStatusLabel2.Text = "�ʶ��ٶ�ֻ������-10~10������������";
                this.toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }
        //�����Ѱ�װ������
        private void LoadTTSVoices()
        {
            ������ToolStripMenuItem.DropDownItems.Clear();
            foreach (var voice in synthesizer.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                ToolStripMenuItem voiceItem = new()
                {
                    Text = $"{info.Name} ({info.Culture.Name}) - {info.Gender}, {info.Age}",
                    CheckOnClick = true,
                    Tag = info.Name
                };
                ������ToolStripMenuItem.DropDownItems.Add(voiceItem);
                voiceItem.Click += (sender, e) =>
                {
                    if (sender is ToolStripMenuItem { Tag: string voiceName })
                    {
                        SelectVoice(voiceName);
                    }
                };
            }
            if (������ToolStripMenuItem.DropDownItems.Count == 0)
            {
                ������ToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("û�а�װ��������"));
            }
        }
        private void SelectVoice(string voiceName)
        {
            if (!string.IsNullOrEmpty(voiceName))
            {
                try
                {
                    synthesizer.SelectVoice(voiceName);
                    if (������ToolStripMenuItem.DropDownItems.Count > 0)
                    {
                        foreach (ToolStripMenuItem dropitem in ������ToolStripMenuItem.DropDownItems)
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
                    MessageBox.Show($"�޷�ѡ��������{ex.Message}");
                }
            }
        }

        //��λ���ϴ��Ķ��½�
        private void SetListBoxSelectedItemByToolStripLabelText()
        {
            string? searchText = this.toolStripStatusLabel4.Text;
            string ck = "0";
            if (searchText != "")
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == searchText)
                    {
                        listBox1.SelectedIndex = i;
                        ck = "1";
                        break; // �ҵ�ƥ������˳�ѭ��
                    }
                }
                //���û���ҵ��ϴε��½�
                if (ck == "0") { this.toolStripStatusLabel2.Text = "����ܻ���һ��С˵���Ҳ����ϴο�������>_<"; }
            }
        }
        //DragEnter�¼��������
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
        // DragDrop�¼��������
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

        //��������½ں�����
        private void button1_Click(object sender, EventArgs e)
        {
            Analyze_novel();
        }
        private void Analyze_novel()
        {
            this.toolStripStatusLabel2.Text = "";//��ǰ״̬
            this.toolStripStatusLabel4.Text="";//��ǰ�½�
            this.toolStripStatusLabel6.Text ="0";//��ǰ�� 
            this.listBox1.Items.Clear();//Ŀ¼
            currentChapterTitle="";//Ŀ¼
            this.richTextBox1.Text = "";//����
            chapters= [];//����            
            string filePath = this.textBox1.Text;// ָ��TXT�ļ���·��
            if (filePath!="")
            {
                try
                {
                    LoadFileContent(filePath); // �����ļ�����
                    PopulateListBox(); // ���ListBox
                    listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
                    if (this.listBox1.Items.Count > 0)
                    {
                        if (this.toolStripStatusLabel4.Text == "")
                        {
                            this.toolStripStatusLabel2.Text = "������ɣ���ʼ�Ķ���~";
                        }
                        else
                        {
                            this.toolStripStatusLabel2.Text = "�Ѷ�λ���ϴε��½ڣ������Ķ���~";
                        }
                        this.toolStripStatusLabel2.ForeColor = Color.Black;
                    }
                    else
                    {
                        this.toolStripStatusLabel2.Text = "δ�ܽ���������½ڣ���ȷ���ļ����������ƥ����½ڱ��⡣";
                        this.toolStripStatusLabel2.ForeColor = Color.Red;
                    }

                }
                catch (Exception ex)
                {
                    this.toolStripStatusLabel2.Text = "��ȡ�ļ�ʱ����: " + ex.Message;
                    this.toolStripStatusLabel2.ForeColor = Color.Red;
                }
            }                     
        }
        //�����½ںͶ�Ӧ������
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
                        chapters[chapterTitle] =chapterTitle+ chapterContent; 
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
        //���½���ӵ�Ŀ¼
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
                this.toolStripStatusLabel2.Text = "δ��ƥ�䵽����½ڣ�����鿴ԭʼ�ĵ����½ڱ����Ƿ����½ڹ���ƥ�䣡����";
                this.toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }

        private void ListBox1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.toolStripStatusLabel4.Text!=null)
            {
                chapters[this.toolStripStatusLabel4.Text] = this.richTextBox1.Text;
            }
            this.toolStripStatusLabel4.Text=listBox1.Items[listBox1.SelectedIndex].ToString() ?? "Default Value"; 
            UpdateText();
        }
        //������ã������ϴε��Ķ�״̬
        private void button2_Click(object sender, EventArgs e)
        {
            Load_set();
        }

        //�������ã����浱ǰ�Ķ�����
        private void button3_Click(object sender, EventArgs e)
        {
            // ����Ҫ���л�ΪJSON�Ķ���
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
            // ��ȡ����Ŀ¼
            string runningDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // ����JSON�ļ�·��
            string jsonFilePath = Path.Combine(runningDirectory, "mySet.json");
            // ���������л�ΪJSON�ַ���
            string jsonString = SerializeObject(patterns);
            // ��JSON�ַ���д���ļ�
            File.WriteAllText(jsonFilePath, jsonString);
        }

        private void Load_set()
        {
            string jsonPath = Path.Combine(Application.StartupPath, "mySet.json");
            bool fileExists = File.Exists(jsonPath);
            if (fileExists)
            {
                // ����JSON�ļ����߼�
                PatternsContainer? patterns = ReadJsonPatterns(jsonPath);
                if (patterns?.Patterns != null && patterns.Patterns.Length > 0)
                {
                    PatternItem firstPattern = patterns.Patterns[0];
                    this.textBox1.Text = firstPattern.textBox1Pattern;//�ϴ��Ķ���С˵·��
                    this.textBox2.Text = firstPattern.textBox2Pattern;//�ϴ��Ķ���С˵�½ڹ���
                    this.toolStripComboBox1.Text = firstPattern.toolStripComboBox1Pattern;//����
                    this.toolStripComboBox2.Text = firstPattern.toolStripComboBox2Pattern;//ģʽ
                    this.toolStripStatusLabel4.Text = firstPattern.toolStripStatusLabel4Pattern;//�ϴ��Ķ������½�
                    // �����µ� Font ����
                    FontStyle fontStyle = (FontStyle)firstPattern.FontStyle;
                    Font newFont = new(
                        familyName: firstPattern.FontName ?? "Microsoft YaHei UI",
                        (float)firstPattern.FontSize,
                        fontStyle
                    );
                    // ���� RichTextBox ������
                    this.richTextBox1.Font = newFont;
                    // �������ı���ɫ
                    this.richTextBox1.BackColor = firstPattern.BlackColor.Color;
                    this.listBox1.BackColor = firstPattern.BlackColor.Color;
                }
                else
                {
                    this.toolStripStatusLabel2.Text = "JSON�ļ���û����Ч�Ĺ������á�";
                    this.toolStripStatusLabel2.ForeColor = Color.Red;
                }
            }
            else
            {
                // �ļ������ڣ�����ִ�д����ļ�����������
                this.toolStripStatusLabel2.Text = "�ļ�������: " + jsonPath;
                this.toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }

        //�ʶ�����
        //��ͣ
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            StopRead();
        }
        //��ʼ
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            StopRead();
            StartRead();
        }
        //��һ��
        private void toolStripButton3_Click(object? sender, EventArgs? e)
        {
            GoToNextChapter();
            StartRead();
        }

        private void StopRead()
        {
            isSpeaking = false; // ����״̬Ϊ�����ʶ�
            this.toolStripStatusLabel2.Text = "����ͣ�ʶ���";
            this.toolStripStatusLabel2.ForeColor = Color.Black;
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // ȡ�������¼�       
        }
        private void StartRead()
        {
            if (int.TryParse(this.toolStripComboBox1.Text, out int rate))
            {
                synthesizer.Rate = rate;
            }
            if (!isSpeaking)
            {
                isSpeaking = true; // ����Ϊ�����ʶ�
                this.toolStripStatusLabel2.Text = "�����ʶ�...";
                this.toolStripStatusLabel2.ForeColor = Color.Black;
                if (this.toolStripComboBox2.Text == "�ֶ�����")
                {
                    synthesizer.SpeakAsync(this.richTextBox1.Text);
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else if (this.toolStripComboBox2.Text == "�ֶ�����")
                {
                    SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else if (this.toolStripComboBox2.Text == "�Զ�����")
                {
                    synthesizer.SpeakAsync(this.richTextBox1.Text);
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else if (this.toolStripComboBox2.Text == "�Զ�����")
                {
                    SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
            }
        }

        //��ȡ��һ��Ŀ¼
        private void GoToNextChapter()
        {
            if (listBox1.SelectedIndex >= 0)
            {
                // ��ȡѡ�е���
                string selectedItem = listBox1.Items[listBox1.SelectedIndex].ToString() ?? "Default Value";
                // ��ȡ��ǰѡ���������
                int selectedIndex = listBox1.SelectedIndex;
                // ����Ƿ������һ����
                if (selectedIndex < listBox1.Items.Count - 1)
                {
                    // ��ȡ��һ���������
                    int nextItemIndex = selectedIndex + 1;
                    listBox1.SelectedIndex = nextItemIndex;
                    this.toolStripStatusLabel4.Text = selectedItem;
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else
                {
                    synthesizer.SpeakAsyncCancelAll();
                    isSpeaking = false; // ����״̬Ϊ�����ʶ�
                    this.toolStripStatusLabel2.Text = "ȫ���ꡣ";
                    this.toolStripStatusLabel2.ForeColor = Color.Black;
                    for (int i = 0; i < 5; i++)
                    {
                        synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted;
                    }
                }
            }
        }
        private void UpdateText()
        {
            if (listBox1.SelectedIndex!=-1)
            {                
                object selectedItem = listBox1.Items[listBox1.SelectedIndex];
                string selectedChapter = selectedItem.ToString() ?? "Default Value";
                currentChapterTitle= selectedChapter;
                if (chapters.TryGetValue(currentChapterTitle, out string? value))
                {
                    this.richTextBox1.Text = value;
                    this.toolStripStatusLabel6.Text="0";
                }
            }
        }
        //�ʶ���������¼�����
        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {

            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // ȡ�������¼�
            isSpeaking = false; // ����Ϊ�����ʶ�״̬

            if (this.toolStripComboBox2.Text == "�ֶ�����")
            {
                this.toolStripStatusLabel2.Text = "���½��������ֶ�����һ�¡�";
            }
            else if (this.toolStripComboBox2.Text == "�ֶ�����")
            {
                this.toolStripStatusLabel2.Text = "���н��������ֶ�����һ�С�";
            }
            else if (this.toolStripComboBox2.Text == "�Զ�����")
            {
                GoToNextChapter();
                UpdateText();
                synthesizer.SpeakAsync(this.richTextBox1.Text);
            }
            else if (this.toolStripComboBox2.Text == "�Զ�����")
            {
                SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                if (this.toolStripStatusLabel2.Text == "���½�����")
                {
                    GoToNextChapter();
                    UpdateText();
                }
                else
                {
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }

            }
        }
        // ȷ���ڱ��ر�ʱ�ͷ���Դ
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            synthesizer.Dispose();
        }
        //�ı��ʶ��������ɫ
        private void SetRichTextBoxTextColor(RichTextBox richTextBox, Color color, string spk)
        {
            // ��ȡ�����е�����
            int lineCount = richTextBox.Lines.Length;
            this.toolStripStatusLabel6.Text ??= "0";
            int i = int.Parse(this.toolStripStatusLabel6.Text); // ���ַ���ת��Ϊ����    
            int startIndex = richTextBox.GetFirstCharIndexFromLine(i);
            int length;
            if (i < lineCount - 1 && startIndex != -1)
            {
                int nextIndex = richTextBox.GetFirstCharIndexFromLine(i + 1);
                length = nextIndex - startIndex;// ��ȡ��ǰ�еĳ��ȣ�������β�Ļ��з���                
                i += 1;
            }
            else
            {
                startIndex = 0;
                length = richTextBox.Text.Length;// ��ȡ��ǰ�еĳ��ȣ�������β�Ļ��з���
            }
            this.toolStripStatusLabel6.Text = i.ToString();
            //�ʶ���������Ϊ��ɫ
            richTextBox.Select(startIndex, length);
            richTextBox.SelectionColor = color;
            string txt = richTextBox.SelectedText;
            // ȡ��ѡ�񣬱����û�����ѡ������ 
            richTextBox.DeselectAll();
            if (length - richTextBox.Text.Length == 0 && spk == "1")//���һ��
            {
                txt = richTextBox.Lines[lineCount - 1];
            }
            if (spk == "1")
            {
                isSpeaking = false;
                if (!isSpeaking)
                {
                    isSpeaking = true; // ����Ϊ�����ʶ�
                    this.toolStripStatusLabel2.Text = "�����ʶ�...";
                    this.toolStripStatusLabel2.ForeColor = Color.Black;
                    if (length - richTextBox.Text.Length == 0 && spk == "1")
                    {
                        this.toolStripStatusLabel2.Text = "���½�����";
                        this.toolStripStatusLabel2.ForeColor = Color.Black;
                    }
                    synthesizer.SpeakAsync(txt);
                }
            }
        }
        //�޸�����
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ��ʾ����Ի���
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                // ��ȡ�û�ѡ�������
                Font selectedFont = fontDialog1.Font;
                richTextBox1.Font = selectedFont;
            }
        }
        //�޸ı���ɫ
        private void ����ɫToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ����û������ȷ��                                                                        
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                // ��ȡ�û�ѡ�����ɫ
                Color selectedColor = colorDialog1.Color;
                // Ӧ����ɫ��RichTextBox�ı���
                this.richTextBox1.BackColor = selectedColor;
                this.listBox1.BackColor = selectedColor;
            }
        }
        //����
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //ԭ�����ļ�·��
            string originalFilePath = this.textBox1.Text;
            string directory = Path.GetDirectoryName(originalFilePath);
            string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = Path.GetExtension(originalFilePath);
            // ��ȡ��ǰʱ�䣬����ʽ��
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyyMMddHHmmss");
            // �����µ��ļ���
            string newFileName = $"{fileName}-{timestamp}{extension}";
            string newFilePath = Path.Combine(directory, newFileName);
            Encoding selectedEncoding = this.comboBox1.SelectedItem.ToString() switch
            {
                "UTF-8" => Encoding.UTF8,
                "UTF-8 BOM" => new UTF8Encoding(true),// ���� BOM �� UTF-8 ����
                "ANSI" => Encoding.Default,// ANSI
                _ => Encoding.UTF8,
            };
            if (!string.IsNullOrEmpty(currentChapterTitle))
            {
                chapters[currentChapterTitle] = this.richTextBox1.Text;
                using StreamWriter writer = new(newFilePath, false, selectedEncoding);
                foreach (var chapter in chapters)
                {
                    writer.Write(chapter.Value); // д���½�����
                    writer.WriteLine(); // ��ÿ���½����ݺ����һ��������Ϊ�ָ�
                }
            }
            else
            {
                using StreamWriter writer = new(newFilePath, false, selectedEncoding);
                writer.Write(this.richTextBox1.Text); // д���½�����
            }
            this.toolStripStatusLabel2.Text = "����ɹ���·����"+ newFilePath;
            this.toolStripStatusLabel2.ForeColor = Color.Black;
        }
    }
}
