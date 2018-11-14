using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;

namespace DashboardMerger {
    public class DashboardMerger {
        IList<DashboardItem> newItems = new List<DashboardItem>();
        Dictionary<string, string> dataSourceNamesMap = new Dictionary<string, string>();
        Dictionary<string, string> groupNamesMap = new Dictionary<string, string>();
        Dictionary<string, string> dashboardItemNamesMap = new Dictionary<string, string>();

        public TabContainerDashboardItem TabContainer { get; private set; }
        public Dashboard OriginalDashboard { get; }
        public IList<DashboardItem> NewItems { get { return newItems; } }

        public Dictionary<string, string> DataSourceNamesMap { get { return dataSourceNamesMap; } }
        public Dictionary<string, string> GroupNamesMap { get { return groupNamesMap; } }
        public Dictionary<string, string> DashboardItemNamesMap { get { return dashboardItemNamesMap; } }

        IEnumerable<DashboardItem> ItemsAndGroups { get { return OriginalDashboard.Items.Union(OriginalDashboard.Groups).Where(item => !(item is TabContainerDashboardItem)); } }

        public DashboardMerger(Dashboard originalDashboard) {
            OriginalDashboard = originalDashboard;
        }

        public bool MergeDashboard(Dashboard dashboard) {
            if(!CheckDashboard(dashboard))
                return false;
            UpdateTabContainer();
            DataSourceMerger.MergeDataSources(dashboard.DataSources, this);
            ItemsMerger.MergeGroups(dashboard.Groups, this);
            ItemsMerger.MergeItems(dashboard.Items, this);
            ParametersMerger.MergeParameters(dashboard.Parameters, this);
            LayoutMerger.MergeLayout(dashboard.LayoutRoot, dashboard.Title.Text, this);
            return true;
        }

        bool CheckDashboard(Dashboard dashboard) {
            return dashboard.Items.All(item => !(item is TabContainerDashboardItem));
        }
        void UpdateTabContainer() {
            TabContainer = OriginalDashboard.Items.FirstOrDefault(item => item is TabContainerDashboardItem) as TabContainerDashboardItem;
            if(TabContainer == null) {
                CreateTabContainer();
                DashboardLayoutTabContainer layoutTabContainer = new DashboardLayoutTabContainer(TabContainer, 1);
                if(ItemsAndGroups.Count() > 0) {
                    DashboardTabPage tabPage = TabContainer.CreateTabPage();
                    tabPage.Name = OriginalDashboard.Title.Text;
                    DashboardLayoutTabPage layoutPage = new DashboardLayoutTabPage(tabPage);
                    layoutTabContainer.ChildNodes.Add(layoutPage);
                    MoveRootToTabPage(layoutPage);
                    SetParentContainer(tabPage);
                }
                OriginalDashboard.LayoutRoot = new DashboardLayoutGroup();
                OriginalDashboard.LayoutRoot.ChildNodes.Add(layoutTabContainer);
            }
        }

        void CreateTabContainer() {
            TabContainer = new TabContainerDashboardItem();
            OriginalDashboard.Items.Add(TabContainer);
        }
        void MoveRootToTabPage(DashboardLayoutTabPage layoutPage) {
            DashboardLayoutGroup rootGroup = OriginalDashboard.LayoutRoot;
            OriginalDashboard.LayoutRoot = null;
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
