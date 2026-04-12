namespace Yum_Now;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(SplashPage), typeof(SplashPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(RestaurantPage), typeof(RestaurantPage));
        Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
        Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
        Routing.RegisterRoute(nameof(DeliveryLocationPage), typeof(DeliveryLocationPage));
        Routing.RegisterRoute(nameof(DeliverySuccessPage), typeof(DeliverySuccessPage));
    }
}
