using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AsstControl;
using AsstCore;
using AsstDemo.Helper;
using Automation;
using DynamicData;
using EmulatorAdb;
using EmulatorAdb.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace AsstDemo.ViewModels.Index;

public class AsstShowVM : ReactiveObject
{
    private readonly SourceCache<Asst, int> _asst;
    public readonly BindingList<Asst> Asst = new();

    public AsstShowVM()
    {
        _asst = AsstDataHelper.AsstData;
        _asst.Connect().RefCount()
            .Sort(Comparer<Asst>.Create((x, y) => x.Id.CompareTo(y.Id)))
            .SubscribeOn(RxApp.TaskpoolScheduler) // 在后台线程中执行数据操作
            .ObserveOn(RxApp.MainThreadScheduler) // 在主线程中更新 UI
            .Bind(Asst)
            .Subscribe();

        // 初始化 DataGrid 选择列
        InitColumnSelectBox();
        // 初始化右键菜单
        InitRightMenu();
        // 初始化模拟器信息
        Observable.FromAsync(async () => await InitEmulatorInfo())
            .SubscribeOn(RxApp.TaskpoolScheduler) // 在后台线程中执行取数据操作
            .ObserveOn(RxApp.MainThreadScheduler) // 在主线程中更新 UI
            .Subscribe();
        
        // 破解大漠插件
        Observable.FromAsync( () =>
            {
                Log.Information("开始初始化大漠插件");
                var autoPlugin = new AutoPlugin();
                return Task.CompletedTask;
            })
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Subscribe();
    }
    
    public SourceCache<Asst, int> GetAsst()
    {
        return _asst;
    }
    

    #region 模拟器

    /// <summary>
    /// 初始化模拟器信息
    /// </summary>
    private async Task InitEmulatorInfo()
    {
        Log.Information("开始初始化模拟器信息");
        try
        {
            var ldAdb = new LDAdb(new EmulatorInfo());
            if (string.IsNullOrEmpty(ldAdb.GetAdbPath()))
                throw new Exception("ADB 未配置");
            var allEmulatorInfo = await ldAdb.GetAllEmulatorInfo();
            foreach (var emulatorInfo in allEmulatorInfo)
            {
                _asst.AddOrUpdate(new Asst { Emulator = new LDAdb(emulatorInfo) });
            }

        }
        catch (Exception e)
        {
            Log.Error(e, "初始化模拟器信息失败, 请检查 ADB 是否正常运行");
            MessageBox.Show("模拟器ADB异常. 错误: " + e);
        }

        Log.Information("模拟器信息初始化完毕");
        
        // 预热, adb第一次会未启动, 直接卡死   
        var cts = new CancellationTokenSource();
        cts.CancelAfter(5000);
        Task.Factory.StartNew( () =>
        {
            _asst.Items.FirstOrDefault()?.Emulator.IsRunApp("");
        },cts.Token);
    }

    #endregion

    #region DataGrid 选择列

    [Reactive] public bool CheckBoxHeaderClick { get; set; }

    private bool _isCheckBoxHeaderClicked;

    public ReactiveCommand<Unit, Unit> CheckBoxHeaderCommand = null!;

    /// <summary>
    /// 初始化 DataGrid 的首列选择框
    /// </summary>
    private void InitColumnSelectBox()
    {
        // 单击每个 CheckBox 时将 CheckBoxHeader 置为 true/false
        _asst.Connect().RefCount()
            .AutoRefresh(x => x.IsSelected)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
                {
                    CheckBoxHeaderClick = _asst.Items.All(y => y.IsSelected);
                    _isCheckBoxHeaderClicked = false;
                }
            );
        // 单击 CheckBoxHeader 时触发 全选/全不选
        this.WhenAnyValue(x => x.CheckBoxHeaderClick)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                if (!_isCheckBoxHeaderClicked) return;
                foreach (var scriptData in _asst.Items)
                {
                    scriptData.IsSelected = x;
                }
            });
        // 解决全选时取消其中一个会导致全取消的问题
        CheckBoxHeaderCommand = ReactiveCommand.CreateFromTask(() =>
        {
            _isCheckBoxHeaderClicked = true;
            return Task.CompletedTask;
        });
    }
    #endregion
    
    

    #region 右键菜单

    public ReactiveCommand<Asst, Unit> RefreshCurrentRowCommand = null!;
    public ReactiveCommand<Unit, Unit> StartSelectedCommand = null!;
    public ReactiveCommand<Unit, Unit> StopSelectedCommand = null!;

    /// <summary>
    /// 初始化右键菜单
    /// </summary>
    private void InitRightMenu()
    {
        RefreshCurrentRowCommand = ReactiveCommand.CreateFromTask<Asst>(async data =>
        {
            // 仅更新模拟器信息
            data.Emulator.EmulatorInfo = await new LDAdb(new EmulatorInfo { Index = data.Id }).GetEmulatorInfo();
            _asst.AddOrUpdate(data);
        });

        // 启动选中的模拟器
        StartSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            foreach (var data in _asst.Items)
            {
                if (!data.IsSelected) continue;
                await AsstThreadControl.MainStartAsync(data);
            }
        });

        // 停止选中的模拟器
        StopSelectedCommand = ReactiveCommand.CreateFromTask(() =>
        {
            foreach (var data in _asst.Items)
            {
                if (!data.IsSelected) continue;
                AsstThreadControl.Stop(data);
            }

            return Task.CompletedTask;
        });
    }

    #endregion
}