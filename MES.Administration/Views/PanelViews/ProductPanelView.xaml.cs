using MES.Administration.ViewModels.PanelViewModels;

namespace MES.Administration.Views.PanelViews;

public partial class ProductPanelView : ContentPage
{
    private readonly ProductPanelViewModel _viewModel;
    public ProductPanelView(ProductPanelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
