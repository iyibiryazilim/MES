using MES.Client.ViewModels.PanelViewModels;

namespace MES.Client.Views.PanelViews;

public partial class MainPanelView : ContentPage
{
	MainPanelViewModel _viewModel;
	public MainPanelView(MainPanelViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}