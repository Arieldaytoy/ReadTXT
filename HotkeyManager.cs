using static ReadTXT.ReadTXT;

namespace ReadTXT
{
    public class HotkeyManager
    {
        private readonly ReadTXT mainForm;
        private PatternItem config;
        private readonly Dictionary<string, Action> actionMap;
        private DateTime lastExecutionTime = DateTime.MinValue;
        private const int HOTKEY_COOLDOWN_MS = 300;

        public HotkeyManager(ReadTXT mainForm, PatternItem patternConfig)
        {
            this.mainForm = mainForm;
            this.config = patternConfig;

            // 初始化动作映射
            actionMap = new Dictionary<string, Action>
            {
                { "ToggleMode", ToggleMainMiniMode },
                { "MinimizeOrClose", HandleMinimizeOrClose },
                { "ToggleTopMost", ToggleMiniModeTopMost },
                { "StartReading", StartReading },
                { "StopReading", StopReading },
                { "NextChapter", NextChapter },
                { "SaveDocument", SaveDocument }
            };

            // 确保热键字典不为空
            if (config.Hotkeys == null)
            {
                config.Hotkeys = new Dictionary<string, string>
                {
                    { "ToggleMode", "Ctrl+Alt+M" },
                    { "MinimizeOrClose", "Ctrl+Alt+X" },
                    { "ToggleTopMost", "Ctrl+Alt+T" },
                    { "StartReading", "Ctrl+K" },
                    { "StopReading", "Ctrl+Z" },
                    { "NextChapter", "Ctrl+N" },
                    { "SaveDocument", "Ctrl+Alt+S" }
                };
            }
        }

        public bool ProcessHotkey(string hotkeyString)
        {
            var now = DateTime.Now;
            if ((now - lastExecutionTime).TotalMilliseconds < HOTKEY_COOLDOWN_MS)
                return false;

            // 查找匹配的热键
            if (config.Hotkeys == null)
                return false;

            foreach (var kvp in config.Hotkeys)
            {
                string actionName = kvp.Key;
                string configuredHotkey = kvp.Value;

                if (string.Equals(hotkeyString, configuredHotkey, StringComparison.OrdinalIgnoreCase))
                {
                    if (actionMap.TryGetValue(actionName, out Action action))
                    {
                        lastExecutionTime = now;
                        mainForm.BeginInvoke(new Action(() => action()));
                        return true;
                    }
                }
            }

            return false;
        }

        public Dictionary<string, string> GetCurrentHotkeys()
        {
            return config.Hotkeys ?? new Dictionary<string, string>();
        }

        public bool SetHotkey(string actionName, string hotkeyString)
        {
            if (!actionMap.ContainsKey(actionName))
                return false;

            if (config.Hotkeys == null)
                config.Hotkeys = new Dictionary<string, string>();

            config.Hotkeys[actionName] = hotkeyString;
            return true;
        }

        // 以下为具体的操作方法（保持不变）
        private void ToggleMainMiniMode()
        {
            if (mainForm.isSwitchingMode) return;
            mainForm.isSwitchingMode = true;

            try
            {
                bool isMainActive = mainForm.IsMainWindowActive();
                bool isMiniActive = mainForm.IsMiniModeActive();

                if (isMiniActive && !isMainActive)
                {
                    mainForm.SwitchToMainFromMini();
                }
                else if (!isMiniActive && isMainActive)
                {
                    mainForm.SwitchToMiniFromMain();
                }
                else
                {
                    mainForm.FixWindowStates();
                }
            }
            finally
            {
                Task.Delay(800).ContinueWith(_ =>
                {
                    mainForm.BeginInvoke(new Action(() => { mainForm.isSwitchingMode = false; }));
                });
            }
        }

        private void HandleMinimizeOrClose()
        {
            bool isMainActive = mainForm.IsMainWindowActive();
            bool isMiniActive = false;

            try
            {
                isMiniActive = mainForm.miniModeForm != null &&
                              !mainForm.miniModeForm.IsDisposed &&
                              mainForm.miniModeForm.Visible;
            }
            catch (ObjectDisposedException)
            {
                mainForm.miniModeForm = null;
                isMiniActive = false;
            }

            if (isMiniActive)
            {
                mainForm.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (mainForm.miniModeForm != null && !mainForm.miniModeForm.IsDisposed)
                        {
                            if (mainForm.miniModeForm is MiniModeForm miniForm)
                            {
                                miniForm.StopTimer();
                                miniForm.AllowClose();
                            }

                            mainForm.miniModeForm.FormClosing -= mainForm.MiniModeForm_FormClosing;
                            mainForm.miniModeForm.FormClosed -= mainForm.MiniModeForm_FormClosed;

                            mainForm.miniModeForm.Close();
                            mainForm.miniModeForm.Dispose();
                            mainForm.miniModeForm = null;

                            mainForm.LogStatus("迷你模式窗口已安全关闭");
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        mainForm.miniModeForm = null;
                        mainForm.LogStatus("迷你模式窗口已释放");
                    }
                }));
            }
            else if (isMainActive)
            {
                mainForm.MinimizeToTray();
            }
        }

        private void ToggleMiniModeTopMost()
        {
            mainForm.ToggleMiniModeTopMost();
        }

        private void StartReading()
        {
            if (!mainForm.GetSpeakingStatus())
            {
                mainForm.StartReadingFromMini();
            }
        }

        private void StopReading()
        {
            if (mainForm.GetSpeakingStatus())
            {
                mainForm.StopReadingFromMini();
            }
        }

        private void NextChapter()
        {
            mainForm.NextChapterFromMini();
        }

        private void SaveDocument()
        {
            mainForm.Save_toolStripButton_Click(null, null);
        }
    }
}