using AndroidX.Annotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Databases.SQLiteDatabase;
using MES.Client.Databases.SQLiteDatabase.Models;
using MES.Client.Helpers.HttpClientHelpers;
using MES.Client.Helpers.MESAPIHelper;
using MES.Client.Views.StopCauseViews;
using Microcharts;
using Newtonsoft.Json;
using Shared.Entity.DTOs;
using Shared.Middleware.Services;
using SkiaSharp;
using System.Diagnostics;
using WorkOrder = Shared.Entity.Models.WorkOrder;

namespace MES.Client.ViewModels.WorkOrderViewModels;

[QueryProperty(name: nameof(WorkOrder), queryId: nameof(WorkOrder))]
public partial class WorkOrderDetailViewModel : BaseViewModel
{
	//StopCauseListViewModel _stopCauseListViewModel;
	public IDispatcherTimer timer;
	public IDispatcherTimer logoTimer;
	IWorkOrderService _workOrderService;
	IHttpClientService _httpClientService;

	private readonly MESDatabase mesDatabase;
	private WorkOrderListViewModel workOrderListViewModel;

	[ObservableProperty]
	WorkOrder workOrder;

	[ObservableProperty]
	public double quantity;

	[ObservableProperty]
	double objCount;

	[ObservableProperty]
	double actualRateChange;

	[ObservableProperty]
	bool startButtonEnabled = true;

	[ObservableProperty]
	DateTime time;

	[ObservableProperty]
	string deviceOpenCloseState;
	//public Command GetDeviceStateCommand { get; }

	[ObservableProperty]
	bool isDeviceOpen;

	[ObservableProperty]
	double sliderValue;

	[ObservableProperty]
	string currentEmployee;
	public Command GetCurrentEmployeeCommand { get; }

	public WorkOrderDetailViewModel(MESDatabase mesDB, IWorkOrderService workOrderService, IHttpClientService httpClientService, WorkOrderListViewModel _workOrderListViewModel)
	{
		Title = "İş Emri Detay Sayfası";
		_workOrderService = workOrderService;
		_httpClientService = httpClientService;
		//GetDeviceStateCommand = new Command(async () => await GetDeviceStateAsync());
		mesDatabase = mesDB;
		

		MainThread.BeginInvokeOnMainThread(async () =>
		{
			 await GetCurrentEmployeeAsync();
		});
	}

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

	public double QuantityChanged
	{
		get { return Quantity; }
		set
		{
			Quantity = value;
			OnPropertyChanged();
		}
	}

	public double ActualRate
	{
		//get
		//{
		//	if (WorkOrder is null)
		//	{
		//		return 0;
		//	}
		//	else
		//	{
		//		if (WorkOrder. == 0)
		//		{
		//			return 0;
		//		}
		//		else
		//		{
		//			return ((double)WorkOrder.ActualQuantity / (double)WorkOrder.Quantity) * 100;
		//		}
		//	}
		//}
		get { return 0; }
		set
		{
			ActualRateChange = value;
			OnPropertyChanged();
		}
	}

	//[RelayCommand]
	//public async Task StopTimer()
	//{
	//	await Task.Run(() =>  { timer.Stop(); })
	//}

	public Chart OEE => GetOEE();
	public Chart Availability => GetAvailability();
	public Chart Productivity => GetProductivity();
	public Chart Quality => GetQuality();

	public Chart GetOEE()
	{
		return new HalfRadialGaugeChart()
		{
			Entries = GetOEEEntries(),
			StartAngle = 20,
			BackgroundColor = SKColors.Transparent,
		};


	}
	public Chart GetAvailability()
	{
		return new HalfRadialGaugeChart()
		{
			Entries = GetAvaibilityEntries(),

			BackgroundColor = SKColors.Transparent,
		};
	}
	public Chart GetProductivity()
	{
		return new RadialGaugeChart()
		{
			Entries = GetProductivityEntries(),
			MaxValue = 100,
			MinValue = 10,

			BackgroundColor = SKColors.Transparent,
		};
	}
	public Chart GetQuality()
	{
		return new HalfRadialGaugeChart()
		{
			Entries = GetQualityEntries(),

			BackgroundColor = SKColors.Transparent,
		};
	}

	private List<ChartEntry> GetOEEEntries()
	{
		List<ChartEntry> entries = new List<ChartEntry>();

		entries.Add(new ChartEntry(80)
		{
			ValueLabel = "80",
			TextColor = SKColor.Parse("434343"),
			Label = "OEE",
			Color = SKColors.Blue
		});

		return entries;
	}

