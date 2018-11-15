using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;

namespace DashboardMergeExample
{
    public class DashboardMerger {
        IList<DashboardItem> newItems = new List<DashboardItem>();
        Dictionary<string, string> dataSourceNamesMap = new Dictionary<string, string>();
        Dictionary<string, string> groupNamesMap = new Dictionary<string, string>();
        Dictionary<string, string> dashboardItemNamesMap = new Dictionary<string, string>();

        public TabContainerDashboardItem TabContainer { get; private set; }
        public Dashboard TargetDashboard { get; }
        public IList<DashboardItem> NewItems { get { return newItems; } }

        public Dictionary<string, string> DataSourceNamesMap { get { return dataSourceNamesMap; } }
        public Dictionary<string, string> GroupNamesMap { get { return groupNamesMap; } }
        public Dictionary<string, string> DashboardItemNamesMap { get { return dashboardItemNamesMap; } }

        IEnumerable<DashboardItem> ItemsAndGroups { get { return TargetDashboard.Items.Union(TargetDashboard.Groups).Where(item => !(item is TabContainerDashboardItem)); } }

        public DashboardMerger(Dashboard targetDashboard) {
            TargetDashboard = targetDashboard;
        }

        public bool MergeDashboard(Dashboard dashboard) {
            // Check whether the dashboard has a tabbed layout.
            if (!CheckDashboard(dashboard))
                return false;
            // If the target dashboard loaded in the designer has no tab container, creates an empty tab container.
            UpdateTabContainer();
            // Copy data sources from the specified dashboard to the target dashboard.
            // Resolve name conflicts and add new and original names to the DataSourceNamesMap dictionary.
            DataSourceMerger.MergeDataSources(dashboard.DataSources, this);
            // Copy groups from the specified dashboard to the target dashboard.
            // Resolve name conflicts and add new and original names to the GroupNamesMap dictionary.
            ItemsMerger.MergeGroups(dashboard.Groups, this);
            // Copy dashboard items from the specified dashboard to the target dashboard.
            // Resolve name conflicts and add new and original names to the DashboardItemNamesMap dictionary.
            // Update data source names using the DataSourceNamesMap dictionary.
            ItemsMerger.MergeItems(dashboard.Items, this);
            // Copy parameters from the specified dashboard to the target dashboard.
            // Resolve name conflicts.
            // Update parameter names in expressions used in data source queries.
            ParametersMerger.MergeParameters(dashboard.Parameters, this);
            // Change item names in the dashboard layout using GroupNamesMap and DashboardItemNamesMap dictionaries.
            // Set the source dashboard's layout root as a child node of a new layout tab page in the target dashboard.
            // Specify the target dashboard's layout tab page as the parent container instead of the former layout root group.
            LayoutMerger.MergeLayout(dashboard.LayoutRoot, dashboard.Title.Text, this);
            return true;
        }

        bool CheckDashboard(Dashboard dashboard) {
            return dashboard.Items.All(item => !(item is TabContainerDashboardItem));
        }
        void UpdateTabContainer() {
            TabContainer = TargetDashboard.Items.FirstOrDefault(item => item is TabContainerDashboardItem) as TabContainerDashboardItem;
            if(TabContainer == null) {
                CreateTabContainer();
                DashboardLayoutTabContainer layoutTabContainer = new DashboardLayoutTabContainer(TabContainer, 1);
                if(ItemsAndGroups.Count() > 0) {
                    DashboardTabPage tabPage = TabContainer.CreateTabPage();
                    tabPage.Name = TargetDashboard.Title.Text;
                    DashboardLayoutTabPage layoutPage = new DashboardLayoutTabPage(tabPage);
                    layoutTabContainer.ChildNodes.Add(layoutPage);
                    MoveRootToTabPage(layoutPage);
                    SetParentContainer(tabPage);
                }
                TargetDashboard.LayoutRoot = new DashboardLayoutGroup();
                TargetDashboard.LayoutRoot.ChildNodes.Add(layoutTabContainer);
            }
        }

        void CreateTabContainer() {
            TabContainer = new TabContainerDashboardItem();
            TargetDashboard.Items.Add(TabContainer);
        }
        void MoveRootToTabPage(DashboardLayoutTabPage layoutPage) {
            DashboardLayoutGroup rootGroup = TargetDashboard.LayoutRoot;
            TargetDashboard.LayoutRoot = null;
            layoutPage.ChildNodes.Add(rootGroup);
        }
        void SetParentContainer(IDashboardItemContainer container) {
            foreach(DashboardItem item in ItemsAndGroups) {
                if(item.ParentContainer == null) {
                    if(!(item is TabContainerDashboardItem))
                        item.ParentContainer = container;
                }
            }
        }
    }
}
