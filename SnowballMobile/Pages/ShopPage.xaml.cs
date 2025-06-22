namespace  SnowballMobile.Pages;

public partial class ShopPage : ContentPage
{
    private readonly ShopPageModel _viewModel;

    public ShopPage(ShopPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_viewModel.IsBusy && !_viewModel.Snowballs.Any())
        {
            _viewModel.LoadSnowballsCommand.Execute(null);
        }
    }
}
