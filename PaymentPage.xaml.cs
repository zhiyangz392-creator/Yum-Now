using System.Globalization;

namespace Yum_Now;

[QueryProperty(nameof(Restaurant), "restaurant")]
[QueryProperty(nameof(Amount), "amount")]
[QueryProperty(nameof(Address), "address")]
public partial class PaymentPage : ContentPage
{
    private string _restaurant = string.Empty;
    private decimal _amount;
    private string _address = string.Empty;

    public string Restaurant
    {
        get => _restaurant;
        set => _restaurant = Decode(value);
    }

    public string Amount
    {
        get => _amount.ToString("0.00", CultureInfo.InvariantCulture);
        set => _amount = ParseMoney(value);
    }

    public string Address
    {
        get => _address;
        set => _address = Decode(value);
    }

    public PaymentPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindData();
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

    private async void OnPayNowClicked(object? sender, EventArgs e)
    {
        var method = GetSelectedMethod();
        if (string.IsNullOrWhiteSpace(method))
        {
            await DisplayAlertAsync("Payment", "Please choose a payment method.", "OK");
            return;
        }

        if (_amount <= 0)
        {
            await DisplayAlertAsync("Payment", "Invalid payment amount.", "OK");
            return;
        }

        LocalStorageService.SavePreferredPaymentMethod(method);
        var addressToSave = string.IsNullOrWhiteSpace(_address) ? AddressLabel.Text ?? string.Empty : _address;
        LocalStorageService.SaveDeliveryAddress(addressToSave);

        StatusLabel.Text = $"Payment success. Method: {method}. Opening GPS map page...";
        StatusLabel.IsVisible = true;

        await DisplayAlertAsync("Payment Success", $"Paid SGD {_amount:0.00} via {method}.", "OK");

        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync(nameof(DeliveryLocationPage));
        }
    }

    private void BindData()
    {
        RestaurantLabel.Text = string.IsNullOrWhiteSpace(_restaurant)
            ? "Selected restaurant"
            : _restaurant;

        AmountLabel.Text = _amount > 0 ? $"SGD {_amount:0.00}" : "SGD 0.00";

        AddressLabel.Text = string.IsNullOrWhiteSpace(_address)
            ? LocalStorageService.GetDeliveryAddress()
            : _address;

        if (string.IsNullOrWhiteSpace(AddressLabel.Text))
        {
            AddressLabel.Text = "No delivery address";
        }

        SelectSavedPaymentMethod();
        StatusLabel.IsVisible = false;
    }

    private void SelectSavedPaymentMethod()
    {
        var savedMethod = LocalStorageService.GetPreferredPaymentMethod();

        if (savedMethod.Equals("Google Pay", StringComparison.OrdinalIgnoreCase))
        {
            GooglePayRadio.IsChecked = true;
            return;
        }

        if (savedMethod.Equals("Alipay", StringComparison.OrdinalIgnoreCase))
        {
            AlipayRadio.IsChecked = true;
            return;
        }

        GooglePayRadio.IsChecked = true;
    }

    private string GetSelectedMethod()
    {
        if (GooglePayRadio.IsChecked)
        {
            return "Google Pay";
        }

        if (AlipayRadio.IsChecked)
        {
            return "Alipay";
        }

        return string.Empty;
    }

    private static decimal ParseMoney(string? value)
    {
        var decoded = Decode(value);

        return decimal.TryParse(decoded, NumberStyles.Number, CultureInfo.InvariantCulture, out var money)
            ? money
            : 0m;
    }

    private static string Decode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : Uri.UnescapeDataString(value);
    }
}
