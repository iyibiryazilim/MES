using MES.Administration.ViewModels.BOMViewModels;

namespace MES.Administration.Views.BOMViews;

public partial class BOMListView : ContentPage
{
    private readonly BOMListViewModel _viewModel;
    public BOMListView(BOMListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
