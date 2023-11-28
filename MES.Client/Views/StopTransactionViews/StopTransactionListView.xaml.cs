using MES.Client.ViewModels.StopTransactionViewModels;

namespace MES.Client.Views.StopTransactionViews;

public partial class StopTransactionListView : ContentPage
{
	StopTransactionListViewModel _viewModel;
	public StopTransactionListView(StopTransactionListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}