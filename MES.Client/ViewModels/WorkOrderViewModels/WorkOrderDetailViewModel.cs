using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Databases.SQLiteDatabase;
using MES.Client.Databases.SQLiteDatabase.Models;
using MES.Client.Helpers.DeviceHelper;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Helpers.MESAPIHelper;
using MES.Client.Views.StopCauseViews;
using MES.Client.Views.StopTransactionViews;
using Newtonsoft.Json;
using Shared.Entity.DTOs;
using Shared.Middleware.Services;
using System.Diagnostics;
using Xamarin.Google.Crypto.Tink.Signature;
using WorkOrder = Shared.Entity.Models.WorkOrder;

namespace MES.Client.ViewModels.WorkOrderViewModels;

[QueryProperty(name: nameof(WorkOrder), queryId: nameof(WorkOrder))]
public partial class WorkOrderDetailViewModel : BaseViewModel
{
	readonly IWorkOrderService _workOrderService;
	readonly IHttpClientService _httpClientService;
	DeviceCommandHelper deviceCommandHelper;
	readonly MESDatabase mesDatabase;

	[ObservableProperty]
	IDispatcherTimer timer;

	[ObservableProperty]
	IDispatcherTimer logoTimer;

	[ObservableProperty]
	WorkOrder workOrder;

	[ObservableProperty]
	bool startButtonEnabled;

	[ObservableProperty]
	DateTime time;

	[ObservableProperty]
	string deviceOpenCloseState;

	[ObservableProperty]
	bool isDeviceOpen;

	[ObservableProperty]
	double sliderValue;

	[ObservableProperty]
	string currentEmployee;

	public bool IsDeviceOpenStateChanged
	{
		get { return IsDeviceOpen; }
		set
		{
			IsDeviceOpen = value;
			OnPropertyChanged();
		}
	}

	public string DeviceOpenCloseStateChanged
	{
		get => DeviceOpenCloseState;
		set
		{
			DeviceOpenCloseState = value;
			OnPropertyChanged();
		}
	}

	public double SliderValueChanged
	{
		get => SliderValue;
		set
		{
			SliderValue = value;
			OnPropertyChanged();
		}
	}

	public Command InProgressWorkOrderCommand { get; }
	public Command StartDeviceCommand { get; }
	public Command GoToStopCauseListCommand { get; }
	public Command GoToStopTransactionListCommand { get; }
	public Command GoToBackCommand { get; }

	public WorkOrderDetailViewModel(MESDatabase mesDB, IWorkOrderService workOrderService, IHttpClientService httpClientService, DeviceCommandHelper _deviceCommandHelper)
	{
		Title = "İş Emri Detay Sayfası";
		_workOrderService = workOrderService;
		_httpClientService = httpClientService;
		deviceCommandHelper = _deviceCommandHelper;
		
		StartDeviceCommand = new Command(async () => await StartDeviceAsync());
		InProgressWorkOrderCommand = new Command(async () => await InProgressWorkOrderAsync());
		GoToStopCauseListCommand = new Command(async () => await GoToStopCauseListAsync());
		GoToStopTransactionListCommand = new Command(async () => await GoToStopTransactionListAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());

		mesDatabase = mesDB;

		StartButtonEnabled = true;

	}

