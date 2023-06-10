using System.Reactive.Disposables;
using AsstDemo.ViewModels.Index;
using ReactiveUI;
using Splat;

namespace AsstDemo.Views.Index;

public partial class AsstShowView : ReactiveUserControl<AsstShowVM>
{
    public AsstShowView()
    {
        InitializeComponent();
        ViewModel = Locator.Current.GetService<AsstShowVM>();
        this.WhenActivated(d =>
        {
            // 数据绑定
            this.OneWayBind(ViewModel, vm => vm.Asst, v => v.DataShow.ItemsSource)
                .DisposeWith(d);

            #region DataGrid 选择列

            // 绑定标题栏选择框全选/全不选
            this.Bind(ViewModel, vm => vm.CheckBoxHeaderClick, v => v.CheckBoxHeader.IsChecked)
                .DisposeWith(d);
            // 点击事件
            this.BindCommand(ViewModel, vm => vm.CheckBoxHeaderCommand, v => v.CheckBoxHeader)
                .DisposeWith(d);

            #endregion


            #region 右键菜单

            // 刷新当前行
            this.BindCommand(ViewModel, vm => vm.RefreshCurrentRowCommand,view => view.RefreshCurrentRow,
                    this.WhenAnyValue(x => x.DataShow.SelectedItem))
                .DisposeWith(d);

            // 启动已选项
            this.BindCommand(ViewModel, viewModel => viewModel.StartSelectedCommand, view => view.StartSelected)
                .DisposeWith(d);

            // 停止已选项
            this.BindCommand(ViewModel, viewModel => viewModel.StopSelectedCommand, view => view.StopSelected)
                .DisposeWith(d);

            #endregion
        });
    }
}