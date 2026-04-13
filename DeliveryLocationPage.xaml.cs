using System.Globalization;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace Yum_Now;

public partial class DeliveryLocationPage : ContentPage
{
    private double? _latitude;
    private double? _longitude;

    public DeliveryLocationPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_latitude.HasValue || !_longitude.HasValue)
        {
            await UpdateCurrentLocationAsync();
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Shell.Current == null)
        {
            return;
        }

        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.Navigation.PopAsync();
            return;
        }

        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await UpdateCurrentLocationAsync();
    }

    private async void OnOpenMapClicked(object? sender, EventArgs e)
    {
        if (!_latitude.HasValue || !_longitude.HasValue)
        {
            StatusLabel.Text = "Please refresh GPS first.";
            return;
        }

        var url = BuildSearchUrl(_latitude.Value, _longitude.Value);
        await Launcher.Default.OpenAsync(url);
    }

    private async void OnConfirmReceivedClicked(object? sender, EventArgs e)
    {
        var sent = await NotificationService.TrySendDeliveryReceivedNotificationAsync();
        if (!sent)
        {
            StatusLabel.Text = "Delivery confirmed. Notification permission not granted.";
        }

        await DisplayAlertAsync("Order", "Delivery confirmed. Enjoy your meal!", "OK");

        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync(nameof(DeliverySuccessPage));
        }
    }

    private async Task UpdateCurrentLocationAsync()
    {
        try
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (permission != PermissionStatus.Granted)
            {
                StatusLabel.Text = "Location permission is required.";
                return;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location == null)
            {
                StatusLabel.Text = "Unable to get current location.";
                return;
            }

            _latitude = location.Latitude;
            _longitude = location.Longitude;

            CoordinateLabel.Text =
                $"Lat: {_latitude.Value:0.000000}   Lng: {_longitude.Value:0.000000}";

            MapView.Source = BuildEmbedUrl(_latitude.Value, _longitude.Value);
            StatusLabel.Text = "GPS location updated.";
        }
        catch (FeatureNotEnabledException)
        {
            StatusLabel.Text = "Please enable GPS on your device.";
        }
        catch (PermissionException)
        {
            StatusLabel.Text = "Location permission denied.";
        }
        catch
        {
            StatusLabel.Text = "Failed to load location.";
        }
    }

    private static string BuildEmbedUrl(double latitude, double longitude)
    {
        var lat = latitude.ToString("0.000000", CultureInfo.InvariantCulture);
        var lng = longitude.ToString("0.000000", CultureInfo.InvariantCulture);
        return $"https://www.google.com/maps?q={lat},{lng}&z=16&output=embed";
    }

    private static string BuildSearchUrl(double latitude, double longitude)
    {
        var lat = latitude.ToString("0.000000", CultureInfo.InvariantCulture);
        var lng = longitude.ToString("0.000000", CultureInfo.InvariantCulture);
        return $"https://www.google.com/maps/search/?api=1&query={lat},{lng}";
    }
}
