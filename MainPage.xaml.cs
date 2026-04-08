namespace Yum_Now;

public partial class MainPage : ContentPage
{
    private const string TagNormalBackground = "#FFF7EA";
    private const string TagNormalText = "#1F1F1F";
    private const string TagNormalBorder = "#BA9B80";
    private const string TagSelectedBackground = "#FAD8AF";
    private const string TagSelectedText = "#5C2D08";
    private const string TagSelectedBorder = "#C66D1A";

    private string _selectedTag = string.Empty;
    private readonly List<Button> _tagButtons;

    public MainPage()
    {
        InitializeComponent();
        _tagButtons = new List<Button>
        {
            FastFoodButton,
            ChineseButton,
            IndianButton,
            SushiButton,
            SeafoodButton,
            CoffeeButton
        };
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

        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlertAsync("Logout", "Log out and clear local login state?", "Yes", "Cancel");
        if (!confirm)
        {
            return;
        }

        LocalStorageService.ClearLoginState();

        if (Shell.Current == null)
        {
            return;
        }

        await Shell.Current.GoToAsync("//LoginPage");
    }

    private void OnTagButtonClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        foreach (var tagButton in _tagButtons)
        {
            SetButtonNormalStyle(tagButton);
        }

        SetButtonSelectedStyle(button);
        _selectedTag = button.CommandParameter?.ToString()?.Trim() ?? button.Text?.Trim() ?? string.Empty;
        ResultLabel.Text = $"Selected tag: {_selectedTag}";
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_selectedTag))
        {
            ResultLabel.Text = "Please select a tag first.";
            return;
        }

        if (Shell.Current == null)
        {
            return;
        }

        var route = $"{nameof(SearchPage)}?tag={Uri.EscapeDataString(_selectedTag)}";
        await Shell.Current.GoToAsync(route);
    }

    private static void SetButtonNormalStyle(Button button)
    {
        button.BackgroundColor = Color.FromArgb(TagNormalBackground);
        button.TextColor = Color.FromArgb(TagNormalText);
        button.BorderColor = Color.FromArgb(TagNormalBorder);
    }

    private static void SetButtonSelectedStyle(Button button)
    {
        button.BackgroundColor = Color.FromArgb(TagSelectedBackground);
        button.TextColor = Color.FromArgb(TagSelectedText);
        button.BorderColor = Color.FromArgb(TagSelectedBorder);
    }
}
