using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using Shared.Config;

namespace AsstDemo.ViewModels;

public class MainVM : ReactiveObject, IScreen
{
    public RoutingState Router { get; }
    
    public MainVM()
    {
        Router = new RoutingState();
        Log.Debug("MainVM created");
        
    }
}