	private List<ChartEntry> GetAvaibilityEntries()
	{
		List<ChartEntry> entries = new List<ChartEntry>();

		entries.Add(new ChartEntry(100)
		{
			ValueLabel = "ValueLabel",
			TextColor = SKColor.Parse("434343"),
			Label = "Label",
			Color = SKColors.Blue
		});

		entries.Add(new ChartEntry(200)
		{
			ValueLabel = "ValueLabel",
			TextColor = SKColor.Parse("434343"),
			Label = "Label",
			Color = SKColors.Yellow
		});

		return entries;
	}

	private List<ChartEntry> GetProductivityEntries()
	{
		List<ChartEntry> entries = new List<ChartEntry>();

		entries.Add(new ChartEntry(100)
		{
			ValueLabel = "%42",
			TextColor = SKColor.Parse("009ef7"),
			Label = "OEE",
			Color = SKColor.Parse("009ef7")
		});

		entries.Add(new ChartEntry(200)
		{
			ValueLabel = "%65",
			TextColor = SKColor.Parse("FFC700"),
			Label = "Productivity",
			Color = SKColor.Parse("FFC700")
		});

		entries.Add(new ChartEntry(150)
		{
			ValueLabel = "%73",
			TextColor = SKColor.Parse("F1416C"),
			Label = "Avaibility",
			Color = SKColor.Parse("F1416C"),
		});

		entries.Add(new ChartEntry(30)
		{
			ValueLabel = "%30",
			TextColor = SKColor.Parse("2B0B98"),
			Label = "Quality",
			Color = SKColor.Parse("2B0B98")
		});

		return entries;
	}

	private List<ChartEntry> GetQualityEntries()
	{
		List<ChartEntry> entries = new List<ChartEntry>();

		entries.Add(new ChartEntry(100)
		{
			ValueLabel = "ValueLabel",
			TextColor = SKColor.Parse("434343"),
			Label = "Label",
			Color = SKColors.Blue
		});

		entries.Add(new ChartEntry(200)
		{
			ValueLabel = "ValueLabel",
			TextColor = SKColor.Parse("434343"),
			Label = "Label",
			Color = SKColors.Yellow
		});

		return entries;
	}

	[RelayCommand]
	async Task GetDBCountAsync()
	{
		var count = await mesDatabase.GetItemsAsync();
		ObjCount = count.Count;
	}

	[RelayCommand]
	async Task GoToStopCauseListAsync()
	{
		await Shell.Current.GoToAsync($"{nameof(StopCauseListView)}");
	}

	[RelayCommand]
	public async Task StartWorkOrderAsync()
	{
		//await GetDeviceStateAsync();
		await Task.Run(() =>
		{
			timer = Application.Current.Dispatcher.CreateTimer();
			logoTimer = Application.Current.Dispatcher.CreateTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			logoTimer.Interval = TimeSpan.FromSeconds(60);
			timer.Tick += (s, e) => DoSomething();
			logoTimer.Tick += async (s,e) => await InsertWorkOrderTableToLogoAsync();
			timer.Start();
			logoTimer.Start();
		});
	}

	// Insert WorkOrderTable to SQLite (local db)
	public async Task InsertWorkOrderTableAsync()
	{
		WorkOrderTable workOrderTable = new()
		{
			ReferenceId = WorkOrder.ReferenceId,
			Date = DateTime.Now,
			Quantity = Quantity,
			IsIntegrated = false,
			SubUnitsetReferenceId = WorkOrder.SubUnitsetReferenceId,
			ProductReferenceId = WorkOrder.ProductReferenceId
		};
		await mesDatabase.InsertWorkOrderAsync(workOrderTable);
	}

	// Insert not integrated items to Logo
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

	public async Task DeleteAllItemsAsync()
	{
		await mesDatabase.DeleteAllItemAsync();
	}

	//public void DoInsertLogo()
	//{
	//	MainThread.BeginInvokeOnMainThread(async () =>
	//	{
	//		await GetDeviceStateAsync();
	//		await InsertWorkOrderTableAsync();
	//	});
	//}

	public void DoSomething()
	{

		StartButtonEnabled = false;
		MainThread.BeginInvokeOnMainThread(async () =>
		{
			await GetDeviceStateAsync();
			//Quantity += 1;
			Time += TimeSpan.FromSeconds(1);
			await InsertWorkOrderTableAsync();
		});
	}

	string json = string.Empty;
	async Task GetDeviceStateAsync()
	{
		try
		{

			var httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("http://192.168.1.3:32000");

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
						Quantity = firstArray[0];
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

	public async Task GetCurrentEmployeeAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			string oauthToken = await SecureStorage.GetAsync("CurrentUserName");
			if (oauthToken == null)
			{
				CurrentEmployee = "Kullanıcı Bulunamadı";
			}
			else
			{
				CurrentEmployee = oauthToken;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			await Application.Current.MainPage.DisplayAlert("Auth Error", "Get Current Employee Error", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	async Task GoToBackAsync()
	{
		await Shell.Current.GoToAsync("..");
	}
}