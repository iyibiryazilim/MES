using CommunityToolkit.Mvvm.Input;
using MES.Client.Views.WorkOrderViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Client.ViewModels.PanelViewModels;

public partial class MainPanelViewModel : BaseViewModel
{
	public MainPanelViewModel()
	{
		Title = "Ana Panel";
	}

	[RelayCommand]
	async Task GoToWorkOrderListAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			//await Task.Delay(200);
			await Shell.Current.GoToAsync($"{nameof(WorkOrderListView)}");
		} catch(Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

}
