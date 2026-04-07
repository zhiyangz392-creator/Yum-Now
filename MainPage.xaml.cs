namespace Yum_Now;

public partial class MainPage : ContentPage
{
    private string _selectedTag = string.Empty;
    private Controls.TagCardView? _selectedCard;

    public MainPage()
    {
        InitializeComponent();
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

    private void OnTagClicked(object? sender, EventArgs e)
    {
        if (sender is not Controls.TagCardView card)
        {
            return;
        }

        if (_selectedCard != null && !ReferenceEquals(_selectedCard, card))
        {
            _selectedCard.IsSelected = false;
        }

        card.IsSelected = true;
        _selectedCard = card;

        _selectedTag = card.TagValue?.Trim() ?? card.Text?.Trim() ?? string.Empty;
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
}
