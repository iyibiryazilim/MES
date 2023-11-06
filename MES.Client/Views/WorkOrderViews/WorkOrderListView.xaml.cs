using Android.Telephony.Emergency;
using CommunityToolkit.Maui.Core.Platform;
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

    public double GetGridProductImageHeight()
    {
        var imageCarousel = carouselList.VisibleViews.
            OfType<Grid>()
            .FirstOrDefault(grid => grid.ClassId == "gridProductImage");

            if (imageCarousel != null)
            {
                return imageCarousel.Height;
            }

        return 620;
    }

    public double ProductImageHeight
    {
        get => GetGridProductImageHeight();
    }
}
