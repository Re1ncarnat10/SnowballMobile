namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using SnowballMobile.Services;
using SnowballMobile.Models;
using System.Collections.ObjectModel;

public partial class AdminPanelPageModel : ObservableObject
{
  private readonly ApiService _apiService;

  [ObservableProperty]
  private ObservableCollection<SnowballDto> snowballs = new();

  public AdminPanelPageModel(ApiService apiService)
  {
    _apiService = apiService;
  }

  public async Task LoadSnowballsAsync()
  {
    var items = await _apiService.GetAllSnowballsAsync();
    Snowballs = new ObservableCollection<SnowballDto>(items);
  }
}