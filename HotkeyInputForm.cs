using System.Runtime.InteropServices;

namespace ReadTXT
{
    public partial class HotkeyInputForm : Form
    {
        // 记录按下的修饰键
        private bool ctrlPressed = false;
        private bool altPressed = false;
        private bool shiftPressed = false;
        private bool winPressed = false;

        // 记录按下的主键
        private Keys? primaryKey = null;

        // 返回的热键字符串
        public string HotkeyString { get; private set; } = string.Empty;

        public HotkeyInputForm(string currentHotkey = "")
        {
            InitializeComponent();
            HotkeyString = currentHotkey;
            UpdateDisplay();

            // 设置窗体属性以捕获按键
            this.KeyPreview = true;
            this.KeyDown += HotkeyInputForm_KeyDown;
            this.KeyUp += HotkeyInputForm_KeyUp;
        }

        private void UpdateDisplay()
        {
            if (primaryKey.HasValue)
            {
                List<string> parts = [];

                if (ctrlPressed) parts.Add("Ctrl");
                if (altPressed) parts.Add("Alt");
                if (shiftPressed) parts.Add("Shift");
                if (winPressed) parts.Add("Win");

                // 获取键的名称
                string keyName = GetKeyName(primaryKey.Value);
                if (!string.IsNullOrEmpty(keyName))
                {
                    parts.Add(keyName);
                }

                HotkeyString = string.Join("+", parts);
                hotkeyLabel.Text = HotkeyString;
            }
            else
            {
                hotkeyLabel.Text = "(请按下组合键)";
            }
        }

        private string GetKeyName(Keys key)
        {
            if (KeyNameMap.Map.TryGetValue((int)key, out string? keyName))
            {
                return keyName;
            }

            // 如果不在映射表中，返回原始名称
            return key.ToString();
        }

        private void HotkeyInputForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // 更新修饰键状态
            UpdateModifierState(e.KeyCode, true);

            // 如果不是修饰键，则作为主键
            if (!IsModifierKey(e.KeyCode))
            {
                primaryKey = e.KeyCode;
                UpdateDisplay();
            }

            // 阻止按键继续传递
            e.SuppressKeyPress = true;
        }

        private void HotkeyInputForm_KeyUp(object? sender, KeyEventArgs e)
        {
            // 更新修饰键状态
            UpdateModifierState(e.KeyCode, false);

            // 阻止按键继续传递
            e.SuppressKeyPress = true;
        }

        private void UpdateModifierState(Keys key, bool isPressed)
        {
            switch (key)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    ctrlPressed = isPressed;
                    break;
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    altPressed = isPressed;
                    break;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    shiftPressed = isPressed;
                    break;
                case Keys.LWin:
                case Keys.RWin:
                    winPressed = isPressed;
                    break;
            }
        }

        private bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey ||
                   key == Keys.Menu || key == Keys.LMenu || key == Keys.RMenu ||
                   key == Keys.ShiftKey || key == Keys.LShiftKey || key == Keys.RShiftKey ||
                   key == Keys.LWin || key == Keys.RWin;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            // 验证热键
            if (primaryKey.HasValue)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("请先按下有效的组合键！", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            HotkeyString = string.Empty;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            // 清除当前设置
            ctrlPressed = false;
            altPressed = false;
            shiftPressed = false;
            winPressed = false;
            primaryKey = null;
            HotkeyString = string.Empty;
            UpdateDisplay();
        }

        private void InitializeComponent()
        {
            this.hotkeyLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.instructionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // instructionLabel
            this.instructionLabel.AutoSize = true;
            this.instructionLabel.Location = new System.Drawing.Point(12, 20);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(131, 15);
            this.instructionLabel.TabIndex = 0;
            this.instructionLabel.Text = "请按下快捷键组合：";

            // hotkeyLabel
            this.hotkeyLabel.AutoSize = true;
            this.hotkeyLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.hotkeyLabel.Location = new System.Drawing.Point(12, 50);
            this.hotkeyLabel.Name = "hotkeyLabel";
            this.hotkeyLabel.Size = new System.Drawing.Size(170, 21);
            this.hotkeyLabel.TabIndex = 1;
            this.hotkeyLabel.Text = "(请按下组合键)";

            // okButton
            this.okButton.Location = new System.Drawing.Point(12, 90);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 30);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "确定";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);

            // clearButton
            this.clearButton.Location = new System.Drawing.Point(93, 90);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 30);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "清除";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);

            // cancelButton
            this.cancelButton.Location = new System.Drawing.Point(174, 90);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 30);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);

            // HotkeyInputForm
            this.ClientSize = new System.Drawing.Size(260, 135);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.hotkeyLabel);
            this.Controls.Add(this.instructionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HotkeyInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置快捷键";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label instructionLabel;
        private System.Windows.Forms.Label hotkeyLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button clearButton;
    }
}