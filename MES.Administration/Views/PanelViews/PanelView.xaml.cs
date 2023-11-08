using MES.Administration.ViewModels.PanelViewModels;

namespace MES.Administration.Views.PanelViews;

public partial class PanelView : ContentPage
{
    private readonly PanelViewModel _viewModel;
    public PanelView(PanelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
