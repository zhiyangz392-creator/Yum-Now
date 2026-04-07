using System.Text.Json;

namespace Yum_Now;

[QueryProperty(nameof(Tag), "tag")]
public partial class SearchPage : ContentPage
{
    private static readonly HttpClient Http = CreateHttpClient();

    private readonly string? _googleMapsApiKey = GoogleMapsKeyProvider.Resolve();
    private bool _hasInitialized;

    public string Tag { get; set; } = string.Empty;

    public SearchPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasInitialized)
        {
            return;
        }

        _hasInitialized = true;

        var initialTag = string.IsNullOrWhiteSpace(Tag)
            ? LocalStorageService.GetLastSearchTag()
            : Uri.UnescapeDataString(Tag).Trim();

        TagEntry.Text = initialTag;

        if (!string.IsNullOrWhiteSpace(initialTag))
        {
            await SearchAsync(initialTag);
            return;
        }

        SetState(SearchState.Idle, "Type a tag and tap Search on Internet.");
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        var input = TagEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            SetState(SearchState.Idle, "Please type a tag first.");
            return;
        }

        LocalStorageService.SaveLastSearchTag(input);
        await SearchAsync(input);
    }

    private async void OnRetryClicked(object? sender, EventArgs e)
    {
        var input = TagEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(input))
        {
            SetState(SearchState.Idle, "Please type a tag first.");
            return;
        }

        LocalStorageService.SaveLastSearchTag(input);
        await SearchAsync(input);
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

    private async void OnResultSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            collectionView.SelectedItem = null;
        }

        if (e.CurrentSelection.FirstOrDefault() is not SearchResultItem selected)
        {
            return;
        }

        if (Shell.Current == null)
        {
            SetState(SearchState.Error, "Navigation is not ready. Please reopen the app.");
            return;
        }

        var route =
            $"{nameof(RestaurantPage)}" +
            $"?name={Uri.EscapeDataString(selected.RestaurantName)}" +
            $"&tag={Uri.EscapeDataString(selected.Tag)}" +
            $"&menu={Uri.EscapeDataString(selected.MenuItemsRaw)}" +
            $"&placeType={Uri.EscapeDataString(selected.PlaceType)}";

        await Shell.Current.GoToAsync(route);
    }

    private async Task SearchAsync(string tag)
    {
        if (string.IsNullOrWhiteSpace(_googleMapsApiKey))
        {
            ResultsCollection.ItemsSource = null;
            SetState(SearchState.Error, "Google Maps API key is missing.");
            return;
        }

        SetState(SearchState.Loading);

        try
        {
            var places = await FetchPlacesAsync(tag);
            var results = places.Select(place => BuildResultItem(tag, place)).ToList();

            if (results.Count == 0)
            {
                ResultsCollection.ItemsSource = null;
                SetState(SearchState.Empty, "No restaurant found (Google Maps).");
                return;
            }

            ResultsCollection.ItemsSource = results;
            SetState(SearchState.Results, $"Found {results.Count} restaurant result(s) via Google Maps.");
        }
        catch (Exception ex)
        {
            ResultsCollection.ItemsSource = null;
            SetState(SearchState.Error, $"Search failed: {ex.Message}");
        }
    }

    private void SetState(SearchState state, string? message = null)
    {
        var stateName = state.ToString();
        VisualStateManager.GoToState(StateHost, stateName);

        if (message == null)
        {
            return;
        }

        switch (state)
        {
            case SearchState.Idle:
                IdleMessageLabel.Text = message;
                break;
            case SearchState.Results:
                ResultsInfoLabel.Text = message;
                break;
            case SearchState.Empty:
                EmptyMessageLabel.Text = message;
                break;
            case SearchState.Error:
                ErrorMessageLabel.Text = message;
                break;
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("YumNowApp/1.0");
        return client;
    }

    private async Task<List<RestaurantPlace>> FetchPlacesAsync(string tag)
    {
        foreach (var query in SearchRules.BuildQueries(tag))
        {
            var url =
                "https://maps.googleapis.com/maps/api/place/textsearch/json" +
                $"?query={Uri.EscapeDataString(query)}&region=sg&language=en&key={_googleMapsApiKey}";

            using var stream = await Http.GetStreamAsync(url);
            using var document = await JsonDocument.ParseAsync(stream);

            var root = document.RootElement;
            var status = root.TryGetProperty("status", out var statusElement)
                ? statusElement.GetString() ?? string.Empty
                : string.Empty;

            if (status.Equals("ZERO_RESULTS", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!status.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                var errorMessage = root.TryGetProperty("error_message", out var errorElement)
                    ? errorElement.GetString() ?? "Google API error"
                    : "Google API error";

                throw new InvalidOperationException($"{status}: {errorMessage}");
            }

            if (!root.TryGetProperty("results", out var resultsElement) ||
                resultsElement.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            var places = ParsePlaces(resultsElement);
            if (places.Count > 0)
            {
                return places;
            }
        }

        return new List<RestaurantPlace>();
    }

    private static List<RestaurantPlace> ParsePlaces(JsonElement resultsElement)
    {
        var places = new List<RestaurantPlace>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in resultsElement.EnumerateArray())
        {
            if (!item.TryGetProperty("name", out var nameElement))
            {
                continue;
            }

            var name = nameElement.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(name) || !seen.Add(name))
            {
                continue;
            }

            var placeType = "restaurant";
            if (item.TryGetProperty("types", out var typesElement) && typesElement.ValueKind == JsonValueKind.Array)
            {
                var firstType = typesElement.EnumerateArray()
                    .Select(x => x.GetString())
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                if (!string.IsNullOrWhiteSpace(firstType))
                {
                    placeType = firstType.Replace("_", " ");
                }
            }

            places.Add(new RestaurantPlace(name, placeType));

            if (places.Count >= 10)
            {
                break;
            }
        }

        return places;
    }

    private static SearchResultItem BuildResultItem(string tag, RestaurantPlace place)
    {
        var menuItems = SearchRules.ResolveMenuItems(tag);
        var menuRaw = string.Join("|", menuItems);

        return new SearchResultItem
        {
            RestaurantName = place.Name,
            Tag = tag,
            PlaceType = place.PlaceType,
            MenuItemsRaw = menuRaw,
            MenuSummary = $"Type: {place.PlaceType} | Suggested menu: {string.Join(", ", menuItems)}"
        };
    }

    private enum SearchState
    {
        Idle,
        Loading,
        Results,
        Empty,
        Error
    }

    private sealed record RestaurantPlace(string Name, string PlaceType);

    private sealed class SearchResultItem
    {
        public string RestaurantName { get; init; } = string.Empty;
        public string Tag { get; init; } = string.Empty;
        public string PlaceType { get; init; } = string.Empty;
        public string MenuItemsRaw { get; init; } = string.Empty;
        public string MenuSummary { get; init; } = string.Empty;
    }
}
