using System.Diagnostics;

namespace MES.Client.Helpers;

public class CurrentUserHelper
{
	public async Task<string> GetCurrentEmployeeAsync()
	{
		string result = string.Empty;
		try
		{
			string oauthToken = await SecureStorage.GetAsync("CurrentUserName");
			if (oauthToken == null)
			{
				result = "Kullanıcı Bulunamadı";
			}
			else
			{
				result = oauthToken;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}

		return result;
	}
}
