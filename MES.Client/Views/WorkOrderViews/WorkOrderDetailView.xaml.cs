using CommunityToolkit.Maui.Views;
using MES.Client.ViewModels.StopCauseViewModels;
using MES.Client.ViewModels.WorkOrderViewModels;
using MES.Client.Views.StopCauseViews;

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
