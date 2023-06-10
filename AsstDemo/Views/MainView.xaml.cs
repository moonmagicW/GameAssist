using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using AsstDemo.Helper;
using AsstDemo.ViewModels;
using DynamicData;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Serilog.Context;
using Shared.Config;
using Splat;

namespace AsstDemo.Views;

public partial class MainView: ReactiveWindow<MainVM>
{    
    public MainView()
    {
        InitializeComponent();
        DataContext = ViewModel = Locator.Current.GetService<MainVM>();
        
        // 将消息弹窗的队列放到全局
        SnackbarHelper.MessageQueue = SnackbarThree.MessageQueue!;
        
        this.WhenActivated(d =>
        {
            // 绑定路由
            this.OneWayBind(ViewModel,
                    x => x.Router,
                    view => view.RoutedViewHost.Router)
                .DisposeWith(d);

        });

    }


}