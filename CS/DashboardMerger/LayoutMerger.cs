using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;

namespace DashboardMergeExample {
    public static class LayoutMerger {
        public static void MergeLayout(DashboardLayoutGroup layoutRoot, string newPageName, DashboardMerger dashboardMerger) {
            TabContainerDashboardItem tabContainer = dashboardMerger.TabContainer;
            Dashboard targetDashboard = dashboardMerger.TargetDashboard;
            IDictionary<string, string> dashboardItemNamesMap = dashboardMerger.DashboardItemNamesMap;
            IDictionary<string, string> groupNamesMap = dashboardMerger.GroupNamesMap;
            IEnumerable<DashboardItem> newItems = dashboardMerger.NewItems;
            DashboardTabPage newTabPage = tabContainer.CreateTabPage();
            DashboardLayoutTabPage layoutPage = new DashboardLayoutTabPage(newTabPage);
            foreach(DashboardLayoutNode node in layoutRoot.GetNodesRecursive()) {
                if(node.DashboardItem != null) {
                    DashboardItemGroup group = node.DashboardItem as DashboardItemGroup;
                    if(group != null) {
                        string groupComponentName = group.ComponentName;
                        string newGroupComponentName = String.Empty;
                        if(!groupNamesMap.TryGetValue(group.ComponentName, out newGroupComponentName)) {
                            newGroupComponentName = group.ComponentName;
                        }
                        node.DashboardItem = newItems.Single(itm => itm.ComponentName == newGroupComponentName);
                    } else {
                        DashboardItem item = node.DashboardItem;
                        string newItemName = String.Empty;
                        if(!dashboardItemNamesMap.TryGetValue(item.ComponentName, out newItemName)) {
                            newItemName = item.ComponentName;
                        }
                        node.DashboardItem = newItems.Single(itm => itm.ComponentName == newItemName);
                    }
                }
            }
            layoutPage.ChildNodes.Add(layoutRoot);
            foreach(DashboardItem item in newItems) {
                if(item.ParentContainer == null) {
                    item.ParentContainer = newTabPage;
                } else {
                    IDashboardItemContainer container = item.ParentContainer;
                    if(container is DashboardItemGroup) {
                        string newGroupName = String.Empty;
                        if(!groupNamesMap.TryGetValue(container.ComponentName, out newGroupName)) {
                            newGroupName = container.ComponentName;
                        }
                        item.ParentContainer = targetDashboard.Groups[newGroupName];
                    } else {
                        item.ParentContainer = newTabPage;
                    }
                }
            }
            DashboardLayoutTabContainer layoutTabContainer = targetDashboard.LayoutRoot.FindRecursive(tabContainer);
            layoutTabContainer.ChildNodes.Add(layoutPage);
            newTabPage.Name = newPageName;
        }
    }
}
