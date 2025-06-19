using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SnowballMobile.PageModels;

public partial class MainPageModel : ObservableObject
{
  [RelayCommand]
  private async Task GoToRegisterAsync()
  {
    await Shell.Current.GoToAsync("//RegisterPage");
  }

  [RelayCommand]
  private async Task GoToLoginAsync()
  {
    await Shell.Current.GoToAsync("//LoginPage");
  }
  [ObservableProperty]
  bool isBusy;
}