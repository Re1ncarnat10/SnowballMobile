using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace SnowballMobile;

public partial class AppShell : Shell
{
    public static string? CurrentUserName { get; set; }
    public static event Action? UserNameChanged;

    public AppShell()
    {
        InitializeComponent();
        var currentTheme = Application.Current!.RequestedTheme;
        ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;

        SetFlyoutHeaderSafeArea();
        UpdateUserName();
    }

    private void SetFlyoutHeaderSafeArea()
    {
#if ANDROID
        var statusBarHeight = 24;
        if (Platform.CurrentActivity is not null)
        {
            var resourceId = Platform.CurrentActivity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
                statusBarHeight = Platform.CurrentActivity.Resources.GetDimensionPixelSize(resourceId);
        }
        FlyoutHeaderGrid.Padding = new Thickness(0, statusBarHeight / Platform.CurrentActivity.Resources.DisplayMetrics.Density, 0, 0);
#else
        FlyoutHeaderGrid.Padding = new Thickness(0, 0, 0, 0);
#endif
    }

    public static void RefreshUserName()
    {
        if (Application.Current?.MainPage is AppShell shell)
        {
            shell.UpdateUserName();
        }
        UserNameChanged?.Invoke();
    }
    private void UpdateUserName()
    {
        UserNameLabel.Text = CurrentUserName ?? "Guest";
    }

    public static async Task DisplaySnackbarAsync(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#FF3300"),
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = new CornerRadius(0),
            Font = Font.SystemFontOfSize(18),
            ActionButtonFont = Font.SystemFontOfSize(14)
        };

        var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

        await snackbar.Show(cancellationTokenSource.Token);
    }

    public static async Task DisplayToastAsync(string message)
    {
        if (OperatingSystem.IsWindows())
            return;

        var toast = Toast.Make(message, textSize: 18);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await toast.Show(cts.Token);
    }

    private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
    {
        Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
    }
}