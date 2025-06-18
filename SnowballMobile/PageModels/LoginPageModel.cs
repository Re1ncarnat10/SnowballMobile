namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;
using Models;

public partial class LoginPageModel : ObservableObject
{
  private readonly ApiService _apiService;

  [ObservableProperty]
  private string email;

  [ObservableProperty]
  private string password;

  [ObservableProperty]
  private string errorMessage;

  public LoginPageModel(ApiService apiService)
  {
    _apiService = apiService;
  }

  [RelayCommand]
  public async Task LoginAsync()
  {
    var result = await _apiService.LoginAsync(new LoginDto { Email = Email, Password = Password });
    if (!result)
      ErrorMessage = "Błędny login lub hasło";
  }
}