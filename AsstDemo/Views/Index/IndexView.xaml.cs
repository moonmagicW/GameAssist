using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using AsstDemo.ViewModels.Index;
using ReactiveUI;

namespace AsstDemo.Views.Index;

public partial class IndexView: ReactiveUserControl<IndexVM>
{
    public IndexView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel,vm => vm.StartAllCommand,v => v.StartAllButton)
                .DisposeWith(d);

            this.BindCommand(ViewModel,vm => vm.StopAllCommand,v => v.StopAllButton)
                .DisposeWith(d);
        });
    }
}