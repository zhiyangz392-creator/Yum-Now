using Android.App;
using Android.Runtime;
using Android.Util;
using System.Diagnostics;

namespace Yum_Now
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        private static void OnAndroidUnhandledException(object? sender, RaiseThrowableEventArgs e)
        {
            try
            {
                var message = $"[AndroidUnhandled] {e.Exception}";
                Log.Error("YumNow", message);
                Debug.WriteLine(message);
            }
            finally
            {
                // Prevent a hard process kill when a managed callback leaks an exception to Java.
                e.Handled = true;
            }
        }
    }
}
