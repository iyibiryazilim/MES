using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Google.Android.Material.BottomAppBar;

namespace MES.Client;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout |ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Android.Views.View decorView = Window.DecorView;
        decorView.SystemUiVisibility = (StatusBarVisibility)(
            SystemUiFlags.HideNavigation |
            SystemUiFlags.Fullscreen |
            SystemUiFlags.ImmersiveSticky);
    }
}

