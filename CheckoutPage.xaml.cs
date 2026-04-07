using System.Collections.ObjectModel;
using System.Globalization;

namespace Yum_Now;

[QueryProperty(nameof(Restaurant), "restaurant")]
[QueryProperty(nameof(Items), "items")]
[QueryProperty(nameof(Total), "total")]
public partial class CheckoutPage : ContentPage
{
    private const decimal DeliveryFee = 3.50m;

    private string _restaurant = string.Empty;
    private string _items = string.Empty;
    private decimal _subtotal;

    private ObservableCollection<BillLine> _billLines = new();

    public string Restaurant
    {
        get => _restaurant;
        set => _restaurant = Decode(value);
    }

    public string Items
    {
        get => _items;
        set => _items = Decode(value);
    }

    public string Total
    {
        get => _subtotal.ToString("0.00", CultureInfo.InvariantCulture);
        set => _subtotal = ParseMoney(value);
    }

    public CheckoutPage()
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

    private async void OnConfirmAmountClicked(object? sender, EventArgs e)
    {
        var address = AddressEditor.Text?.Trim() ?? string.Empty;

        if (_billLines.Count == 0)
        {
            await DisplayAlertAsync("Bill", "Please select menu items first.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            await DisplayAlertAsync("Address", "Please enter a delivery address.", "OK");
            return;
        }

        LocalStorageService.SaveDeliveryAddress(address);
        LocalStorageService.SaveCheckoutDraft(_restaurant, _items, _subtotal);

        var totalPay = _subtotal + DeliveryFee;

        ConfirmMessageLabel.Text =
            $"Amount confirmed.\nAddress: {address}\nTotal: SGD {totalPay:0.00}";
        ConfirmMessageLabel.IsVisible = true;

        await DisplayAlertAsync("Confirmed", $"Your bill is confirmed: SGD {totalPay:0.00}", "OK");
    }

    private void BindData()
    {
        TryFillFromStoredDraft();

        RestaurantNameLabel.Text = string.IsNullOrWhiteSpace(_restaurant)
            ? "Selected restaurant"
            : _restaurant;

        _billLines = new ObservableCollection<BillLine>(ParseBillLines(_items));
        MenuCollection.ItemsSource = _billLines;
        EmptyMenuLabel.IsVisible = _billLines.Count == 0;

        if (_subtotal <= 0)
        {
            _subtotal = _billLines.Sum(x => x.LineTotal);
        }

        var totalPay = _subtotal + DeliveryFee;

        SubtotalLabel.Text = $"SGD {_subtotal:0.00}";
        DeliveryFeeLabel.Text = $"SGD {DeliveryFee:0.00}";
        TotalLabel.Text = $"SGD {totalPay:0.00}";

        var savedAddress = LocalStorageService.GetDeliveryAddress();
        AddressEditor.Text = string.IsNullOrWhiteSpace(savedAddress)
            ? "Campus Gate A, Building 3, Unit 08-12"
            : savedAddress;

        ConfirmMessageLabel.IsVisible = false;
    }

    private void TryFillFromStoredDraft()
    {
        var draft = LocalStorageService.LoadCheckoutDraft();
        if (draft == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_restaurant))
        {
            _restaurant = draft.Value.Restaurant;
        }

        if (string.IsNullOrWhiteSpace(_items))
        {
            _items = draft.Value.Items;
        }

        if (_subtotal <= 0)
        {
            _subtotal = draft.Value.Subtotal;
        }
    }

    private static List<BillLine> ParseBillLines(string raw)
    {
        var lines = new List<BillLine>();

        if (string.IsNullOrWhiteSpace(raw))
        {
            return lines;
        }

        var rows = raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var row in rows)
        {
            var parts = row.Split('*', StringSplitOptions.TrimEntries);
            if (parts.Length != 3)
            {
                continue;
            }

            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var qty) || qty <= 0)
            {
                continue;
            }

            if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var unitPrice) || unitPrice < 0)
            {
                continue;
            }

            lines.Add(new BillLine(parts[0], qty, unitPrice));
        }

        return lines;
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

    private sealed record BillLine(string Name, int Qty, decimal UnitPrice)
    {
        public string QtyText => $"x{Qty}";
        public decimal LineTotal => Qty * UnitPrice;
        public string LineTotalText => $"SGD {LineTotal:0.00}";
    }
}
