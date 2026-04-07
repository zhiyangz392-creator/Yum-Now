namespace Yum_Now;

public partial class SplashPage : ContentPage
{
    private CancellationTokenSource? _animationToken;
    private bool _introStarted;

    public SplashPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_introStarted)
        {
            return;
        }

        _introStarted = true;
        _animationToken = new CancellationTokenSource();
        _ = RunIntroAsync(_animationToken.Token);
    }

    protected override void OnDisappearing()
    {
        _animationToken?.Cancel();
        _animationToken?.Dispose();
        _animationToken = null;

        base.OnDisappearing();
    }

    private async Task RunIntroAsync(CancellationToken token)
    {
        try
        {
            LogoImage.Scale = 0.88;
            LogoImage.Opacity = 0;
            DotsRow.Opacity = 0;
            SetDotsOpacity(0.35);

            await Task.WhenAll(
                LogoImage.FadeToAsync(1, 480, Easing.CubicOut),
                LogoImage.ScaleToAsync(1, 480, Easing.CubicOut),
                DotsRow.FadeToAsync(1, 320, Easing.CubicOut));

            var dotsTask = RunDotsLoopAsync(token);

            await Task.Delay(1700, token);

            if (Shell.Current != null)
            {
                await LogoImage.FadeToAsync(0, 260, Easing.CubicIn);
                var targetRoute = LocalStorageService.IsLoggedIn ? "//MainPage" : "//LoginPage";
                await Shell.Current.GoToAsync(targetRoute);
            }

            await dotsTask;
        }
        catch (OperationCanceledException)
        {
            // Ignore: page closed before animation finished.
        }
    }

    private async Task RunDotsLoopAsync(CancellationToken token)
    {
        var dots = new[] { Dot1, Dot2, Dot3 };
        var index = 0;

        while (!token.IsCancellationRequested)
        {
            for (var i = 0; i < dots.Length; i++)
            {
                dots[i].Opacity = i == index ? 1 : 0.35;
            }

            index = (index + 1) % dots.Length;
            await Task.Delay(220, token);
        }
    }

    private void SetDotsOpacity(double opacity)
    {
        Dot1.Opacity = opacity;
        Dot2.Opacity = opacity;
        Dot3.Opacity = opacity;
    }
}