	async Task InProgressWorkOrderAsync()
	{
		if(IsBusy)
			return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			//_workOrderService.InProgressWorkOrderAsync(httpClient, WorkOrder.ReferenceId);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"InProgress Work Order Error :{ex.Message}");
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
	async Task StartDeviceAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			CancellationToken cancellationToken = new();

			await Task.Delay(250);
			await deviceCommandHelper.SendCommandAsync("connectDevice", "http://192.168.1.15:32000").WaitAsync(cancellationToken);

			await Task.Delay(250);
			await deviceCommandHelper.SendCommandAsync("initDevice", "http://192.168.1.15:32000").WaitAsync(cancellationToken);

			await Task.Delay(250);
			await deviceCommandHelper.SendCommandAsync("startDevice", "http://192.168.1.15:32000").WaitAsync(cancellationToken);

			await Task.Delay(250);
			InProgressWorkOrderCommand.Execute(null);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Device Start Error :{ex.Message}");
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}

	}
	async Task GoToStopCauseListAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(StopCauseListView)}");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	async Task GoToStopTransactionListAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(StopTransactionListView)}");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	public async Task StartWorkOrderAsync()
	{
		//await GetDeviceStateAsync();
		await Task.Run(() =>
		{
			Timer = Application.Current.Dispatcher.CreateTimer();
			LogoTimer = Application.Current.Dispatcher.CreateTimer();
			Timer.Interval = TimeSpan.FromSeconds(1);
			LogoTimer.Interval = TimeSpan.FromSeconds(60);
			Timer.Tick += (s, e) => DoSomething();
			LogoTimer.Tick += LogoTimer_Tick;
			Timer.Start();
			LogoTimer.Start();
		});
	}

	private async void LogoTimer_Tick(object sender, EventArgs e)
	{
		await InsertWorkOrderTableToLogoAsync();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public async Task InsertWorkOrderTableAsync()
	{
		WorkOrderTable workOrderTable = new()
		{
			ReferenceId = WorkOrder.ReferenceId,
			Date = DateTime.Now,
			Quantity = WorkOrder.ActualQuantity,
			IsIntegrated = false,
			SubUnitsetReferenceId = WorkOrder.SubUnitsetReferenceId,
			ProductReferenceId = WorkOrder.ProductReferenceId
		};
		await mesDatabase.InsertWorkOrderAsync(workOrderTable);
	}

	/// <summary>
	/// Insert not integrated items to Logo
	/// </summary>
	/// <returns></returns>
	public async Task InsertWorkOrderTableToLogoAsync()
	{
		List<WorkOrderDto> results = new List<WorkOrderDto>();
		var items = await mesDatabase.GetItemsNotIntegratedAsync();

		if (items is not null)
		{
			if (items.Any())
			{
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				foreach (var item in items)
				{
					WorkOrderDto workOrderDto = new WorkOrderDto();
					workOrderDto.WorkOrderReferenceId = item.ReferenceId;
					workOrderDto.ActualQuantity = item.Quantity;
					workOrderDto.CalculatedMethod = 0;
					workOrderDto.IsIncludeSideProduct = false;
					workOrderDto.ProductReferenceId = item.ProductReferenceId;
					workOrderDto.SubUnitsetReferenceId = item.SubUnitsetReferenceId;
					results.Add(workOrderDto);
				}
				var operationResult = await _workOrderService.InsertAsync(httpClient, results);
				if (operationResult.IsSuccess)
				{
					foreach (var item in items)
					{
						item.IsIntegrated = true;
						await mesDatabase.InsertWorkOrderAsync(item);
					}
				}
			}
		}
		return;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public async Task DeleteAllItemsAsync()
	{
		await mesDatabase.DeleteAllItemAsync();
	}

	public async void DoSomething()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			StartButtonEnabled = false;
			await GetDeviceStateAsync();
			Time += TimeSpan.FromSeconds(1);
			await InsertWorkOrderTableAsync();
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	string json = string.Empty;
	async Task GetDeviceStateAsync()
	{
		try
		{

			var httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("http://192.168.1.15:32000");

			var body = "{\"cmd\": \"getDeviceState\"}";
			StringContent stringContent = new StringContent(body);
			stringContent.Headers.Remove("Content-Type");
			stringContent.Headers.Add("Content-Type", "application/json");

			var response = await httpClient.PostAsync("json", stringContent);

			if (response.IsSuccessStatusCode)
			{
				json = await response.Content.ReadAsStringAsync();
				DeviceStateResult deviceStateResult = JsonConvert.DeserializeObject<DeviceStateResult>(json);
				if (deviceStateResult != null)
				{
					if (deviceStateResult.encoder.Count > 0)
					{
						var openCloseState = deviceStateResult.din[0];
						if (openCloseState == 60 || openCloseState == 62)
						{
							IsDeviceOpen = true;
							DeviceOpenCloseState = "Açık";
						}
						else if (openCloseState == 61 || openCloseState == 63)
						{
							IsDeviceOpen = false;
							DeviceOpenCloseState = "Kapalı";
						}
						var firstArray = deviceStateResult.encoder[0];
						WorkOrder.ActualQuantity = firstArray[0];
						SliderValue = firstArray[1];
					}
				}

			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "tamam");
		}
	}

	async Task GoToBackAsync()
	{
		if(IsBusy)
			return;

		try
		{
			IsBusy = true;
			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
		
	}
}