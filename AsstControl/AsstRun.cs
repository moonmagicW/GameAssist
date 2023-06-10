using AsstCore;
using Serilog;
using Serilog.Context;

namespace AsstControl;

public class AsstRun
{
    private readonly Asst _asst;
    private readonly CancellationToken _ct;

    public AsstRun(in Asst asst, CancellationToken ct)
    {
        _asst = asst;
        _ct = ct;
        // 日志添加指定属性
        LogContext.PushProperty(AsstDataHelper.LogProperty, $"Main {_asst.Id} ");
    }

    public async Task RunAsync()
    {
        // todo 启动监控线程
        
        // 取时间戳
        long ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // 无限循环
        while (!_ct.IsCancellationRequested)
        {
            Log.Information("{Time} 现在时间: {Now}", ts, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            // 等待指定时间
            await Task.Delay(2_000, _ct);
        }
        await Task.CompletedTask;
    }
}