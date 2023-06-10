using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Media.TextFormatting;
using AsstCore;
using DynamicData;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Splat;
using Splat.Serilog;

namespace AsstDemo.Config;
public static class ConfigureSerilog
{
    /// <summary>
    /// 初始化日志
    /// </summary>
    public static void Init()
    {
        Log.Logger = BaseConfiguration().CreateLogger();
        Locator.CurrentMutable.UseSerilogFullLogger();
    }


    /// <summary>
    /// 基础日志配置
    /// </summary>
    /// <returns></returns>
    private static LoggerConfiguration BaseConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(@"Config/serilog.json")
            .Build();
        return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // 从配置文件读取基础配置
                // 自定义配置
                .Enrich.WithProperty(AsstDataHelper.LogProperty, string.Empty)
                .WriteTo.Async(w => w.Sink(new AsstSink(AsstDataHelper.AsstData)));
    }
}

/// <summary>
/// 将日志记录到 <see cref="AsstDataHelper.AsstData"/> , 这会显示在界面上
/// </summary>
public class AsstSink : ILogEventSink
{
    private readonly SourceCache<Asst, int> _asst;

    public AsstSink(SourceCache<Asst, int> asst)
    {
        _asst = asst;
    }

    private const string AsstEmpty = "\"\"";
    private const string Main = "\"Main ";

    readonly MessageTemplateTextFormatter formatter = new("[{Timestamp:HH:mm:ss}] {Message:lj}");

    public void Emit(LogEvent logEvent)
    {
        // 只有 Information 以上的日志才会记录
        if (logEvent.Level < LogEventLevel.Information)
            return;

        string asstLog = logEvent.Properties[AsstDataHelper.LogProperty].ToString();
        if (asstLog != AsstEmpty && asstLog.StartsWith(Main))
        {
            // 删除前缀, 尾部会固定带 " 这个双引号, 去除一个字符, 然后转为数字拿到主键
            if (int.TryParse(asstLog[Main.Length..^1], out int id))
            {
                // _asst 取出对应主键的数据
                var asst = _asst.Lookup(id);
                if (asst.HasValue)
                {
                    using var writer = new StringWriter();
                    formatter.Format(logEvent, writer);
                    asst.Value.Log = writer.ToString();
                }
            }
        }
    }
}