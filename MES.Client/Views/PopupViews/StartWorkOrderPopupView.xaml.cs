using CommunityToolkit.Maui.Views;
using MES.Client.ViewModels.WorkOrderViewModels;
using MES.Client.Views.WorkOrderViews;

namespace MES.Client.Views.PopupViews;

public partial class StartWorkOrderPopupView : Popup
{
    WorkOrderDetailViewModel _viewModel;
	public StartWorkOrderPopupView(WorkOrderDetailViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    public StartWorkOrderPopupView()
    {
    }

    async void OnYesButtonClicked(object? sender, EventArgs e) => await CloseAsync(true);

    async void OnNoButtonClicked(object? sender, EventArgs e) => await CloseAsync(false);


}