using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;

namespace SnowballMobile.PageModels;

public partial class CartPageModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private UserCartSummaryDto? _cartSummary;
    [ObservableProperty]
    private bool _isBusy;
    [ObservableProperty]
    private ObservableCollection<UserCartDto> _cartItems = new();

    public CartPageModel(ApiService apiService)
    {
        _apiService = apiService;
        PlaceOrderCommand = new AsyncRelayCommand(PlaceOrderAsync, CanPlaceOrder);
    }

    public IAsyncRelayCommand PlaceOrderCommand { get; }

    private bool CanPlaceOrder()
        => CartSummary != null && CartSummary.Items.Count > 0;

    private async Task PlaceOrderAsync()
    {
        if (CartSummary == null) return;
        IsBusy = true;
        try
        {
            var userId = AppShell.CurrentUserId;
            if (string.IsNullOrEmpty(userId)) return;
            var result = await _apiService.PlaceOrderAsync(userId);
            if (result)
            {
                await AppShell.DisplaySnackbarAsync("Zamówienie złożone!");
                CartSummary = await _apiService.GetCartSummaryAsync(userId);
            }
            else
            {
                await AppShell.DisplaySnackbarAsync("Nie udało się złożyć zamówienia.");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RefreshCartAsync()
    {
        var userId = AppShell.CurrentUserId;
        if (!string.IsNullOrEmpty(userId))
        {
            CartSummary = await _apiService.GetCartSummaryAsync(userId);
        }
        else
        {
            CartSummary = null;
        }
    }
}