using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowballMobile.Models;

public partial class AddToCartPopupViewModel : ObservableObject
{
    public SnowballDto Snowball { get; }
    [ObservableProperty] private bool isBusy;

    public IAsyncRelayCommand AddToCartCommand { get; }
    public IRelayCommand ClosePopupCommand { get; }

    public string ProductName => Snowball.Name;

    public AddToCartPopupViewModel(
        SnowballDto snowball,
        Func<Task> addToCartAction,
        Action closeAction)
    {
        Snowball = snowball;
        AddToCartCommand = new AsyncRelayCommand(async () =>
        {
            IsBusy = true;
            await addToCartAction();
            IsBusy = false;
            closeAction();
        });
        ClosePopupCommand = new RelayCommand(closeAction);
    }
}