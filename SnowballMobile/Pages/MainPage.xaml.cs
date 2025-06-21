namespace SnowballMobile.Pages;

public partial class MainPage : ContentPage
{
  public MainPage(MainPageModel viewModel)
  {

    InitializeComponent();
    BindingContext = viewModel;
  }
}