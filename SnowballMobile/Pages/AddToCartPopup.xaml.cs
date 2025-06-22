using CommunityToolkit.Maui.Views;

namespace SnowballMobile.Pages;

public partial class AddToCartPopup : Popup
{
    public AddToCartPopup(AddToCartPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}