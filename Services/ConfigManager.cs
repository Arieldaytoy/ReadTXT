using System.Text.Json;

namespace ReadTXT;

/// <summary>
/// 配置管理器：负责 mySet.json 配置文件的读写。
/// </summary>
public class ConfigManager
{
    private readonly string _jsonFilePath;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public ConfigManager(string jsonFilePath)
    {
        _jsonFilePath = jsonFilePath;
    }

    /// <summary>
    /// 从 JSON 文件加载配置。如果文件不存在或格式无效，返回 null。
    /// </summary>
    public PatternsContainer? LoadPatterns()
    {
        try
        {
            if (!File.Exists(_jsonFilePath))
                return null;

            string jsonContent = File.ReadAllText(_jsonFilePath);
            return JsonSerializer.Deserialize<PatternsContainer>(jsonContent, _serializerOptions);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"读取JSON文件时出错: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 保存配置到 JSON 文件。
    /// </summary>
    public void SavePatterns(PatternItem pattern)
    {
        var container = new PatternsContainer
        {
            Patterns = [pattern]
        };

        string jsonString = JsonSerializer.Serialize(container, _serializerOptions);
        File.WriteAllText(_jsonFilePath, jsonString);
    }

    /// <summary>
    /// 创建默认配置对象（不写文件，不更新 UI）。
    /// </summary>
    public static PatternItem CreateDefault()
    {
        return new PatternItem
        {
            TXTPath_textBoxPattern = "",
            Rule_textBoxPattern = "",
            Speed_toolStripComboBoxPattern = "0",
            ReadMode_toolStripComboBoxPattern = "整行",
            CurrentChapter_toolStripStatusLabelPattern = "",
            CurrentLine_toolStripStatusLabelPattern = "0",
            FontName = "Microsoft YaHei UI",
            FontStyle = 0,
            FontSize = 12.0,
            BlackColor = new BlackColor { R = 199, G = 237, B = 204, A = 255 },
            HotkeysForJson = new Dictionary<string, string>(PatternItem.DefaultHotkeys)
        };
    }

    public static string SerializeObject<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, _serializerOptions);
    }
}
