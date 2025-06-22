using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
using SnowballMobile.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

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
        SelectSnowballCommand = new AsyncRelayCommand<SelectionChangedEventArgs>(SelectSnowballAsync);
        ApplyFilter();
    }

    public IAsyncRelayCommand LoadSnowballsCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand<SelectionChangedEventArgs> SelectSnowballCommand { get; }

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

    private async Task SelectSnowballAsync(SelectionChangedEventArgs args)
    {
        var selected = args.CurrentSelection.FirstOrDefault() as SnowballDto;
        if (selected == null) return;
        await AppShell.DisplaySnackbarAsync($"Wybrano: {selected.Name}");
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