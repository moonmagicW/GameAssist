using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using AsstDemo.Models;
using AsstDemo.ViewModels.Index;
using ReactiveUI;
using Serilog;
using Splat;

namespace AsstDemo.ViewModels;

public class LeftMenuVM : ReactiveObject
{
    /// <summary>
    /// 主菜单的路由跳转
    /// </summary>
    public ReactiveCommand<MenuBar, Unit> MenuBarNavigateCommand { get; set; }

    public LeftMenuVM()
    {
        // 路由导航
        MenuBarNavigateCommand = ReactiveCommand.CreateFromObservable<MenuBar, Unit>(menuBar =>
            {
                if (Locator.Current.GetService(menuBar.Target) is IRoutableViewModel routableViewModel)
                    Locator.Current.GetService<MainVM>()?.Router.Navigate.Execute(routableViewModel);
                return Observable.Return(Unit.Default);
            }
        );
        Log.Debug("LeftMenuViewModel created");
    }

    

}