using MES.Client.ViewModels.WorkOrderViewModels;

namespace MES.Client.Views.WorkOrderViews;

public partial class WorkOrderListView : ContentPage
{
    WorkOrderListViewModel _viewModel;
    public WorkOrderListView(WorkOrderListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
