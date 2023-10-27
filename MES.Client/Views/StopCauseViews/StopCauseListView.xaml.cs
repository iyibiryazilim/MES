using CommunityToolkit.Maui.Views;
using MES.Client.ViewModels.StopCauseViewModels;

namespace MES.Client.Views.StopCauseViews;

public partial class StopCauseListView : ContentPage
{
	StopCauseListViewModel _viewModel;

    public StopCauseListView(StopCauseListViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}