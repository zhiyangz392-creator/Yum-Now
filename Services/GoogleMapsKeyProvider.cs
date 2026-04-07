#if ANDROID
using Android.App;
using Android.Content.PM;
using AndroidApplication = Android.App.Application;
#endif

namespace Yum_Now;

internal static class GoogleMapsKeyProvider
{
    public static string? Resolve()
    {
        var envKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY");
        if (!string.IsNullOrWhiteSpace(envKey))
        {
            return envKey.Trim();
        }

#if ANDROID
        return ResolveFromAndroidManifest();
#else
        return null;
#endif
    }

#if ANDROID
    private static string? ResolveFromAndroidManifest()
    {
        try
        {
            var context = AndroidApplication.Context;
            var packageName = context.PackageName;
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return null;
            }

            var appInfo = context.PackageManager?.GetApplicationInfo(packageName, PackageInfoFlags.MetaData);
            var metadata = appInfo?.MetaData;

            var key = metadata?.GetString("GOOGLE_MAPS_API_KEY")
                      ?? metadata?.GetString("com.google.android.geo.API_KEY");

            return string.IsNullOrWhiteSpace(key) ? null : key.Trim();
        }
        catch
        {
            return null;
        }
    }
#endif
}
