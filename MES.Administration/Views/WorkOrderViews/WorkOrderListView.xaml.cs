using MES.Administration.ViewModels.WorkOrderViewModels;

namespace MES.Administration.Views.WorkOrderViews;

public partial class WorkOrderListView : ContentPage
{
    private readonly WorkOrderListViewModel _viewModel;
    public WorkOrderListView(WorkOrderListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
