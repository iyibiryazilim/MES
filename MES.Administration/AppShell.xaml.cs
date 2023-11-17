using MES.Administration.Views.PanelViews;
using MES.Administration.Views.ProductViews;
using MES.Administration.Views.WorkOrderViews;
using MES.Administration.Views.WorkstationViews;
using SimpleToolkit.Core;

namespace MES.Administration;

public partial class AppShell : SimpleToolkit.SimpleShell.SimpleShell
{
    public AppShell()
    {
        InitializeComponent();

        AddTab(typeof(PanelView), PageType.PanelView);
        AddTab(typeof(ProductionPanelView), PageType.ProductionPanel);
        AddTab(typeof(WorkstationListView), PageType.WorkstationPanel);
        AddTab(typeof(WorkOrderListView), PageType.ProductPanel);
        AddTab(typeof(ProductListView), PageType.MaintenancePanel);

        Loaded += AppShellLoaded;
    }

    private static void AppShellLoaded(object sender, EventArgs e)
    {
        var shell = sender as AppShell;

        shell.Window.SubscribeToSafeAreaChanges(safeArea =>
        {
            shell.pageContainer.Margin = safeArea;
            shell.tabBarView.Margin = safeArea;
            shell.bottomBackgroundRectangle.IsVisible = safeArea.Bottom > 0;
            shell.bottomBackgroundRectangle.HeightRequest = safeArea.Bottom;
        });
    }

    private void AddTab(Type page, PageType pageEnum)
    {
        Tab tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
        tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });

        tabBar.Items.Add(tab);
    }

    private void TabBarViewCurrentPageChanged(object sender, TabBarEventArgs e)
    {
        Shell.Current.GoToAsync("///" + e.CurrentPage.ToString());
    }
}

public enum PageType
{
    PanelView, ProductionPanel, WorkstationPanel, ProductPanel, MaintenancePanel
}

