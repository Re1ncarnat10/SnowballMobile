using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SnowballMobile;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if IOS || MACCATALYST
				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
            })
            .ConfigureFonts(fonts =>
            {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
              fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
              fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
    builder.Logging.AddDebug();
    builder.Services.AddLogging(configure => configure.AddDebug());
#endif

    
    builder.Services.AddSingleton<ApiService>();
    builder.Services.AddSingleton<AppShell>();

    builder.Services.AddTransient<MainPageModel>();
    builder.Services.AddTransient<ShopPageModel>();
    builder.Services.AddTransient<RegisterPageModel>();
    builder.Services.AddTransient<AdminPanelPageModel>();
    builder.Services.AddTransient<CartPageModel>();
    builder.Services.AddTransient<OrdersPageModel>();
    builder.Services.AddTransient<LoginPageModel>();

    builder.Services.AddTransient<Pages.MainPage>();
    builder.Services.AddTransient<Pages.ShopPage>();
    builder.Services.AddTransient<Pages.RegisterPage>();
    builder.Services.AddTransient<Pages.AdminPage>();
    builder.Services.AddTransient<Pages.CartPage>();
    builder.Services.AddTransient<Pages.OrderPage>();
    builder.Services.AddTransient<Pages.LoginPage>();
    
    return builder.Build();
  }
}