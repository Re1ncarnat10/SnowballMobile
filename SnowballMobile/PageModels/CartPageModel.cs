using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;
using Microsoft.Maui.Devices;

namespace SnowballMobile.PageModels;

public partial class CartPageModel : ObservableObject
{
    private readonly ApiService _apiService;
    public IAsyncRelayCommand<CartItemViewModel> RemoveFromCartCommand { get; }
    public IAsyncRelayCommand ClearCartCommand { get; }
    
    public IAsyncRelayCommand PlaceOrderCommand { get; }

    [ObservableProperty]
    private UserCartSummaryDto? _cartSummary;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<UserCartDto> _cartItems = new();

    [ObservableProperty]
    private ObservableCollection<CartItemViewModel> _cartDisplayItems = new();

    public CartPageModel(ApiService apiService)
    {
        _apiService = apiService;
        PlaceOrderCommand = new AsyncRelayCommand(PlaceOrderAsync, CanPlaceOrder);
        RemoveFromCartCommand = new AsyncRelayCommand<CartItemViewModel>(RemoveFromCartAsync);
        ClearCartCommand = new AsyncRelayCommand(ClearCartAsync);
    }
    private bool CanPlaceOrder()
        => CartSummary != null && CartSummary.Items.Count > 0;
    partial void OnCartSummaryChanged(UserCartSummaryDto? value)
    {
        PlaceOrderCommand.NotifyCanExecuteChanged();
    }
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
                if (Vibration.Default.IsSupported)
                {
                    try
                    {
                        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Vibration error: {ex.Message}");
                    }
                }
                await AppShell.DisplaySnackbarAsync("Zamówienie złożone!");
                await RefreshCartAsync();
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

    private async Task RemoveFromCartAsync(CartItemViewModel? item)
    {
        if (item == null || CartSummary == null) return;
        IsBusy = true;
        try
        {
            var userId = AppShell.CurrentUserId;
            System.Diagnostics.Debug.WriteLine($"[RemoveFromCartAsync] userId: {userId}, snowballId: {item.SnowballId}");
            if (userId != null)
            {
                await _apiService.RemoveFromCartAsync(userId, item.SnowballId);
                await RefreshCartAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ClearCartAsync()
    {
        if (CartSummary == null) return;
        IsBusy = true;
        try
        {
            var userId = AppShell.CurrentUserId;
            if (userId != null)
            {
                await _apiService.ClearCartAsync(userId);
                await RefreshCartAsync();
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

            CartDisplayItems = new ObservableCollection<CartItemViewModel>(
                CartSummary?.Items.Select(item => new CartItemViewModel
                {
                    SnowballId = item.SnowballId,
                    Name = item.Name,
                    Description = item.Description,
                    Image = string.IsNullOrWhiteSpace(item.Image) ? "cart.png" : item.Image,
                    Price = item.Price
                }) ?? new List<CartItemViewModel>());
        }
        else
        {
            CartSummary = null;
            CartDisplayItems.Clear();
        }
    }
    public class CartItemViewModel
    {
        public int SnowballId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = "cart.png";
        public decimal Price { get; set; }
    }
}
