namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using SnowballMobile.Services;
using SnowballMobile.Models;
using System.Collections.ObjectModel;

public partial class OrdersPageModel : ObservableObject
{
  private readonly ApiService _apiService;

  [ObservableProperty]
  private ObservableCollection<OrderDto> orders = new();

  public OrdersPageModel(ApiService apiService)
  {
    _apiService = apiService;
  }

  public async Task LoadOrdersAsync()
  {
    var items = await _apiService.GetAllOrdersAsync();
    Orders = new ObservableCollection<OrderDto>(items);
  }
}