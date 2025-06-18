namespace SnowballMobile.PageModels;

using CommunityToolkit.Mvvm.ComponentModel;
using SnowballMobile.Services;
using SnowballMobile.Models;

public partial class CartPageModel : ObservableObject
{
  private readonly ApiService _apiService;

  [ObservableProperty]
  private UserCartSummaryDto? cartSummary;

  public CartPageModel(ApiService apiService)
  {
    _apiService = apiService;
  }

  public async Task LoadCartAsync(string userId)
  {
    CartSummary = await _apiService.GetCartSummaryAsync(userId);
  }
}