using AsstCore.GameInfo;
using EmulatorAdb;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AsstCore;

/// <summary>
/// 核心数据类
/// </summary>
public class Asst : ReactiveObject
{
    /// <summary>
    /// 模拟器实例
    /// </summary>
    public IEmulatorAdb Emulator { get; set; } = null!;

    /// <summary>
    /// 主键, 取模拟器的 Index
    /// </summary>
    public int Id => Emulator.EmulatorInfo.Index;

    /// <summary>
    /// 是否选中
    /// </summary>
    [Reactive]
    public bool IsSelected { get; set; }

    /// <summary>
    /// 用于显示的运行日志
    /// </summary>
    [Reactive]
    public String Log { get; set; } = string.Empty;

    /// <summary>
    /// 脚本执行控制
    /// </summary>
    public AsstRunControl RunControl { get; set; } = new();

    
    
    #region 游戏相关数据

    /// <summary>
    /// 账号
    /// </summary>
    [Reactive] public Account Account { get; set; } = new();
    /// <summary>
    /// 任务详情
    /// </summary>
    public TaskDetails TaskDetails { get; set; } = new();
    public string AccountName => Account.Password;

    #endregion


    #region 通用方法封装

    /// <summary>
    /// 执行线程的延时函数, 在这里做一些额外的操作, 默认使用主线程的 CancellationTokenSource
    /// </summary>
    public async Task Delay(int ms)
    {
        // 可以做一些额外的操作, 例如暂停
        await Task.Delay(ms, RunControl.MainCts.Token);
    }

    /// <summary>
    /// 执行线程的延时函数, 在这里做一些额外的操作
    /// </summary>
    public async Task Delay(int ms, CancellationToken cancellationToken)
    {
        // 可以做一些额外的操作, 例如暂停
        await Task.Delay(ms, cancellationToken);
    }

    #endregion
}