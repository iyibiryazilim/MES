using MES.Client.Views.WorkOrderViews;

namespace MES.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(route: nameof(WorkOrderDetailView), type: typeof(WorkOrderDetailView));
    }
}

