using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;

namespace SnowballMobile.PageModels;

public partial class ShopPageModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly List<SnowballDto> _allSnowballs = new();

    [ObservableProperty]
    private ObservableCollection<SnowballDto> snowballs = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public ShopPageModel(ApiService apiService)
    {
        _apiService = apiService;
        LoadSnowballsCommand = new AsyncRelayCommand(LoadSnowballsAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshSnowballsAsync);
        SnowballTappedCommand = new AsyncRelayCommand<SnowballDto>(OnSnowballTappedAsync);
        ApplyFilter();
    }

    public IAsyncRelayCommand LoadSnowballsCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand<SnowballDto> SnowballTappedCommand { get; }

    private async Task LoadSnowballsAsync()
    {
        IsBusy = true;
        try
        {
            var fetched = await _apiService.GetAllSnowballsAsync();
            _allSnowballs.Clear();
            _allSnowballs.AddRange(fetched);
            ApplyFilter();
        }
        catch (Exception ex)
        {
            await AppShell.DisplaySnackbarAsync($"Błąd: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RefreshSnowballsAsync()
    {
        await LoadSnowballsAsync();
    }

    private async Task OnSnowballTappedAsync(SnowballDto? snowball)
    {
        if (snowball == null) return;

        AddToCartPopup? popup = null;
        popup = new AddToCartPopup(new AddToCartPopupViewModel(
            snowball,
            async () =>
            {
                var userId = AppShell.CurrentUserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    var result = await _apiService.AddToCartAsync(userId, snowball.SnowballId);
                    await AppShell.DisplaySnackbarAsync(result ? "Dodano do koszyka!" : "Nie udało się dodać do koszyka.");
                }
            },
            () => popup?.Close()
        ));
        try
        {
            await Application.Current.MainPage.ShowPopupAsync(popup);
        }
        catch (Exception ex)
        {
            await AppShell.DisplaySnackbarAsync($"Błąd popupu: {ex.Message}");
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allSnowballs
            : _allSnowballs.Where(s => s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Snowballs.Clear();
        foreach (var snowball in filtered)
            Snowballs.Add(snowball);
    }
}