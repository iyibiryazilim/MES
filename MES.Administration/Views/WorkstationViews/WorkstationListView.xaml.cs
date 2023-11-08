using MES.Administration.ViewModels.WorkstationViewModels;

namespace MES.Administration.Views.WorkstationViews;

public partial class WorkstationListView : ContentPage
{
    private readonly WorkstationListViewModel _viewModel;
    public WorkstationListView(WorkstationListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}
