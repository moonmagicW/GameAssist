using System.ComponentModel;
using DynamicData;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Shared.Config;

/// <summary>
/// Appsettings 配置文件绑定的类
/// </summary>
public class Appsettings: ReactiveObject
{
    [Reactive]
    public int AsstIndex { get; set; }
    [Reactive]
    public SourceList<string> AsstList { get; set; } = new();
}