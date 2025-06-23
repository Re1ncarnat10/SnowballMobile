namespace SnowballMobile.Pages;

public partial class OrderPage : ContentPage
{
  public OrderPage(OrdersPageModel viewModel)
  {
    InitializeComponent();
    BindingContext = viewModel;
    Loaded += async (_, _) => await viewModel.LoadOrdersAsync();
  }
  
}