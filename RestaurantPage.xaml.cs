using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Yum_Now;

[QueryProperty(nameof(RestaurantName), "name")]
[QueryProperty(nameof(Tag), "tag")]
[QueryProperty(nameof(Menu), "menu")]
[QueryProperty(nameof(PlaceType), "placeType")]
public partial class RestaurantPage : ContentPage
{
    private string _restaurantName = string.Empty;
    private string _tag = string.Empty;
    private string _menuRaw = string.Empty;
    private string _placeType = string.Empty;

    private ObservableCollection<MenuItemRow> _menuItems = new();

    public string RestaurantName
    {
        get => _restaurantName;
        set => _restaurantName = Decode(value);
    }

    public string Tag
    {
        get => _tag;
        set => _tag = Decode(value);
    }

    public string Menu
    {
        get => _menuRaw;
        set => _menuRaw = Decode(value);
    }

    public string PlaceType
    {
        get => _placeType;
        set => _placeType = Decode(value);
    }

    public RestaurantPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindPageData();
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

    private void OnIncreaseClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: MenuItemRow row })
        {
            row.Quantity++;
            UpdateTotals();
        }
    }

    private void OnDecreaseClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: MenuItemRow row } && row.Quantity > 0)
        {
            row.Quantity--;
            UpdateTotals();
        }
    }

    private async void OnGoToCheckoutClicked(object? sender, EventArgs e)
    {
        var selected = _menuItems
            .Where(item => item.Quantity > 0)
            .ToList();

        if (selected.Count == 0)
        {
            await DisplayAlertAsync("Bill", "Please select at least one menu item.", "OK");
            return;
        }

        if (Shell.Current == null)
        {
            return;
        }

        var subtotal = selected.Sum(item => item.Quantity * item.Price);
        var itemsRaw = string.Join(";", selected.Select(BuildCheckoutItemRow));

        LocalStorageService.SaveCheckoutDraft(_restaurantName, itemsRaw, subtotal);

        var route =
            $"{nameof(CheckoutPage)}" +
            $"?restaurant={Uri.EscapeDataString(_restaurantName)}" +
            $"&items={Uri.EscapeDataString(itemsRaw)}" +
            $"&total={Uri.EscapeDataString(subtotal.ToString("0.00", CultureInfo.InvariantCulture))}";

        await Shell.Current.GoToAsync(route);
    }

    private void BindPageData()
    {
        RestaurantNameLabel.Text = string.IsNullOrWhiteSpace(_restaurantName)
            ? "Unknown Restaurant"
            : _restaurantName;

        TagLabel.Text = string.IsNullOrWhiteSpace(_tag)
            ? "Not specified"
            : _tag;

        var (menuSeeds, source) = ResolveMenu();

        _menuItems = new ObservableCollection<MenuItemRow>(
            menuSeeds.Select(item => new MenuItemRow(item.Name, item.Price)));

        RestoreMenuDraft();

        MenuCollection.ItemsSource = _menuItems;
        MenuSourceLabel.Text = $"Menu source: {source}";

        UpdateTotals();
    }

    private (List<RestaurantCatalog.MenuSeed> Items, string Source) ResolveMenu()
    {
        if (RestaurantCatalog.TryGetProfileMenu(_restaurantName, out var profileItems, out var profileSource))
        {
            return (profileItems, profileSource);
        }

        var categoryMenu = RestaurantCatalog.BuildCategoryMenu(_tag, _placeType);
        if (categoryMenu.Count > 0)
        {
            return (categoryMenu, "category-based menu");
        }

        var routeMenu = RestaurantCatalog.BuildRouteMenu(_menuRaw);
        if (routeMenu.Count > 0)
        {
            return (routeMenu, "search-result fallback");
        }

        return (RestaurantCatalog.DefaultMenu.ToList(), "default fallback");
    }

    private void RestoreMenuDraft()
    {
        var saved = LocalStorageService.LoadMenuDraft(_restaurantName);
        if (saved.Count == 0)
        {
            return;
        }

        foreach (var item in _menuItems)
        {
            if (saved.TryGetValue(item.Name, out var qty) && qty > 0)
            {
                item.Quantity = qty;
            }
        }
    }

    private void UpdateTotals()
    {
        var totalItems = _menuItems.Sum(item => item.Quantity);
        var totalPrice = _menuItems.Sum(item => item.Quantity * item.Price);

        TotalItemsLabel.Text = $"Total items: {totalItems}";
        EstimatedTotalLabel.Text = $"Estimated total: SGD {totalPrice:0.00}";

        PersistMenuDraft();
    }

    private void PersistMenuDraft()
    {
        var snapshot = _menuItems.ToDictionary(
            item => item.Name,
            item => item.Quantity,
            StringComparer.OrdinalIgnoreCase);

        LocalStorageService.SaveMenuDraft(_restaurantName, snapshot);
    }

    private static string BuildCheckoutItemRow(MenuItemRow item)
    {
        var safeName = item.Name.Replace(";", " ").Replace("*", " ").Trim();
        var unitPrice = item.Price.ToString("0.00", CultureInfo.InvariantCulture);
        return $"{safeName}*{item.Quantity}*{unitPrice}";
    }

    private static string Decode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : Uri.UnescapeDataString(value);
    }

    private sealed class MenuItemRow : INotifyPropertyChanged
    {
        private int _quantity;

        public MenuItemRow(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public decimal Price { get; }
        public string PriceText => $"SGD {Price:0.00}";

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity == value)
                {
                    return;
                }

                _quantity = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
