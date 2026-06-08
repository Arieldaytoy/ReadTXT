namespace ReadTXT
{
    public class HotkeyManager
    {
        private readonly ReadTXT mainForm;
        private readonly PatternItem config;
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
            config.Hotkeys ??= new Dictionary<string, string>
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
            return config.Hotkeys ?? [];
        }

        public bool SetHotkey(string actionName, string hotkeyString)
        {
            if (!actionMap.ContainsKey(actionName))
                return false;

            config.Hotkeys ??= [];

            config.Hotkeys[actionName] = hotkeyString;
            return true;
        }

        // 以下为具体的操作方法
        private void ToggleMainMiniMode()
        {
            mainForm.ToggleMainMiniMode();
        }

        private void HandleMinimizeOrClose()
        {
            mainForm.HandleMinimizeOrClose();
        }

        private void ToggleMiniModeTopMost()
        {
            mainForm.ToggleMiniModeTopMost();
        }

        private void StartReading()
        {
            mainForm.StartReadingFromHotkey();
        }

        private void StopReading()
        {
            mainForm.StopReadingFromHotkey();
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