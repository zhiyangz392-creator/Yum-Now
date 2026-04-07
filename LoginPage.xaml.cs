namespace Yum_Now;

public partial class LoginPage : ContentPage
{
    private bool _prefillDone;

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_prefillDone)
        {
            return;
        }

        _prefillDone = true;

        var savedEmail = await LocalStorageService.GetLoginEmailAsync();
        if (!string.IsNullOrWhiteSpace(savedEmail) && string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            EmailEntry.Text = savedEmail;
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

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text?.Trim();

        if (!HasText(email) || !HasText(password))
        {
            await DisplayAlertAsync("Login", "Please enter email and password.", "OK");
            return;
        }

        LocalStorageService.SaveLoginState(true);
        await LocalStorageService.SaveLoginEmailAsync(email!);

        if (Shell.Current == null)
        {
            return;
        }

        await Shell.Current.GoToAsync("//MainPage");
    }

    private static bool HasText(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
