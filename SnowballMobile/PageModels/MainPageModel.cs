using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
namespace SnowballMobile.PageModels;

public partial class MainPageModel : ObservableObject
{
    private readonly ApiService _apiService;

    public MainPageModel(ApiService apiService)
    {
        _apiService = apiService;
        UpdateLoginState();
    }
    private bool _isLoggedIn;
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }
    [ObservableProperty]
    private string token;

    [ObservableProperty]
    private string userRole;
    public void UpdateLoginState()
    {
        IsLoggedIn = !string.IsNullOrEmpty(AppShell.Instance.CurrentUserName) && AppShell.Instance.CurrentUserName != "Guest";
        Token = _apiService.Token ?? string.Empty;
        if (!string.IsNullOrEmpty(Token))
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Token);
            UserRole = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Brak";
        }
        else
        {
            UserRole = "Brak";
        }
    }
    [ObservableProperty]
    private string _connectionStatus = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    // Commands
    [RelayCommand]
    private async Task LogoutAsync()
    {
        AppShell.SetUser(null, null);
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    public async Task TestConnectionAsync()
    {
        IsBusy = true;
        try
        {
            var response = await _apiService.TestConnectionAsync();
            ConnectionStatus = response ? "Connection successful." : "Connection failed. Please check the logs.";
        }
        catch (HttpRequestException httpEx)
        {
            ConnectionStatus = $"HTTP Error: {httpEx.Message}";
            if (httpEx.InnerException != null)
            {
                ConnectionStatus += $" Inner Exception: {httpEx.InnerException.Message}";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavToRegister()
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }

    [RelayCommand]
    private async Task NavToLogin()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}