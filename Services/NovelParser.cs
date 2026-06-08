using System.Text;
using System.Text.RegularExpressions;

namespace ReadTXT;

/// <summary>
/// 小说文件解析器：读取 TXT 文件并按正则规则分章。
/// </summary>
public class NovelParser
{
    /// <summary>
    /// 根据编码和章节匹配规则解析小说文件，返回章节标题→内容的字典。
    /// </summary>
    /// <param name="filePath">小说文件路径</param>
    /// <param name="encoding">文件编码</param>
    /// <param name="rulePattern">章节标题匹配正则表达式（为空则不分章）</param>
    /// <returns>章节字典，key 为章节标题，value 为标题+正文</returns>
    public Dictionary<string, string> Parse(string filePath, Encoding encoding, string? rulePattern)
    {
        var chapters = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(filePath, encoding);

        if (!string.IsNullOrEmpty(rulePattern))
        {
            for (int j = 0; j < lines.Length;)
            {
                if (Regex.IsMatch(lines[j], rulePattern))
                {
                    string chapterTitle = lines[j];
                    string chapterContent = "";
                    j++;
                    int contentIndex = j;
                    while (contentIndex < lines.Length && !Regex.IsMatch(lines[contentIndex], rulePattern))
                    {
                        chapterContent += lines[contentIndex] + Environment.NewLine;
                        contentIndex++;
                    }
                    chapters[chapterTitle] = chapterTitle + Environment.NewLine + chapterContent;
                    j = contentIndex;
                }
                else
                {
                    j++;
                }
            }
        }

        return chapters;
    }

    /// <summary>
    /// 读取文件所有行（无规则匹配时使用）。
    /// </summary>
    public string ReadAllText(string filePath, Encoding encoding)
    {
        string[] lines = File.ReadAllLines(filePath, encoding);
        return string.Join(Environment.NewLine, lines);
    }
}
