using System.Reactive.Linq;
using DynamicData.Binding;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace Shared.Config;

public static class AppsettingsHelper
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public static IConfigurationRoot? config { get; set; }

    /// <summary>
    /// Appsettings 实例
    /// </summary>
    public static Appsettings Appsettings { get; } = new();

    private static int _initCount = 0;

    /// <summary>
    /// 仅允许一次初始化
    /// </summary>
    public static void Init()
    {
        if (Interlocked.Increment(ref _initCount) > 1)
            return;

        config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        config.Bind(Appsettings); // 绑定

        // 开始对 Appsettings 进行订阅, 这里仅为了更新配置文件
        Appsettings.WhenAnyPropertyChanged().SubscribeOn(RxApp.TaskpoolScheduler)
            .Throttle(TimeSpan.FromSeconds(5)) // 控制频率, 5 秒内只会触发一次
            .Subscribe(_ =>
            {
                // 在这里应该对配置文件进行保存
                Log.Information("Appsettings changed");
            });
        // 这个是专门针对 SourceList 的订阅, 必须要这样写才会触发通知
        Appsettings.AsstList.Connect().SubscribeOn(RxApp.TaskpoolScheduler)
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(_ =>
            {
                // 在这里应该对配置文件进行保存
                Log.Information("Appsettings AsstList changed");
            });
    }
}