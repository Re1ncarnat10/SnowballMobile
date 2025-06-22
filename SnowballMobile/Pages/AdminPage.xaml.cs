namespace SnowballMobile.Pages;

public partial class AdminPage : ContentPage
{
    public AdminPage(AdminPanelPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.LoadSnowballsCommand.Execute(null); // Dodaj tę linię
    }
}