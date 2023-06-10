namespace EmulatorAdb.Model;

public class EmulatorInfo
{
    public int Index { get; set; }
    public string? Title { get; set; }

    /// <summary>
    /// 主窗口句柄
    /// </summary>
    public int MainWindowHandle { get; set; }

    /// <summary>
    /// 渲染窗口句柄
    /// </summary>
    public int RenderWindowHandle { get; set; }

    /// <summary>
    /// 是否进入安卓, 0:未进入, 1:进入, 2:启动中(雷电9有效)
    /// </summary>
    public int Running { get; set; }

    /// <summary>
    /// 主进程 ID
    /// </summary>
    public int Pid { get; set; }

    /// <summary>
    /// 对应 VirtualBox 进程 ID, 内存操作用此ID
    /// </summary>
    public int VBoxPid { get; set; }

}