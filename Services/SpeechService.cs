using System.Speech.Synthesis;
using Timer = System.Windows.Forms.Timer;

namespace ReadTXT;

/// <summary>
/// 朗读引擎服务：管理 SpeechSynthesizer 生命周期、朗读状态和行级朗读流程。
/// UI 相关操作通过回调委托桥接。
/// </summary>
public class SpeechService : IDisposable
{
    private readonly SpeechSynthesizer _synthesizer = new();
    private readonly Timer _completionTimer;
    private bool _disposed;

    // 朗读状态
    public bool IsSpeaking { get; private set; }
    public bool IsResuming { get; set; }
    public int CurrentLineIndex { get; set; }
    public int PauseLineIndex { get; private set; } = -1;
    public int Rate
    {
        get => _synthesizer.Rate;
        set => _synthesizer.Rate = value;
    }

    // 内部标志
    private bool _waitForRealCompletion;

    // UI 桥接回调
    public Func<int, string?>? GetLineText;       // 获取指定行的文本
    public Func<int>? GetTotalLineCount;           // 获取总行数
    public Func<bool>? ShouldContinueToNext;       // 是否读完本章后自动进入下一章
    public Func<bool>? GoToNextChapter;            // 尝试进入下一章，返回是否成功
    public Action<int>? OnLineChanged;             // 当前行改变
    public Action? OnHighlightRequest;             // 请求高亮当前行
    public Action? OnChapterCompleted;             // 章节读完
    public Action? OnAllCompleted;                 // 全书读完
    public Action<string>? OnLogMessage;           // 日志消息

    public SpeechService()
    {
        _synthesizer.SetOutputToDefaultAudioDevice();
        _synthesizer.SpeakStarted += (_, _) => _waitForRealCompletion = true;
        _synthesizer.SpeakProgress += (_, e) =>
            OnLogMessage?.Invoke($"朗读进度: '{e.Text}' (位置: {e.CharacterPosition})");
        _synthesizer.SpeakCompleted += (_, _) =>
        {
            if (_waitForRealCompletion && IsSpeaking)
            {
                _waitForRealCompletion = false;
                ProcessLineCompletion();
            }
            else if (!IsSpeaking && PauseLineIndex >= 0)
            {
                OnHighlightRequest?.Invoke();
            }
        };

        _completionTimer = new Timer { Interval = 10000 };
        _completionTimer.Tick += (_, _) =>
        {
            _completionTimer.Stop();
            if (_waitForRealCompletion && IsSpeaking)
            {
                OnLogMessage?.Invoke("朗读超时，强制继续下一行");
                _waitForRealCompletion = false;
                ProcessLineCompletion();
            }
        };
    }

    /// <summary>
    /// 开始朗读（从当前行继续或从头开始）。
    /// </summary>
    public void StartRead()
    {
        if (IsSpeaking) return;

        IsSpeaking = true;
        _waitForRealCompletion = false;

        if (PauseLineIndex >= 0)
        {
            CurrentLineIndex = PauseLineIndex;
            IsResuming = true;
            PauseLineIndex = -1;
            OnLogMessage?.Invoke($"从暂停位置继续：第{CurrentLineIndex}行");
        }
        else if (!IsResuming)
        {
            OnLogMessage?.Invoke($"从保存位置开始：第{CurrentLineIndex}行");
        }

        Task.Delay(100).ContinueWith(_ =>
        {
            // 需要在 UI 线程上回调高亮和启动
            OnHighlightRequest?.Invoke();
            // 延迟确保高亮完成后再开始朗读
            Task.Delay(50).ContinueWith(__ =>
            {
                IsResuming = false;
                ReadCurrentLine();
            });
        });
    }

    /// <summary>
    /// 从头开始朗读（重置行号为 0）。
    /// </summary>
    public void StartReadFromBeginning()
    {
        PauseLineIndex = -1;
        IsResuming = false;
        CurrentLineIndex = 0;
        OnLineChanged?.Invoke(0);
        StartRead();
    }

    /// <summary>
    /// 停止/暂停朗读。
    /// </summary>
    public void StopRead()
    {
        int totalLines = GetTotalLineCount?.Invoke() ?? 0;
        if (IsSpeaking && CurrentLineIndex < totalLines)
        {
            PauseLineIndex = CurrentLineIndex;
            OnLogMessage?.Invoke($"已暂停，当前位置：第{PauseLineIndex}行");
        }

        IsSpeaking = false;
        _waitForRealCompletion = false;
        _completionTimer.Stop();
        _synthesizer.SpeakAsyncCancelAll();
    }

    /// <summary>
    /// 选择语音包。
    /// </summary>
    public void SelectVoice(string voiceName)
    {
        _synthesizer.SelectVoice(voiceName);
    }

    /// <summary>
    /// 获取已安装的语音包信息。
    /// </summary>
    public IReadOnlyList<InstalledVoice> GetInstalledVoices()
    {
        return _synthesizer.GetInstalledVoices();
    }

    private void ReadCurrentLine()
    {
        if (!IsSpeaking) return;

        _waitForRealCompletion = false;
        _completionTimer.Stop();
        _completionTimer.Start();

        int totalLines = GetTotalLineCount?.Invoke() ?? 0;
        if (CurrentLineIndex >= totalLines)
        {
            HandleChapterCompleted();
            return;
        }

        string? lineText = GetLineText?.Invoke(CurrentLineIndex);

        if (string.IsNullOrWhiteSpace(lineText))
        {
            CurrentLineIndex++;
            OnLineChanged?.Invoke(CurrentLineIndex);
            Task.Delay(100).ContinueWith(_ => ReadCurrentLine());
            return;
        }

        OnHighlightRequest?.Invoke();

        try
        {
            _synthesizer.SpeakAsync(new Prompt(lineText));
            OnLogMessage?.Invoke($"开始朗读第{CurrentLineIndex + 1}行，字符数：{lineText.Length}");
        }
        catch (Exception ex)
        {
            OnLogMessage?.Invoke($"朗读出错: {ex.Message}");
            ProcessLineCompletion();
        }
    }

    private void ProcessLineCompletion()
    {
        _completionTimer.Stop();
        if (!IsSpeaking) return;

        OnLogMessage?.Invoke($"完成第{CurrentLineIndex + 1}行的朗读");

        int totalLines = GetTotalLineCount?.Invoke() ?? 0;
        bool isLastLine = CurrentLineIndex >= totalLines - 1;

        if (isLastLine)
        {
            HandleChapterCompleted();
        }
        else
        {
            CurrentLineIndex++;
            OnLineChanged?.Invoke(CurrentLineIndex);
            Task.Delay(300).ContinueWith(_ =>
            {
                OnHighlightRequest?.Invoke();
                ReadCurrentLine();
            });
        }
    }

    private void HandleChapterCompleted()
    {
        bool continueToNext = ShouldContinueToNext?.Invoke() ?? false;
        if (continueToNext)
        {
            bool hasNext = GoToNextChapter?.Invoke() ?? false;
            if (hasNext)
            {
                Task.Delay(300).ContinueWith(_ =>
                {
                    IsSpeaking = false;
                    _waitForRealCompletion = false;
                    _completionTimer.Stop();
                    StartReadFromBeginning();
                });
            }
            else
            {
                IsSpeaking = false;
                OnAllCompleted?.Invoke();
            }
        }
        else
        {
            IsSpeaking = false;
            OnChapterCompleted?.Invoke();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _completionTimer.Stop();
            _completionTimer.Dispose();
            _synthesizer.Dispose();
        }
    }
}
