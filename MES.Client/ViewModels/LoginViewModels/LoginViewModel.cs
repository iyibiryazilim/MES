using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.HttpClientHelpers;
using Shared.Middleware.Services;
using System.Diagnostics;

namespace MES.Client.ViewModels.LoginViewModels;

public partial class LoginViewModel : BaseViewModel
{
	IHttpClientService _httpClientService;
	IEmployeeService _employeeService;

	public LoginViewModel(IHttpClientService httpClientService, IEmployeeService employeeService)
	{
		_httpClientService = httpClientService;
		_employeeService = employeeService;
	}

	public LoginViewModel() { }

	[ObservableProperty]
	string userCode;


	[RelayCommand]
	async Task AuthenticateAsync(string usercode)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			IsRefreshing = true;

			if(!string.IsNullOrEmpty(usercode))
			{
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var employeeResult = await _employeeService.GetObjects(httpClient);
				if (employeeResult.IsSuccess)
				{
					if(employeeResult.Data.Any())
					{
						
						var currentEmployee = employeeResult.Data.FirstOrDefault(x => x.Code == usercode);
						if (currentEmployee != null)
						{
							
							UserCode = currentEmployee.Code;
							await SecureStorage.Default.SetAsync("CurrentUserName", currentEmployee.Name);
							Application.Current.MainPage = new AppShell();
						}
						else
						{
							await new Helpers.ToastMessageHelper.ToastMessageHelper().ShowToastMessageAsync($"{usercode} kodunda kullanıcı bulunamadı", CommunityToolkit.Maui.Core.ToastDuration.Short);
						}


					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Shell.Current.DisplayAlert("Customer Error: ", $"{ex.Message}", "Tamam");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	[RelayCommand]
	async Task GoToWorkOrderListAsync()
	{
		try
		{
			await Task.Delay(500);
			Application.Current.MainPage = new AppShell();
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
	}

	[RelayCommand]
	public async Task TextChangedAsync(string text)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			IsRefreshing = true;

			if (!string.IsNullOrEmpty(text))
			{
				UserCode = text;
				await Task.Delay(1000);
				Application.Current.MainPage = new AppShell();
			}


		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
			UserCode = "";
		}
	}
}

