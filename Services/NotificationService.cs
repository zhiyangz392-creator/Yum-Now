using Microsoft.Maui.ApplicationModel;

#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
#endif

namespace Yum_Now;

internal static class NotificationService
{
    public static async Task<bool> TrySendDeliveryReceivedNotificationAsync()
    {
#if ANDROID
        var hasPermission = await EnsureNotificationPermissionAsync();
        if (!hasPermission)
        {
            return false;
        }

        ShowAndroidDeliveryNotification();
        return true;
#else
        await Task.CompletedTask;
        return false;
#endif
    }

#if ANDROID
    private const string DeliveryChannelId = "yum_now_delivery_channel";
    private const string DeliveryChannelName = "Delivery Updates";
    private const string DeliveryChannelDescription = "Yum Now delivery notifications";
    private const int DeliveryNotificationId = 2001;

    private static async Task<bool> EnsureNotificationPermissionAsync()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return true;
        }

        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status == PermissionStatus.Granted)
        {
            return true;
        }

        status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        return status == PermissionStatus.Granted;
    }

    private static void ShowAndroidDeliveryNotification()
    {
        var context = Android.App.Application.Context;
        if (context == null)
        {
            return;
        }

        var notificationManager =
            context.GetSystemService(Context.NotificationService) as NotificationManager;

        if (notificationManager == null)
        {
            return;
        }

        EnsureChannel(notificationManager);

        var intent = new Intent(context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            pendingIntentFlags |= PendingIntentFlags.Immutable;
        }

        var pendingIntent = PendingIntent.GetActivity(
            context,
            0,
            intent,
            pendingIntentFlags);
        if (pendingIntent == null)
        {
            return;
        }

        var builder = new NotificationCompat.Builder(context, DeliveryChannelId);
        if (builder == null)
        {
            return;
        }

        builder.SetSmallIcon(Resource.Mipmap.appicon);
        builder.SetContentTitle("Yum Now");
        builder.SetContentText("Your order has been received. Please check it promptly.");
        builder.SetPriority(NotificationCompat.PriorityDefault);
        builder.SetAutoCancel(true);
        builder.SetContentIntent(pendingIntent);

        var notification = builder.Build();
        if (notification == null)
        {
            return;
        }

        var managerCompat = NotificationManagerCompat.From(context);
        if (managerCompat == null)
        {
            return;
        }

        managerCompat.Notify(DeliveryNotificationId, notification);
    }

    private static void EnsureChannel(NotificationManager manager)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        if (manager.GetNotificationChannel(DeliveryChannelId) != null)
        {
            return;
        }

        var channel = new NotificationChannel(
            DeliveryChannelId,
            DeliveryChannelName,
            NotificationImportance.Default)
        {
            Description = DeliveryChannelDescription
        };

        manager.CreateNotificationChannel(channel);
    }
#endif
}
