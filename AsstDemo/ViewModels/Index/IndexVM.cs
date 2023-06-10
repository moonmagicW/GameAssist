using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AsstControl;
using AsstCore;
using AsstDemo.Helper;
using DynamicData;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Splat;

namespace AsstDemo.ViewModels.Index;

public class IndexVM : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment { get; } = "IndexVM";
    public IScreen HostScreen { get; } = null!;

    private readonly SourceCache<Asst, int> _asst;
    public IndexVM()
    {
        _asst = AsstDataHelper.AsstData;
        InitAsstControl();
    }



    #region 辅助执行线程控制

    /// <summary>
    /// 启动所有正在运行的脚本线程
    /// </summary>
    public ReactiveCommand<Unit, Unit> StartAllCommand { get; set; } = null!;

    /// <summary>
    /// 停止所有正在运行的脚本线程
    /// </summary>
    public ReactiveCommand<Unit, Unit> StopAllCommand { get; set; } = null!;


    /// <summary>
    /// 初始化辅助执行线程控制
    /// </summary>
    private void InitAsstControl()
    {
        StartAllCommand = ReactiveCommand.CreateFromTask(async _  =>
        {
            SnackbarHelper.MessageQueue.Enqueue("全部启动");
            foreach (var asst in _asst.Items)
            {
                asst.IsSelected = true; // 勾选上
                await AsstThreadControl.MainStartAsync(asst);
            }
            await Task.CompletedTask;
        });
        StopAllCommand = ReactiveCommand.CreateFromTask(async _  =>
        {
            SnackbarHelper.MessageQueue.Enqueue("全部停止");
            foreach (var asst in _asst.Items)
            {
                AsstThreadControl.Stop(asst);
            }
            await Task.CompletedTask;
        });
    }

    #endregion
}