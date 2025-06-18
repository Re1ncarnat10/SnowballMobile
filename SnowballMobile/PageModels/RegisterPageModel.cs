namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Services;
using SnowballMobile.Models;

public partial class RegisterPageModel : ObservableObject
{
  private readonly ApiService _apiService;

  [ObservableProperty]
  private string email;

  [ObservableProperty]
  private string password;

  [ObservableProperty]
  private string confirmPassword;

  [ObservableProperty]
  private string errorMessage;

  public RegisterPageModel(ApiService apiService)
  {
    _apiService = apiService;
  }

  [RelayCommand]
  public async Task RegisterAsync()
  {
    var result = await _apiService.RegisterAsync(new RegisterDto { Email = Email, Password = Password, ConfirmPassword = ConfirmPassword });
    if (!result)
      ErrorMessage = "Rejestracja nie powiodła się";
  }
}