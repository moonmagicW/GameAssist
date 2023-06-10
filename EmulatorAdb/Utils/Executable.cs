using CliWrap;
using System.Text;
using Serilog;

namespace EmulatorAdb.Utils;

/// <summary>
/// Process 调用可执行程序
/// </summary>
public class Executable
{
    /// <summary>
    /// 异步执行进程命令
    /// </summary>
    /// <param name="filePath">可执行文件全路径</param>
    /// <param name="arg">参数, 用字符串数组避免转义问题, 字符串形式执行参数转为每个空格为一个成员. </param>
    /// <param name="isResultValidation">对结果的验证, 某些情况下需要关闭, 用于抑制正确的"错误"</param>
    /// <returns>第一个值为正确执行返回值, 第二个值为错误值</returns>
    public static async Task<(string, string)> ExecProcess(string filePath, bool isResultValidation = true,
        params String[] arg)
    {
        // 设置 10 秒超时
        using var forcefulCts = new CancellationTokenSource();
        using var gracefulCts = new CancellationTokenSource();
        //超时10秒后强制取消
        forcefulCts.CancelAfter(TimeSpan.FromSeconds(10));
        //在 7 秒超时后优雅地取消
        //如果进程响应优雅取消的时间过长
        // 它最终会被上面配置的强制取消杀死
        gracefulCts.CancelAfter(TimeSpan.FromSeconds(7));
        
        
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();
        var command = Cli.Wrap(filePath).WithArguments(arg);
        if (!isResultValidation)
            command = command.WithValidation(CommandResultValidation.None);
        try
        {
            await command.WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync(forcefulCts.Token, gracefulCts.Token);
        }
        catch (OperationCanceledException e)
        {
            Log.Error(e,"ADB 命令超时取消");
        }

        return (stdOutBuffer.ToString(), stdErrBuffer.ToString());
    }
}