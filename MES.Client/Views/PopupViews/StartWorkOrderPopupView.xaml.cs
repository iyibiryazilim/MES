using CommunityToolkit.Maui.Views;
using MES.Client.ViewModels.WorkOrderViewModels;

namespace MES.Client.Views.PopupViews;

public partial class StartWorkOrderPopupView : Popup
{
	WorkOrderListViewModel _viewModel;
	public StartWorkOrderPopupView(WorkOrderListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	public StartWorkOrderPopupView()
	{
	}

	async void OnYesButtonClicked(object sender, EventArgs e) => await CloseAsync(true);

	async void OnNoButtonClicked(object sender, EventArgs e) => await CloseAsync(false);


}