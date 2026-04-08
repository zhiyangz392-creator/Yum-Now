using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Yum_Now;

internal static class LocalStorageService
{
    private const string LoginStateKey = "user.logged_in";
    private const string LoginEmailFallbackKey = "user.email";
    private const string LoginEmailSecureKey = "user.email.secure";
    private const string LastSearchTagKey = "search.last_tag";
    private const string DeliveryAddressKey = "order.delivery_address";
    private const string CheckoutDraftKey = "checkout.last_draft";
    private const string PreferredPaymentMethodKey = "payment.preferred_method";
    private const string RestaurantDraftPrefix = "restaurant.draft.";

    public static bool IsLoggedIn => Preferences.Default.Get(LoginStateKey, false);

    public static void SaveLoginState(bool isLoggedIn)
    {
        Preferences.Default.Set(LoginStateKey, isLoggedIn);
    }

    public static void ClearLoginState()
    {
        Preferences.Default.Remove(LoginStateKey);
        Preferences.Default.Remove(LoginEmailFallbackKey);

        try
        {
            SecureStorage.Default.Remove(LoginEmailSecureKey);
        }
        catch
        {
            // Ignore secure-storage issues on unsupported devices.
        }
    }

    public static async Task SaveLoginEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var clean = email.Trim();
        Preferences.Default.Set(LoginEmailFallbackKey, clean);

        try
        {
            await SecureStorage.Default.SetAsync(LoginEmailSecureKey, clean);
        }
        catch
        {
            // Some platforms may block secure storage in debug sessions.
        }
    }

    public static async Task<string> GetLoginEmailAsync()
    {
        try
        {
            var secureEmail = await SecureStorage.Default.GetAsync(LoginEmailSecureKey);
            if (!string.IsNullOrWhiteSpace(secureEmail))
            {
                return secureEmail;
            }
        }
        catch
        {
            // Fall back to preferences.
        }

        return Preferences.Default.Get(LoginEmailFallbackKey, string.Empty);
    }

    public static void SaveLastSearchTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            Preferences.Default.Remove(LastSearchTagKey);
            return;
        }

        Preferences.Default.Set(LastSearchTagKey, tag.Trim());
    }

    public static string GetLastSearchTag()
    {
        return Preferences.Default.Get(LastSearchTagKey, string.Empty);
    }

    public static void SaveDeliveryAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return;
        }

        Preferences.Default.Set(DeliveryAddressKey, address.Trim());
    }

    public static string GetDeliveryAddress()
    {
        return Preferences.Default.Get(DeliveryAddressKey, string.Empty);
    }

    public static void SavePreferredPaymentMethod(string method)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            return;
        }

        Preferences.Default.Set(PreferredPaymentMethodKey, method.Trim());
    }

    public static string GetPreferredPaymentMethod()
    {
        return Preferences.Default.Get(PreferredPaymentMethodKey, string.Empty);
    }

    public static void SaveMenuDraft(string restaurant, Dictionary<string, int> quantities)
    {
        if (string.IsNullOrWhiteSpace(restaurant))
        {
            return;
        }

        var key = BuildRestaurantDraftKey(restaurant);
        var rows = new List<MenuDraftItem>();

        foreach (var pair in quantities)
        {
            if (pair.Value <= 0 || string.IsNullOrWhiteSpace(pair.Key))
            {
                continue;
            }

            rows.Add(new MenuDraftItem
            {
                Name = pair.Key.Trim(),
                Quantity = pair.Value
            });
        }

        if (rows.Count == 0)
        {
            Preferences.Default.Remove(key);
            return;
        }

        var json = JsonSerializer.Serialize(rows);
        Preferences.Default.Set(key, json);
    }

    public static Dictionary<string, int> LoadMenuDraft(string restaurant)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(restaurant))
        {
            return result;
        }

        var key = BuildRestaurantDraftKey(restaurant);
        var json = Preferences.Default.Get(key, string.Empty);

        if (string.IsNullOrWhiteSpace(json))
        {
            return result;
        }

        try
        {
            var rows = JsonSerializer.Deserialize<List<MenuDraftItem>>(json);
            if (rows == null)
            {
                return result;
            }

            foreach (var row in rows)
            {
                if (row == null || string.IsNullOrWhiteSpace(row.Name) || row.Quantity <= 0)
                {
                    continue;
                }

                result[row.Name] = row.Quantity;
            }
        }
        catch
        {
            // Keep empty dictionary on broken json.
        }

        return result;
    }

    public static void SaveCheckoutDraft(string restaurant, string items, decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(items) || subtotal <= 0)
        {
            return;
        }

        var draft = new CheckoutDraftModel
        {
            Restaurant = restaurant.Trim(),
            Items = items.Trim(),
            Subtotal = subtotal.ToString("0.00", CultureInfo.InvariantCulture)
        };

        var json = JsonSerializer.Serialize(draft);
        Preferences.Default.Set(CheckoutDraftKey, json);
    }

    public static (string Restaurant, string Items, decimal Subtotal)? LoadCheckoutDraft()
    {
        var json = Preferences.Default.Get(CheckoutDraftKey, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var draft = JsonSerializer.Deserialize<CheckoutDraftModel>(json);
            if (draft == null || string.IsNullOrWhiteSpace(draft.Items))
            {
                return null;
            }

            var subtotal = 0m;
            if (!decimal.TryParse(draft.Subtotal, NumberStyles.Number, CultureInfo.InvariantCulture, out subtotal))
            {
                subtotal = 0m;
            }

            return (draft.Restaurant ?? string.Empty, draft.Items, subtotal);
        }
        catch
        {
            return null;
        }
    }

    private static string BuildRestaurantDraftKey(string restaurant)
    {
        var sb = new StringBuilder();

        foreach (var ch in restaurant.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
            }
            else
            {
                sb.Append('_');
            }

            if (sb.Length >= 48)
            {
                break;
            }
        }

        if (sb.Length == 0)
        {
            sb.Append("unknown");
        }

        return RestaurantDraftPrefix + sb;
    }

    private sealed class MenuDraftItem
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    private sealed class CheckoutDraftModel
    {
        public string Restaurant { get; set; } = string.Empty;
        public string Items { get; set; } = string.Empty;
        public string Subtotal { get; set; } = string.Empty;
    }
}
