using MES.Administration.ViewModels.ProductViewModels;

namespace MES.Administration.Views.ProductViews;

public partial class ProductListView : ContentPage
{
    private readonly ProductListViewModel _viewModel;
    public ProductListView(ProductListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
