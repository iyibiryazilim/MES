using CommunityToolkit.Maui.Views;
using MES.Client.ViewModels.WorkOrderViewModels;

namespace MES.Client.Views.PopupViews;

public partial class ShutdownWorkOrderPopupView : Popup
{
	WorkOrderDetailViewModel viewModel;
	public ShutdownWorkOrderPopupView(WorkOrderDetailViewModel _viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel = _viewModel;
	}
	async void OnYesButtonClicked(object sender, EventArgs e) => await CloseAsync(true);

	async void OnNoButtonClicked(object sender, EventArgs e) => await CloseAsync(false);
}