namespace ReadTXT
{
    // 迷你模式窗口类
    public class MiniModeForm : Form
    {
        private readonly ReadTXT mainForm;
        private bool isTopMost = true;
        private System.Windows.Forms.Timer? updateTimer;
        private bool allowClose = false; // 控制是否允许关闭
        private TableLayoutPanel tableLayoutPanel1;
        private Button playPauseBtn;
        private Button nextChapterBtn;
        private Button restoreBtn;
        private Button topMostBtn;
        private Label currentChapterLabel;
        private TextBox statusTextBox;
        private bool isDisposed = false; // 跟踪是否已释放

        // 获取当前置顶状态
        public bool IsTopMost() => isTopMost;

        public MiniModeForm(ReadTXT mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void MiniModeForm_Load(object? sender, EventArgs e)
        {
            BindButtonEvents();
            InitializeMiniModeForm();
            SetFormPosition(); // 设置窗体位置
        }

        private void SetFormPosition()
        {
            // 正确设置窗体位置为屏幕右下角
            Screen screen = Screen.FromControl(this);

            // 计算窗体位置：屏幕右下角减去窗体大小
            int x = screen.WorkingArea.Right - this.Width;
            int y = screen.WorkingArea.Bottom - this.Height;

            this.Location = new Point(x, y);

            // 调试信息
            Console.WriteLine($"窗体位置: X={x}, Y={y}, 窗体大小: {this.Width}x{this.Height}");
            Console.WriteLine($"屏幕工作区: {screen.WorkingArea}");
        }

        private void InitializeMiniModeForm()
        {
            // 更新状态文本的方法
            void UpdateStatusText()
            {
                string newStatusText = mainForm.GetSpeakingStatus() ?
                    "正在朗读: \r\n" + mainForm.GetCurrentReadingText() :
                    "状态: 已暂停";

                if (statusTextBox.Text != newStatusText)
                {
                    statusTextBox.Text = newStatusText;

                    // 如果文本太长，自动滚动到最底部，显示最新内容
                    statusTextBox.SelectionStart = statusTextBox.Text.Length;
                    statusTextBox.ScrollToCaret();
                }
            }

            // 更新章节信息的方法
            void UpdateChapterText()
            {
                string newChapterText = "当前章节: " + mainForm.GetCurrentChapter();
                if (currentChapterLabel.Text != newChapterText)
                {
                    currentChapterLabel.Text = newChapterText;
                }
            }

            // 创建定时器
            updateTimer = new System.Windows.Forms.Timer
            {
                Interval = 500
            };

            updateTimer.Tick += (s, ev) =>
            {
                try
                {
                    // 检查窗体是否正在关闭或已释放
                    if (this.IsDisposed || !this.Visible)
                        return;

                    // 更新按钮文本
                    playPauseBtn.Text = mainForm.GetSpeakingStatus() ? "暂停" : "继续";

                    // 更新章节信息
                    UpdateChapterText();

                    // 更新状态信息
                    UpdateStatusText();
                }
                catch (ObjectDisposedException)
                {
                    // 如果窗体已释放，停止定时器
                    updateTimer?.Stop();
                }
                catch (Exception ex)
                {
                    mainForm.LogStatus($"迷你窗体定时器错误: {ex.Message}");
                }
            };

            updateTimer.Start();

            // 窗体关闭时停止定时器
            this.FormClosed += (s, ev) =>
            {
                updateTimer?.Stop();
                updateTimer?.Dispose();
                updateTimer = null;
            };

            // 初始更新
            UpdateChapterText();
            UpdateStatusText();
        }

        private void BindButtonEvents()
        {
            // 播放/暂停按钮
            playPauseBtn.Click += (s, ev) =>
            {
                if (mainForm.GetSpeakingStatus())
                {
                    mainForm.StopReadingFromMini();
                    playPauseBtn.Text = "继续";
                }
                else
                {
                    mainForm.StartReadingFromMini();
                    playPauseBtn.Text = "暂停";
                }
            };

            // 下一章按钮
            nextChapterBtn.Click += (s, ev) => mainForm.NextChapterFromMini();

            // 主窗口按钮
            restoreBtn.Click += (s, ev) => mainForm.SwitchToMainFromMini();

            // 置顶/不置顶按钮
            topMostBtn.Click += (s, ev) =>
            {
                isTopMost = !isTopMost;
                this.TopMost = isTopMost;
                topMostBtn.Text = isTopMost ? "不置顶" : "置顶";
            };
        }

        // 切换置顶状态
        public void ToggleTopMost()
        {
            isTopMost = !isTopMost;
            this.TopMost = isTopMost;

            // 更新按钮文本
            foreach (Control control in this.Controls)
            {
                if (control is Button button && (button.Text == "置顶" || button.Text == "不置顶"))
                {
                    button.Text = isTopMost ? "不置顶" : "置顶";
                    break;
                }
            }
        }

        // 允许关闭的方法（供外部调用）
        public void AllowClose()
        {
            allowClose = true;
        }

        // 安全停止定时器的方法
        public void StopTimer()
        {
            try
            {
                if (updateTimer != null)
                {
                    updateTimer.Stop();
                    updateTimer.Dispose();
                    updateTimer = null;
                }
            }
            catch (Exception ex)
            {
                mainForm.LogStatus($"停止迷你窗体定时器失败: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 停止定时器，防止在关闭过程中触发事件
            StopTimer();

            base.OnFormClosing(e);

            if (!allowClose && e.CloseReason == CloseReason.UserClosing)
            {
                // 用户点击关闭按钮时，最小化而不是关闭
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                return;
            }

            // 程序控制的关闭，允许关闭
            isDisposed = true;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // 确保定时器已停止
            StopTimer();

            base.OnFormClosed(e);

            // 移除主窗体的引用
            if (mainForm != null && !mainForm.IsDisposed)
            {
                // 确保移除事件绑定
                this.FormClosing -= mainForm.MiniModeForm_FormClosing;
                this.FormClosed -= mainForm.MiniModeForm_FormClosed;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // 清理托管资源
                    StopTimer();
                }

                isDisposed = true;
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            playPauseBtn = new Button();
            nextChapterBtn = new Button();
            restoreBtn = new Button();
            topMostBtn = new Button();
            currentChapterLabel = new Label();
            statusTextBox = new TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 79.591835F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.4081631F));
            tableLayoutPanel1.Controls.Add(playPauseBtn, 1, 0);
            tableLayoutPanel1.Controls.Add(nextChapterBtn, 1, 1);
            tableLayoutPanel1.Controls.Add(restoreBtn, 1, 2);
            tableLayoutPanel1.Controls.Add(topMostBtn, 1, 3);
            tableLayoutPanel1.Controls.Add(currentChapterLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(statusTextBox, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(344, 141);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // playPauseBtn
            // 
            playPauseBtn.Dock = DockStyle.Fill;
            playPauseBtn.Location = new Point(276, 3);
            playPauseBtn.Name = "playPauseBtn";
            playPauseBtn.Size = new Size(65, 29);
            playPauseBtn.TabIndex = 0;
            playPauseBtn.Text = "继续";
            playPauseBtn.UseVisualStyleBackColor = true;
            // 
            // nextChapterBtn
            // 
            nextChapterBtn.Dock = DockStyle.Fill;
            nextChapterBtn.Location = new Point(276, 38);
            nextChapterBtn.Name = "nextChapterBtn";
            nextChapterBtn.Size = new Size(65, 29);
            nextChapterBtn.TabIndex = 1;
            nextChapterBtn.Text = "下一章";
            nextChapterBtn.UseVisualStyleBackColor = true;
            // 
            // restoreBtn
            // 
            restoreBtn.Dock = DockStyle.Fill;
            restoreBtn.Location = new Point(276, 73);
            restoreBtn.Name = "restoreBtn";
            restoreBtn.Size = new Size(65, 29);
            restoreBtn.TabIndex = 2;
            restoreBtn.Text = "主窗口";
            restoreBtn.UseVisualStyleBackColor = true;
            // 
            // topMostBtn
            // 
            topMostBtn.Dock = DockStyle.Fill;
            topMostBtn.Location = new Point(276, 108);
            topMostBtn.Name = "topMostBtn";
            topMostBtn.Size = new Size(65, 30);
            topMostBtn.TabIndex = 3;
            topMostBtn.Text = "不置顶";
            topMostBtn.UseVisualStyleBackColor = true;
            // 
            // currentChapterLabel
            // 
            currentChapterLabel.AutoSize = true;
            currentChapterLabel.Dock = DockStyle.Fill;
            currentChapterLabel.Location = new Point(3, 0);
            currentChapterLabel.Name = "currentChapterLabel";
            currentChapterLabel.Size = new Size(267, 35);
            currentChapterLabel.TabIndex = 4;
            currentChapterLabel.Text = "当前章节: ";
            currentChapterLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statusTextBox
            // 
            statusTextBox.Dock = DockStyle.Fill;
            statusTextBox.Location = new Point(3, 38);
            statusTextBox.Multiline = true;
            statusTextBox.Name = "statusTextBox";
            tableLayoutPanel1.SetRowSpan(statusTextBox, 3);
            statusTextBox.ScrollBars = ScrollBars.Vertical;
            statusTextBox.Size = new Size(267, 100);
            statusTextBox.TabIndex = 5;
            statusTextBox.Text = "状态: 已暂停";
            // 
            // MiniModeForm
            // 
            ClientSize = new Size(344, 141);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MaximumSize = new Size(360, 180);
            MinimizeBox = false;
            MinimumSize = new Size(360, 180);
            Name = "MiniModeForm";
            // 重要：改为 Manual，这样才能手动控制位置
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            Load += MiniModeForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        // 检查定时器是否在运行
        public bool IsTimerRunning()
        {
            return updateTimer != null && updateTimer.Enabled;
        }
    }
}