using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ReadTXT
{
    public class HotkeyEventArgs : EventArgs
    {
        public string HotkeyString { get; }
        public bool Handled { get; set; }

        public HotkeyEventArgs(string hotkeyString)
        {
            HotkeyString = hotkeyString;
            Handled = false;
        }
    }

    public class GlobalKeyboardHook : IDisposable
    {
        private IntPtr hookId = IntPtr.Zero;
        private readonly LowLevelKeyboardProc hookProc;
        public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

        // 修饰键状态跟踪
        private bool ctrlPressed = false;
        private bool altPressed = false;
        private bool shiftPressed = false;
        private bool winPressed = false;



        public GlobalKeyboardHook()
        {
            hookProc = HookCallback;
            hookId = SetHook(hookProc);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule?.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = (Keys)vkCode;

                // 更新修饰键状态
                UpdateModifierState(key, wParam);

                // 只处理键按下事件
                if (wParam == (IntPtr)0x0100) // WM_KEYDOWN
                {
                    // 如果是修饰键，不单独触发热键
                    if (IsModifierKey(key))
                        return CallNextHookEx(hookId, nCode, wParam, lParam);

                    // 获取热键字符串
                    string hotkeyString = BuildHotkeyString(key);
                    if (!string.IsNullOrEmpty(hotkeyString))
                    {
                        HotkeyPressed?.Invoke(this, new HotkeyEventArgs(hotkeyString));
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private void UpdateModifierState(Keys key, IntPtr wParam)
        {
            bool isKeyDown = wParam == (IntPtr)0x0100; // WM_KEYDOWN
            bool isKeyUp = wParam == (IntPtr)0x0101;   // WM_KEYUP

            if (isKeyDown || isKeyUp)
            {
                bool pressed = isKeyDown;

                switch (key)
                {
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                    case Keys.ControlKey:
                        ctrlPressed = pressed;
                        break;
                    case Keys.LMenu:
                    case Keys.RMenu:
                    case Keys.Menu:
                        altPressed = pressed;
                        break;
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                    case Keys.ShiftKey:
                        shiftPressed = pressed;
                        break;
                    case Keys.LWin:
                    case Keys.RWin:
                        winPressed = pressed;
                        break;
                }
            }
        }

        private bool IsModifierKey(Keys key)
        {
            return key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.ControlKey ||
                   key == Keys.LMenu || key == Keys.RMenu || key == Keys.Menu ||
                   key == Keys.LShiftKey || key == Keys.RShiftKey || key == Keys.ShiftKey ||
                   key == Keys.LWin || key == Keys.RWin;
        }

        private string BuildHotkeyString(Keys key)
        {
            List<string> parts = new List<string>();

            // 按固定顺序添加修饰键
            if (ctrlPressed) parts.Add("Ctrl");
            if (altPressed) parts.Add("Alt");
            if (shiftPressed) parts.Add("Shift");
            if (winPressed) parts.Add("Win");

            // 获取主键的名称
            if (KeyNameMap.Map.TryGetValue((int)key, out string? keyName))
            {
                parts.Add(keyName);
            }
            else
            {
                // 如果不在映射表中，使用键的名称（去掉"Key"后缀）
                string defaultName = key.ToString();
                if (defaultName.EndsWith("Key"))
                    defaultName = defaultName.Substring(0, defaultName.Length - 3);
                parts.Add(defaultName);
            }

            // 如果没有修饰键，不生成热键字符串（防止普通按键触发热键）
            if (parts.Count <= 1)
                return string.Empty;

            return string.Join("+", parts);
        }

        #region Windows API
        private const int WH_KEYBOARD_LL = 13;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);
        #endregion

        public void Dispose()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }
    }
}