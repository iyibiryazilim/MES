using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace MES.Client;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape)]
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

