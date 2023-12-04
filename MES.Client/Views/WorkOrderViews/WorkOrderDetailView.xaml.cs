using MES.Client.ViewModels.WorkOrderViewModels;

namespace MES.Client.Views.WorkOrderViews;

public partial class WorkOrderDetailView : ContentPage
{
	WorkOrderDetailViewModel _viewModel;
	public WorkOrderDetailView(WorkOrderDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		
	}
	//protected override void OnDisappearing()
	//{
	//	_viewModel.Timer.Stop();
	//	_viewModel.LogoTimer.Stop();
	//	base.OnDisappearing();

	//}
}
