namespace ReadTXT;

/// <summary>
/// 迷你模式窗口与主窗体之间的通信接口。
/// </summary>
public interface IMiniModeHost
{
    bool GetSpeakingStatus();
    string GetCurrentChapter();
    string GetCurrentReadingText();
    void StartReadingFromMini();
    void StopReadingFromMini();
    void NextChapterFromMini();
    void SwitchToMainFromMini();
    void LogStatus(string message);
}
