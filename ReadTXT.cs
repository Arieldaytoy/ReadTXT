using System.Speech.Synthesis;
using System.Text;
using System.Text.Json; // ����ʹ�� Newtonsoft.Json
using System.Text.RegularExpressions;
using Application = System.Windows.Forms.Application;


namespace ReadTXT
{
    public partial class ReadTXT : Form
    {
        private List<string> fileLines; // �洢�ļ���������
        private Dictionary<string, int> chapterIndices; // �洢�½ڱ��⵽����������ӳ��
        private readonly SpeechSynthesizer synthesizer = new();
        private bool isSpeaking = false; // ���ڸ����Ƿ������ʶ�               



        // ������JSON�ṹ��ƥ�����
        public class PatternItem
        {
            public required string textBox1Pattern { get; set; }
            public required string textBox2Pattern { get; set; }
            public required string toolStripComboBox1Pattern { get; set; }
            public required string toolStripComboBox2Pattern { get; set; }
            public required string toolStripLabel6Pattern { get; set; }

        }
        // ���һ����������ȡJSON�ļ�
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
            textBox1.AllowDrop = true;
            textBox1.DragEnter += new DragEventHandler(textBox1_DragEnter);
            textBox1.DragDrop += new DragEventHandler(textBox1_DragDrop);
            this.FormClosed += new FormClosedEventHandler(Read_TXTClosed);
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
                this.toolStripLabel5.Text = "�ʶ��ٶ�ֻ������-10~10����������";
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
                    break; // �ҵ�ƥ������˳�ѭ��
                }
            }
            // ���û���ҵ�ƥ������Կ��ǽ� SelectedIndex ����Ϊ -1����ʾû��ѡ���
            // ����ִ�������߼�
            // if (listBox1.SelectedIndex == -1)
            // {
            //     // û���ҵ�ƥ����ʱ�Ĵ����߼�
            // }
        }

        //�˳�ʱ���浱ǰ�����Լ��½�
        private void Read_TXTClosed(object? sender, FormClosedEventArgs e)
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
                        toolStripComboBox1Pattern = toolStripComboBox1.Text,
                        toolStripComboBox2Pattern = toolStripComboBox2.Text,
                        toolStripLabel6Pattern = toolStripLabel6.Text

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

        // DragEnter�¼��������
        private void textBox1_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // ��ʾ������ק
            }
            else
            {
                e.Effect = DragDropEffects.None; // ��ʾ��������ק
            }
        }
        // DragDrop�¼��������
        private void textBox1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string filePath = files[0];
                    textBox1.Text = filePath; // ���������ʹ��Path.GetFileName(filePath)����ȡ�ļ���
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
            this.richTextBox1.Text = "";
            this.toolStripLabel5.Text = "";
            // ָ��TXT�ļ���·��
            string filePath = this.textBox1.Text;
            try
            {
                LoadFileContent(filePath); // �����ļ�����
                PopulateListBox(); // ���ListBox
                listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
                this.toolStripLabel5.Text = "������ɣ���ʼ�Ķ���~";
                this.toolStripLabel6.Text = "";
            }
            catch (Exception ex)
            {
                this.toolStripLabel5.Text = "��ȡ�ļ�ʱ����: " + ex.Message;
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
        //������ã������ϴε��Ķ�״̬
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
                // ����JSON�ļ����߼�
                PatternsContainer patterns = ReadJsonPatterns(jsonPath);
                if (patterns?.Patterns != null && patterns.Patterns.Length > 0)
                {
                    PatternItem firstPattern = patterns.Patterns[0];
                    this.textBox1.Text = firstPattern.textBox1Pattern;//�ϴ��Ķ���С˵·��
                    this.textBox2.Text = firstPattern.textBox2Pattern;//�ϴ��Ķ���С˵�½ڹ���
                    this.toolStripComboBox1.Text = firstPattern.toolStripComboBox1Pattern;//����
                    this.toolStripComboBox2.Text = firstPattern.toolStripComboBox2Pattern;//ģʽ
                    this.toolStripLabel6.Text = firstPattern.toolStripLabel6Pattern;//�ϴ��Ķ������½�
                }
                else
                {
                    this.toolStripLabel5.Text = "JSON�ļ���û����Ч�Ĺ������á�";
                }
            }
            else
            {
                // �ļ������ڣ�����ִ�д����ļ�����������
                this.toolStripLabel5.Text = "�ļ�������: " + jsonPath;
            }
        }

        //�ʶ�����
        //��ͣ
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            isSpeaking = false; // ����״̬Ϊ�����ʶ�
            this.toolStripLabel5.Text = "����ͣ�ʶ���";
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // ȡ�������¼�            
        }
        //��ʼ
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //isSpeaking = false;
            // ȷ�������ʶ�ʱ�ſ�ʼ�µ��ʶ�
            if (!isSpeaking)
            {
                isSpeaking = true; // ����Ϊ�����ʶ�
                this.toolStripLabel5.Text = "�����ʶ�...";
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
        //��һ��
        private void toolStripButton3_Click(object? sender, EventArgs? e)
        {
            GoToNextChapter();
        }

        //��ȡ��һ��Ŀ¼
        private void GoToNextChapter()
        {
            // ����Ƿ���ѡ�е���
            if (listBox1.SelectedIndex >= 0)
            {
                // ��ȡѡ�е���
                string selectedItem = listBox1.SelectedItem.ToString();
                // ��ȡ��ǰѡ���������
                int selectedIndex = listBox1.SelectedIndex;
                // ����Ƿ������һ����
                if (selectedIndex < listBox1.Items.Count - 1)
                {
                    // ��ȡ��һ���������
                    int nextItemIndex = selectedIndex + 1;
                    listBox1.SelectedIndex = nextItemIndex;
                    this.toolStripLabel6.Text = listBox1.SelectedItem.ToString();
                    synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
                }
                else
                {
                    synthesizer.SpeakAsyncCancelAll();
                    isSpeaking = false; // ����״̬Ϊ�����ʶ�
                    this.toolStripLabel5.Text = "ȫ���ꡣ";
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
                int selectedIndex = chapterIndices[selectedChapter];//������ʼ�к�                
                int nextIndex;
                // ���Ի�ȡ��һ���½ڵ�����
                if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                {
                    string nextChapter = listBox1.Items[listBox1.SelectedIndex + 1].ToString();
                    nextIndex = chapterIndices[nextChapter] - selectedIndex - 1;
                }
                else
                {
                    nextIndex = fileLines.Count - selectedIndex - 1;
                }
                // ��ȡ�����½�֮�������
                List<string> contentBetweenChapters = fileLines.GetRange(selectedIndex + 1, nextIndex);
                // ��ʾ���ݣ�������StringBuilderΪ�����ϲ���������
                StringBuilder contentBuilder = new();
                foreach (string line in contentBetweenChapters)
                {
                    contentBuilder.AppendLine(line);
                }
                // ���ı�������ʾ����
                this.richTextBox1.Text = listBox1.SelectedItem.ToString() + "\r\n" + contentBuilder.ToString().Trim() + "\r\n";
                this.toolStripLabel6.Text = listBox1.SelectedItem.ToString();
                this.toolStripLabel7.Text = "0";//������ʼ��
            }
        }
        //�ʶ���������¼�����
        private void Synthesizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {

            synthesizer.SpeakCompleted -= Synthesizer_SpeakCompleted; // ȡ�������¼�
            isSpeaking = false; // ����Ϊ�����ʶ�״̬

            if (this.toolStripComboBox2.Text == "�ֶ�����")
            {
                this.toolStripLabel5.Text = "���½��������ֶ�����һ�¡�";
            }
            else if (this.toolStripComboBox2.Text == "�ֶ�����")
            {
                this.toolStripLabel5.Text = "���н��������ֶ�����һ�С�";
            }
            else if (this.toolStripComboBox2.Text == "�Զ�����")
            {
                GoToNextChapter();
                UpdateTextBoxWithNextChapterText();
                synthesizer.SpeakAsync(this.richTextBox1.Text);
            }
            else if (this.toolStripComboBox2.Text == "�Զ�����")
            {
                SetRichTextBoxTextColor(richTextBox1, Color.Red, "1");
                if (this.toolStripLabel5.Text == "���½�����")
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
        // ȷ���ڱ��ر�ʱ�ͷ���Դ
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            synthesizer.Dispose();
        }

        private void SetRichTextBoxTextColor(RichTextBox richTextBox, Color color, string spk)
        {
            // ��ȡ�����е�����
            int lineCount = richTextBox.Lines.Length;
            int i = int.Parse(this.toolStripLabel7.Text); // ���ַ���ת��Ϊ����     
            int startIndex = richTextBox.GetFirstCharIndexFromLine(i);
            int length;
            if (i < lineCount - 1 && startIndex != -1)
            {
                int nextIndex = richTextBox.GetFirstCharIndexFromLine(i + 1);
                length = nextIndex - startIndex;//richTextBox.Lines[i].Length;// ��ȡ��ǰ�еĳ��ȣ�������β�Ļ��з���                
                i += 1;
            }
            else
            {
                startIndex = 0;
                length = richTextBox.Text.Length;// ��ȡ��ǰ�еĳ��ȣ�������β�Ļ��з���

            }
            this.toolStripLabel7.Text = i.ToString();
            richTextBox.Select(startIndex, length);
            string txt = richTextBox.SelectedText;
            if (length - richTextBox.Text.Length == 0 && spk=="1")//���һ��
            {
                txt = richTextBox.Lines[lineCount - 1];

            }
            richTextBox.SelectionColor = color;
            // ȡ��ѡ�񣬱����û�����ѡ������ 
            richTextBox.DeselectAll();
            if (spk == "1")
            {
                isSpeaking = false;
                if (!isSpeaking)
                {
                    isSpeaking = true; // ����Ϊ�����ʶ�
                    this.toolStripLabel5.Text = "�����ʶ�...";
                    if (length - richTextBox.Text.Length == 0 && spk == "1")
                    {
                        this.toolStripLabel5.Text = "���½�����";
                    }
                    synthesizer.SpeakAsync(txt);
                }
            }
        }
        //��������
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.fontDialog1.ShowDialog();

        }

        private void ����ɫToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.colorDialog1=new ColorDialog();
        }
        


    }
}
