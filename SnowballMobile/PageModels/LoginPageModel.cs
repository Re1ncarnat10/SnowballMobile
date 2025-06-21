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
            Debug.WriteLine($"Payload: Email={Email}, Password={Password}");

            var result = await _apiService.LoginAsync(new LoginDto
            {
                Email = Email,
                Password = Password
            });

            if (!result)
            {
                ErrorMessage = "Invalid login or password";
                Debug.WriteLine("Login failed.");
            }
            else
            {
                Debug.WriteLine("Login successful.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception during login: {ex.Message}");
            ErrorMessage = "An error occurred during login.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}