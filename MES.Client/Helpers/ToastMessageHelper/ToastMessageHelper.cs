using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace MES.Client.Helpers.ToastMessageHelper;

public class ToastMessageHelper
{
	public ToastMessageHelper()
	{
	}

	public async Task ShowToastMessageAsync(string message, ToastDuration toastDuration)
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		string text = message;
		ToastDuration duration = toastDuration;
		var toast = Toast.Make(text, duration);

		await toast.Show(cancellationTokenSource.Token);
	}
}
