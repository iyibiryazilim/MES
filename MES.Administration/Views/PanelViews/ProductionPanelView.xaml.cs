using MES.Administration.ViewModels.PanelViewModels;

namespace MES.Administration.Views.PanelViews;

public partial class ProductionPanelView : ContentPage
{
    private readonly ProductionPanelViewModel _viewModel;
    public ProductionPanelView(ProductionPanelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
