namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
using SnowballMobile.Services;

public partial class RegisterPageModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public RegisterPageModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public bool IsFormValid =>
        !string.IsNullOrWhiteSpace(Name)
        && !string.IsNullOrWhiteSpace(Email)
        && !string.IsNullOrWhiteSpace(Password)
        && !string.IsNullOrWhiteSpace(ConfirmPassword)
        && Password == ConfirmPassword;

    partial void OnNameChanged(string value) => OnPropertyChanged(nameof(IsFormValid));
    partial void OnEmailChanged(string value) => OnPropertyChanged(nameof(IsFormValid));
    partial void OnPasswordChanged(string value) => OnPropertyChanged(nameof(IsFormValid));
    partial void OnConfirmPasswordChanged(string value) => OnPropertyChanged(nameof(IsFormValid));

    [RelayCommand]
    public async Task RegisterAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            ErrorMessage = string.Empty;

            var (success, errorMessage) = await _apiService.RegisterAsync(new RegisterDto
            {
                Name = Name,
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword
            });

            if (success)
            {
                await AppShell.DisplaySnackbarAsync("Rejestracja zakończona sukcesem!");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                ErrorMessage = errorMessage ?? "Wystąpił nieznany błąd podczas rejestracji.";
                await AppShell.DisplaySnackbarAsync(ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Błąd: {ex.Message}";
            await AppShell.DisplaySnackbarAsync(ErrorMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }
}