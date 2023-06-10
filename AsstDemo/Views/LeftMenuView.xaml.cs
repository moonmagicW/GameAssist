using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AsstDemo.Models;
using AsstDemo.ViewModels;
using AsstDemo.ViewModels.Index;
using ReactiveUI;

namespace AsstDemo.Views;

public partial class LeftMenuView : ReactiveUserControl<LeftMenuVM>
{
    public LeftMenuView()
    {
        InitializeComponent();
        // ViewModel = Locator.Current.GetService<LeftMenuVM>()!;

        DataContext = ViewModel = new LeftMenuVM();
        // 绑定菜单项目
        CreateMenuBar();
        MenuBarListBox.ItemsSource = MenuBarList;


        this.WhenActivated(d =>
        {
            // 绑定导航事件
            this.BindCommand(ViewModel,
                viewModel => viewModel.MenuBarNavigateCommand,
                view => view.MenuBarListBox,
                this.WhenAnyValue(view => view.MenuBarListBox.SelectedItem)
            ).DisposeWith(d);
            
            // 转到主页
            ViewModel.MenuBarNavigateCommand.Execute(MenuBarList[0]);
        });



    }

    /// <summary>
    /// 主菜单列表
    /// </summary>
    private List<MenuBar> MenuBarList { get; set; } = new();

    /// <summary>
    /// 初始化左侧主菜单
    /// </summary>
    void CreateMenuBar()
    {
        MenuBarList.Add(new MenuBar("FolderNetworkOutline", "主页", typeof(IndexVM)));
        MenuBarList.Add(new MenuBar("CogOutline", "设置", typeof(SettingsVM)));
    }
}