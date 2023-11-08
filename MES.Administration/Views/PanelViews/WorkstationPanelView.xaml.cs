using MES.Administration.ViewModels.PanelViewModels;

namespace MES.Administration.Views.PanelViews;

public partial class WorkstationPanelView : ContentPage
{
    private readonly WorkstationPanelViewModel _viewModel;
    public WorkstationPanelView(WorkstationPanelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
