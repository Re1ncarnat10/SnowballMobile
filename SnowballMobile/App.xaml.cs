using SnowballMobile.Services;

namespace SnowballMobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var apiService = new ApiService();
        return new Window(new AppShell(apiService));
    }
}