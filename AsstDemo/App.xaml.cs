using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using AsstDemo.Config;
using AsstDemo.ViewModels;
using AsstDemo.ViewModels.Index;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Serilog;
using Shared.Config;
using Splat;

namespace AsstDemo;

public partial class App
{
    public App()
    {
        // 压抑 reactiveui 非必要的警告, 需要在最开始就调用
        Locator.CurrentMutable.Register(() => new CustomPropertyResolver(), typeof(ICreatesObservableForProperty));

        // 初始化配置
        AppsettingsHelper.Init();

        // 初始化日志
        ConfigureSerilog.Init();

        // 注册 ViewModel
        RegisterViewModels();
    }

    /// <summary>
    /// 注册 ViewModel
    /// </summary>
    private void RegisterViewModels()
    {
        
        // 注册主窗口
        Locator.CurrentMutable.RegisterLazySingletonAnd(() => new MainVM(), typeof(MainVM));

        // 注册主数据
        Locator.CurrentMutable.RegisterLazySingletonAnd(() => new AsstShowVM());

        
        // 注册左侧菜单
        Locator.CurrentMutable.RegisterLazySingletonAnd(() => new IndexVM());
        Locator.CurrentMutable.RegisterLazySingletonAnd(() => new SettingsVM());

        // 快速注册所有符合条件的 ViewModel
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
    }


    public class CustomPropertyResolver : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false) => 1;

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender,
            System.Linq.Expressions.Expression expression, string propertyName, bool beforeChanged = false,
            bool suppressWarnings = false)
        {
            return Observable.Return(new ObservedChange<object, object>(sender, expression, default),
                    RxApp.MainThreadScheduler)
                .Concat(Observable.Never<IObservedChange<object, object>>());
        }
    }

}