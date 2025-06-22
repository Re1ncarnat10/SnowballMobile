using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
using System.Collections.ObjectModel;

namespace SnowballMobile.PageModels;

public partial class ShopPageModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<SnowballDto> Snowballs { get; set; } = new();

    public ShopPageModel(ApiService apiService)
    {
        _apiService = apiService;
        LoadSnowballsCommand = new AsyncRelayCommand(LoadSnowballsAsync);
    }

    public IAsyncRelayCommand LoadSnowballsCommand { get; }

    private async Task LoadSnowballsAsync()
    {
        IsBusy = true;
        try
        {
            var snowballs = await _apiService.GetAllSnowballsAsync();
            Snowballs.Clear();
            foreach (var snowball in snowballs)
            {
                System.Diagnostics.Debug.WriteLine($"SNOWBALL: {snowball.Name}, {snowball.Description}, {snowball.Price}");
                Snowballs.Add(snowball);
            }
        }
        catch (Exception ex)
        {
            await AppShell.DisplaySnackbarAsync($"Unexpected error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}