namespace SnowballMobile.Pages;

public partial class ShopPage : ContentPage
{
  public ShopPage(ShopPageModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
    viewModel.LoadSnowballsCommand.Execute(null);
  }
}