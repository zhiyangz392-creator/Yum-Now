namespace Yum_Now;

internal static class GoogleMapsKeyProvider
{
    // For class demo use; replace with your own key when needed.
    private const string DemoKey = "AIzaSyA7J02RZqiSVOP5UNAOa3k2n76lP2PlE0Y";

    public static string Resolve()
    {
        var envKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY");
        if (!string.IsNullOrWhiteSpace(envKey))
        {
            return envKey.Trim();
        }

        return DemoKey;
    }
}
