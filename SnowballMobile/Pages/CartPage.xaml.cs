namespace SnowballMobile.Pages;

public partial class CartPage : ContentPage
{
  public CartPage(CartPageModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
  }
}