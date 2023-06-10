using System.Diagnostics;
using Automation;

namespace AsstCore;

/// <summary>
/// 辅助运行控制
/// </summary>
public class AsstRunControl
{
    /// <summary>
    /// 超时控制, 用于监控线程销毁
    /// </summary>
    public KeyValuePair<int, Stopwatch> TimeLimit = new(60, new Stopwatch());
    
    #region 主线程

    /// <summary>
    /// 用于控制主线程结束的 CancellationTokenSource
    /// </summary>
    public CancellationTokenSource MainCts = new CancellationTokenSource();
    /// <summary>
    /// 启动主线程返回的 Task, 代表了主线程
    /// </summary>
    public Task MainTask = Task.CompletedTask;
    /// <summary>
    /// 主线程的执行状态
    /// </summary>
    public RunStatus MainStatus = RunStatus.Stop;
    
    /// <summary>
    /// 主线程的 AutoPlugin
    /// </summary>
    public AutoPlugin MainAp  = new();
    #endregion


    #region 监控线程
    /// <summary>
    /// 用于控制监控线程结束的 CancellationTokenSource
    /// </summary>
    public CancellationTokenSource MonitorCts = new();
    public Task MonitorTask = Task.CompletedTask;
    
    /// <summary>
    /// 监控线程的 AutoPlugin
    /// </summary>
    public AutoPlugin MonitorAp  = new();
    #endregion

}

/// <summary>
/// 执行状态
/// </summary>
public enum RunStatus
{
    Stop,
    Running,
    Pause
}