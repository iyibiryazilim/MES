using MES.Client.ViewModels.WorkOrderViewModels;

namespace MES.Client.Views.WorkOrderViews;

public partial class WorkOrderDetailView : ContentPage
{
    WorkOrderDetailViewModel _viewModel;
    public WorkOrderDetailView(WorkOrderDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
