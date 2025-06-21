using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Services;

namespace SnowballMobile.PageModels;

public partial class MainPageModel : ObservableObject
{

    private readonly ApiService _apiService;

    public MainPageModel(ApiService apiService)
    {
        _apiService = apiService;
    }
    [RelayCommand]
    public async Task TestConnectionAsync()
    {
        IsBusy = true;
        try
        {
            bool isConnected = await _apiService.TestConnectionAsync();
            if (isConnected)
                await Shell.Current.DisplayAlert("Połączenie", "Połączenie z backendem działa.", "OK");
            else
                await Shell.Current.DisplayAlert("Błąd", "Brak połączenia z backendem.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
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