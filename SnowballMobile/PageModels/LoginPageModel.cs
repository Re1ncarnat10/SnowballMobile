namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Services;
using SnowballMobile.Models;
using System.Diagnostics;
public partial class LoginPageModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginPageModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var result = await _apiService.LoginAsync(new LoginDto
            {
                Email = Email,
                Password = Password
            });

            if (!result)
            {
                ErrorMessage = "Invalid login or password";
            }
            else
            {
                var token = _apiService.Token;
                var userId = JwtHelper.GetUserIdFromToken(token);
                AppShell.CurrentUserId = userId;
                AppShell.CurrentUserName = Email;
                AppShell.RefreshUserName();
                await Shell.Current.GoToAsync("//main");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred during login: {ex.Message}";
            Debug.WriteLine($"Login error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}