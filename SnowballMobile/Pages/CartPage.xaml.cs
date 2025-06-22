namespace SnowballMobile.Pages;

public partial class CartPage : ContentPage
{
  public CartPage(CartPageModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
  }
  protected override async void OnAppearing()
  {
    base.OnAppearing();
    if (BindingContext is CartPageModel vm)
    {
      await vm.RefreshCartAsync();
    }
  }
}