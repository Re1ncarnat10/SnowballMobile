using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Font = Microsoft.Maui.Font;
using System.Windows.Input;
using SnowballMobile.Models;
namespace SnowballMobile;

public partial class AppShell : Shell, INotifyPropertyChanged
{
    private string _currentUserName = "Guest";
    public string CurrentUserName
    {
        get => _currentUserName;
        set
        {
            if (_currentUserName != value)
            {
                _currentUserName = value;
                OnPropertyChanged();
            }
        }
    }

    public static AppShell Instance { get; private set; }
    public ICommand LogoutCommand { get; }

    public static string? CurrentUserId { get; set; }

    internal readonly ApiService _apiService;
    public AppShell(ApiService apiService)
    {
        _apiService = apiService;
        InitializeComponent();
        Instance = this;
        BindingContext = this; 
        LogoutCommand = new Command(Logout);
        var currentTheme = Application.Current!.RequestedTheme;
        ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
        SetFlyoutHeaderSafeArea();
        UpdateUserName();
    }
    private async void Logout()
    {
        SetUser(null, null);
        await DisplaySnackbarAsync("Wylogowano.");
        await Shell.Current.GoToAsync("//LoginPage");
    }
    public static void SetUser(string? userName, string? userId)
    {
        Instance.CurrentUserName = string.IsNullOrEmpty(userName) ? "Guest" : userName;
        CurrentUserId = userId;
        var token = Instance._apiService?.Token;

        // Sprawdzenie roli admina na podstawie tokena
        Instance.IsAdmin = JwtHelper.HasRole(token, "Admin");

        Instance.UpdateUserName();
        Instance.OnPropertyChanged(nameof(IsAdmin));
        Instance.BindingContext = null;
        Instance.BindingContext = Instance;
    }

    public void UpdateAdminState(string token)
    {
        IsAdmin = JwtHelper.HasRole(token, "Admin");
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
    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set
        {
            if (_isAdmin != value)
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}