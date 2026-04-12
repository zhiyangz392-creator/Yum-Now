namespace Yum_Now;

public partial class DeliverySuccessPage : ContentPage
{
    private int _selectedStars;

    public DeliverySuccessPage()
    {
        InitializeComponent();
    }

    private void OnStarClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var raw = button.CommandParameter?.ToString();
        if (!int.TryParse(raw, out var starCount))
        {
            return;
        }

        _selectedStars = Math.Clamp(starCount, 1, 5);
        UpdateStarDisplay();
    }

    private async void OnBackToRestaurantClicked(object? sender, EventArgs e)
    {
        if (Shell.Current == null)
        {
            return;
        }

        var navigation = Shell.Current.Navigation;
        var targetIndex = -1;

        for (var i = navigation.NavigationStack.Count - 1; i >= 0; i--)
        {
            if (navigation.NavigationStack[i] is RestaurantPage)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex >= 0)
        {
            while (navigation.NavigationStack.Count - 1 > targetIndex)
            {
                await navigation.PopAsync();
            }

            return;
        }

        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    private void UpdateStarDisplay()
    {
        var stars = new[] { StarButton1, StarButton2, StarButton3, StarButton4, StarButton5 };

        for (var i = 0; i < stars.Length; i++)
        {
            stars[i].Text = i < _selectedStars ? "★" : "☆";
        }

        RatingLabel.Text = $"Rating: {_selectedStars}/5";
    }
}
