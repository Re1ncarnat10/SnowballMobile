namespace SnowballMobile.Pages;

public partial class MainPage : ContentPage
{
  public MainPage(MainPageModel viewModel)
  {

    InitializeComponent();
    BindingContext = viewModel;
  }
  protected override void OnAppearing()
  {
    base.OnAppearing();
    if (BindingContext is MainPageModel vm)
      vm.UpdateLoginState();
  }
}