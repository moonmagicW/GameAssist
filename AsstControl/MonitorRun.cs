using AsstCore;
using Automation;
using Serilog.Context;

namespace AsstControl;

public class MonitorRun
{
    private readonly Asst _asst;
    private readonly CancellationToken _ct;
    /// <summary>
    /// 自动化插件
    /// </summary>
    private readonly AutoPlugin ap;
    public MonitorRun(in Asst asst, CancellationToken ct)
    {
        _asst = asst;
        _ct = ct;
        ap = asst.RunControl.MonitorAp;
        
        // 日志添加指定属性
        LogContext.PushProperty(AsstDataHelper.LogProperty, $"Monitor {_asst.Id} ");
    }
    public async Task RunAsync()
    {
        await Task.CompletedTask;
    }
}