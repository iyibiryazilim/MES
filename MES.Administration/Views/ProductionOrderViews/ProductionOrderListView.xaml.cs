using MES.Administration.ViewModels.ProductionOrderViewModels;

namespace MES.Administration.Views.ProductionOrderViews;

public partial class ProductionOrderListView : ContentPage
{
    private readonly ProductionOrderListViewModel _viewModel;
    public ProductionOrderListView(ProductionOrderListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
