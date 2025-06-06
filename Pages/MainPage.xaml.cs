using SnowballMobile.Models;
using SnowballMobile.PageModels;

namespace SnowballMobile.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}