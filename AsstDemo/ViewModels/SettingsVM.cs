using ReactiveUI;
using Serilog;

namespace AsstDemo.ViewModels;

public class SettingsVM : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment { get; } = "SettingsVM";
    public IScreen HostScreen { get; } = null!;

    public SettingsVM()
    {
        Log.Debug("SettingsVM created");
    }
}