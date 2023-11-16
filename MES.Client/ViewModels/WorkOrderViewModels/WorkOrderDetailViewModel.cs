using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES.Client.Helpers.MESAPIHelper;
using MES.Client.Views.StopCauseViews;
using Microcharts;
using Newtonsoft.Json;
using Shared.Entity.BaseModels;
using Shared.Entity.Models;
using SkiaSharp;
using System.Diagnostics;

namespace MES.Client.ViewModels.WorkOrderViewModels;

[QueryProperty(name: nameof(WorkOrder), queryId: nameof(WorkOrder))]
public partial class WorkOrderDetailViewModel : BaseViewModel
{
	//StopCauseListViewModel _stopCauseListViewModel;

	[ObservableProperty]
	WorkOrder workOrder;

	[ObservableProperty]
	double quantity;

	[ObservableProperty]
	double actualRateChange;

	[ObservableProperty]
	bool startButtonEnabled = true;

	[ObservableProperty]
	DateTime time;

	//public Command GetDeviceStateCommand { get; }

	public WorkOrderDetailViewModel()
	{
		Title = "İş Emri Detay Sayfası";
		//GetDeviceStateCommand = new Command(async () => await GetDeviceStateAsync());
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
		//     get
		//     {
		//         if(ProductionWorkOrderList is null)
		//         {
		//	return 0;
		//} else
		//         {
		//             if(ProductionWorkOrderList.ActualQuantity == 0)
		//             {
		//                 return 0;
		//             } else
		//             {
		//                 return ((double)ProductionWorkOrderList.ActualQuantity/(double)ProductionWorkOrderList.Quantity) * 100;
		//             }
		//         }
		//     }
		get { return 60; }
		set
		{
			ActualRateChange = value;
			OnPropertyChanged();
		}
	}

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
	async Task GoToStopCauseListAsync()
	{
		await Shell.Current.GoToAsync($"{nameof(StopCauseListView)}");
	}

	//[RelayCommand]
	//async Task ShowStartWorkOrderPopupAsync()
	//{
	//	var popup = new StartWorkOrderPopupView(this);
	//	var result = await Shell.Current.ShowPopupAsync(popup);
	//	if (result is bool boolResult)
	//	{
	//		if (boolResult)
	//		{
	//			await StartWorkOrderAsync();
	//		}
	//		else
	//		{
	//			return;
	//		}
	//	}
	//}

	//public IDispatcherTimer timer;

	[RelayCommand]
	public async Task StartWorkOrderAsync()
	{
		//await Task.Run(() =>
		//{
		//	var timer = Application.Current.Dispatcher.CreateTimer();
		//	timer.Interval = TimeSpan.FromSeconds(1);
		//	timer.Tick += (s, e) => DoSomething();
		//	timer.Start();
		//});
		await GetDeviceStateAsync();
	}

	public void DoSomething()
	{
		StartButtonEnabled = false;
		MainThread.BeginInvokeOnMainThread(() =>
		{
			//await GetDeviceStateAsync();
			Quantity += 1;
			Time += TimeSpan.FromSeconds(1);

		});
	}

	string json = string.Empty;
	async Task GetDeviceStateAsync()
	{
		try
		{

			var httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("http://192.168.1.10:32000");

			var body = "{\"cmd\": \"getDeviceState\"}";
			StringContent stringContent = new StringContent(body);
			stringContent.Headers.Remove("Content-Type");
			stringContent.Headers.Add("Content-Type", "application/json");

			var response = await httpClient.PostAsync("json", stringContent);

			if (response.IsSuccessStatusCode)
			{
				json = await response.Content.ReadAsStringAsync();
				//Console.WriteLine(json);
				//Debug.WriteLine(json);
				DeviceStateResult deviceStateResult = JsonConvert.DeserializeObject<DeviceStateResult>(json);
				if (deviceStateResult != null)
				{
					if (deviceStateResult.encoder.Count > 0)
					{
						var firstArray = deviceStateResult.encoder[0];
						Quantity = firstArray[0];
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

	[RelayCommand]
	async Task GoToBackAsync()
	{
		await Shell.Current.GoToAsync("..");
	}

	//async Task InsertWorkOrderToDatabase(Databases.SQLiteDatabase.Models.WorkOrder workOrder)
	//{

	//	workOrder.Date =
	//	workOrder.WorkOrderCode = ;
	//	workOrder.WorkStationCode= ;
	//	workOrder.IsIntegrated = true;
	//}

}