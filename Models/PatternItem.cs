using System.Text.Json.Serialization;

namespace ReadTXT;

public class PatternItem
{
    [JsonPropertyName("txtPathTextBoxPattern")]
    public string? TXTPath_textBoxPattern { get; set; }

    [JsonPropertyName("ruleTextBoxPattern")]
    public string? Rule_textBoxPattern { get; set; }

    [JsonPropertyName("speedToolStripComboBoxPattern")]
    public string? Speed_toolStripComboBoxPattern { get; set; }

    [JsonPropertyName("readModeToolStripComboBoxPattern")]
    public string? ReadMode_toolStripComboBoxPattern { get; set; }

    [JsonPropertyName("currentChapterToolStripStatusLabelPattern")]
    public string? CurrentChapter_toolStripStatusLabelPattern { get; set; }

    [JsonPropertyName("currentLineToolStripStatusLabelPattern")]
    public string? CurrentLine_toolStripStatusLabelPattern { get; set; }

    [JsonPropertyName("fontName")]
    public string? FontName { get; set; }

    [JsonPropertyName("fontStyle")]
    public int FontStyle { get; set; }

    [JsonPropertyName("fontSize")]
    public double FontSize { get; set; }

    [JsonPropertyName("blackColor")]
    public required BlackColor BlackColor { get; set; }

    [JsonPropertyName("hotkeysForJson")]
    public Dictionary<string, string> HotkeysForJson { get; set; } = new(DefaultHotkeys);

    [JsonIgnore]
    public Dictionary<string, string> Hotkeys
    {
        get => HotkeysForJson;
        set => HotkeysForJson = value;
    }

    /// <summary>
    /// 默认快捷键字典
    /// </summary>
    public static readonly Dictionary<string, string> DefaultHotkeys = new()
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
