using MES.Client.Views.LoginViews;
using MES.Client.Views.StopCauseViews;
using MES.Client.Views.WorkOrderViews;

namespace MES.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(route: nameof(WorkOrderDetailView), type: typeof(WorkOrderDetailView));
        Routing.RegisterRoute(route: nameof(StopCauseListView), type: typeof(StopCauseListView));
        Routing.RegisterRoute(route: nameof(LoginView), type: typeof(LoginView));
        Routing.RegisterRoute(route: nameof(WorkOrderListModalView), type: typeof(WorkOrderListModalView));
    }
}

