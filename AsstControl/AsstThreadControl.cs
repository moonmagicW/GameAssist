using AsstCore;
using Serilog;
using Serilog.Context;

namespace AsstControl;

public static class AsstThreadControl
{
    /// <summary>
    /// 启动指定的辅助主线程
    /// </summary>
    /// <param name="asst"></param>
    public static async Task MainStartAsync(Asst asst)
    {
        AsstRunControl rc = asst.RunControl;
        if (rc.MainStatus == RunStatus.Stop)
        {
            rc.MainStatus = RunStatus.Running;

            // 等待旧任务完成或取消
            if (!rc.MainCts.IsCancellationRequested)
                rc.MainCts.Cancel();
            await Task.WhenAny(rc.MainTask, Task.Delay(Timeout.Infinite, rc.MainCts.Token));
            rc.MainCts = new CancellationTokenSource();

            Start(ref rc.MainCts, ref rc.MainTask, $"Main {asst.Id} ",
                () => new AsstRun(asst, rc.MainCts.Token).RunAsync());
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="asst"></param>
    public static async Task MonitorStartAsync(Asst asst)
    {
        AsstRunControl rc = asst.RunControl;
    
        // 等待旧任务完成或取消
        if (!rc.MonitorCts.IsCancellationRequested)
            rc.MonitorCts.Cancel();
        await Task.WhenAny(rc.MonitorTask, Task.Delay(Timeout.Infinite, rc.MonitorCts.Token));
        rc.MonitorCts = new CancellationTokenSource();
    
        Start(ref rc.MonitorCts, ref rc.MonitorTask, $"Monitor {asst.Id} ", () => new MonitorRun(asst, rc.MonitorCts.Token).RunAsync());
    }

    /// <summary>
    /// 启动线程
    /// </summary>
    // ReSharper disable once RedundantAssignment
    private static void Start(ref CancellationTokenSource cts, ref Task task, string logProp, Func<Task> run)
    {
        // 开启线程执行新的任务
        task = Task.Factory.StartNew(async () =>
        {
            using (LogContext.PushProperty(AsstDataHelper.LogProperty, logProp))
                Log.Information("启动线程");
            await run();
        }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        // 注册取消事件
        cts.Token.Register(mTask =>
        {
            if (mTask is not Task mTask1) return;
            using (LogContext.PushProperty(AsstDataHelper.LogProperty, logProp))
                Log.Information("关闭线程");

            // 如果CancellationTokens已经被取消，则尝试取消任务
            if (mTask1.Status is TaskStatus.Running or TaskStatus.RanToCompletion)
            {
                mTask1.Wait(2500);
                if (mTask1.Status is TaskStatus.RanToCompletion or TaskStatus.Faulted or TaskStatus.Canceled)
                    mTask1.Dispose();
            }
        }, task);
    }

    /// <summary>
    /// 停止指定的辅助线程
    /// </summary>
    public static void Stop(Asst asst)
    {
        AsstRunControl rc = asst.RunControl;
        if (rc.MainStatus == RunStatus.Stop) return;
        // 通知任务执行线程停止
        rc.MainStatus = RunStatus.Stop;
        rc.MainCts.Cancel();
        rc.MonitorCts.Cancel();
    }

    /// <summary>
    /// 重启指定的辅助线程
    /// </summary>
    public static async Task Reboot(Asst asst)
    {
        Stop(asst);
        Thread.Sleep(2_000);
        await MainStartAsync(asst);
    }
}