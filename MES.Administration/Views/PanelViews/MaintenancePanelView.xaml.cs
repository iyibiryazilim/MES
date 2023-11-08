using MES.Administration.ViewModels.PanelViewModels;

namespace MES.Administration.Views.PanelViews;

public partial class MaintenancePanelView : ContentPage
{
    private readonly MaintenancePanelViewModel _viewModel;
    public MaintenancePanelView(MaintenancePanelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
