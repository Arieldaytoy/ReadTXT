namespace ReadTXT;

/// <summary>
/// 键盘按键码到字符串名称的映射表，供 GlobalKeyboardHook 和 HotkeyInputForm 共用。
/// </summary>
public static class KeyNameMap
{
    public static readonly Dictionary<int, string> Map = new()
    {
        // 字母键
        { (int)Keys.A, "A" }, { (int)Keys.B, "B" }, { (int)Keys.C, "C" }, { (int)Keys.D, "D" },
        { (int)Keys.E, "E" }, { (int)Keys.F, "F" }, { (int)Keys.G, "G" }, { (int)Keys.H, "H" },
        { (int)Keys.I, "I" }, { (int)Keys.J, "J" }, { (int)Keys.K, "K" }, { (int)Keys.L, "L" },
        { (int)Keys.M, "M" }, { (int)Keys.N, "N" }, { (int)Keys.O, "O" }, { (int)Keys.P, "P" },
        { (int)Keys.Q, "Q" }, { (int)Keys.R, "R" }, { (int)Keys.S, "S" }, { (int)Keys.T, "T" },
        { (int)Keys.U, "U" }, { (int)Keys.V, "V" }, { (int)Keys.W, "W" }, { (int)Keys.X, "X" },
        { (int)Keys.Y, "Y" }, { (int)Keys.Z, "Z" },

        // 数字键
        { (int)Keys.D0, "0" }, { (int)Keys.D1, "1" }, { (int)Keys.D2, "2" }, { (int)Keys.D3, "3" },
        { (int)Keys.D4, "4" }, { (int)Keys.D5, "5" }, { (int)Keys.D6, "6" }, { (int)Keys.D7, "7" },
        { (int)Keys.D8, "8" }, { (int)Keys.D9, "9" },

        // 功能键
        { (int)Keys.F1, "F1" }, { (int)Keys.F2, "F2" }, { (int)Keys.F3, "F3" }, { (int)Keys.F4, "F4" },
        { (int)Keys.F5, "F5" }, { (int)Keys.F6, "F6" }, { (int)Keys.F7, "F7" }, { (int)Keys.F8, "F8" },
        { (int)Keys.F9, "F9" }, { (int)Keys.F10, "F10" }, { (int)Keys.F11, "F11" }, { (int)Keys.F12, "F12" },

        // 特殊键
        { (int)Keys.Space, "Space" }, { (int)Keys.Enter, "Enter" }, { (int)Keys.Escape, "Esc" },
        { (int)Keys.Tab, "Tab" }, { (int)Keys.Back, "Backspace" }, { (int)Keys.Insert, "Insert" },
        { (int)Keys.Delete, "Delete" }, { (int)Keys.Home, "Home" }, { (int)Keys.End, "End" },
        { (int)Keys.PageUp, "PageUp" }, { (int)Keys.PageDown, "PageDown" },
        { (int)Keys.Up, "Up" }, { (int)Keys.Down, "Down" }, { (int)Keys.Left, "Left" }, { (int)Keys.Right, "Right" },

        // 符号键
        { (int)Keys.OemSemicolon, ";" }, { (int)Keys.Oemcomma, "," }, { (int)Keys.OemPeriod, "." },
        { (int)Keys.OemQuestion, "/" }, { (int)Keys.Oemtilde, "~" }, { (int)Keys.OemOpenBrackets, "[" },
        { (int)Keys.OemCloseBrackets, "]" }, { (int)Keys.OemPipe, "\\" }, { (int)Keys.OemQuotes, "'" },
        { (int)Keys.OemBackslash, "\\" }
    };
}
