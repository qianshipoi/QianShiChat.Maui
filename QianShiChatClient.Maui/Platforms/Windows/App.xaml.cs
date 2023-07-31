// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Xaml;

namespace QianShiChatClient.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }


    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    static Mutex __SingleMutex;
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (!IsSingleInstance())
        {
            Environment.Exit(0);
            return;
        }
        base.OnLaunched(args);
    }

    static bool IsSingleInstance()
    {
        const string applicationId = "95E0F24B-E738-454D-847F-4A20FB601832";
        __SingleMutex = new Mutex(false, applicationId);
        GC.KeepAlive(__SingleMutex);

        try
        {
            return __SingleMutex.WaitOne(0, false);
        }
        catch (Exception)
        {
            __SingleMutex.ReleaseMutex();
            return __SingleMutex.WaitOne(0, false);
        }
    }

}