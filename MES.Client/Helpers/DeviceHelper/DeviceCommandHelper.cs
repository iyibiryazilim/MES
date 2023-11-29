using System.Diagnostics;

namespace MES.Client.Helpers.DeviceHelper;

public class DeviceCommandHelper
{
	public DeviceCommandHelper()
	{

	}

	public async Task SendCommandAsync(string command, string baseUrl)
	{
		try
		{
			var _httpClient = new HttpClient();
			_httpClient.BaseAddress = new Uri(baseUrl);

			var body = $"{{\"cmd\": \"{command}\"}}";
			StringContent stringContent = new StringContent(body);
			stringContent.Headers.Remove("Content-Type");
			stringContent.Headers.Add("Content-Type", "application/json");

			var response = await _httpClient.PostAsync("json", stringContent);
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}
	}
}