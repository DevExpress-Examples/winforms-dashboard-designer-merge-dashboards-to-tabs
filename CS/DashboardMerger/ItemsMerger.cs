using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;

namespace DashboardMerger {
    public static class ItemsMerger {
        public static void MergeGroups(DashboardItemGroupCollection fromGroups, DashboardMerger dashboardMerger) {
            DashboardItemGroupCollection toGroups = dashboardMerger.OriginalDashboard.Groups;
            IList<DashboardItem> newItems = dashboardMerger.NewItems;
            foreach(DashboardItemGroup group in fromGroups) {
                AddGroupCopy(group, dashboardMerger, (groupCopy) => {
                    toGroups.Add(groupCopy);
                    newItems.Add(groupCopy);
                });
            }
        }
        public static void MergeItems(DashboardItemCollection fromItems, DashboardMerger dashboardMerger) {
            DashboardItemCollection toItems = dashboardMerger.OriginalDashboard.Items;
            IList<DashboardItem> newItems = dashboardMerger.NewItems;

            foreach(DashboardItem dashboardItem in fromItems) {
                AddItemCopy(dashboardItem, dashboardMerger, (dashboardItemCopy) => {
                    toItems.Add(dashboardItemCopy);
                    newItems.Add(dashboardItemCopy);
                });
            }
        }
        static void AddGroupCopy(DashboardItemGroup originalGroup, DashboardMerger dashboardMerger, Action<DashboardItemGroup> addGroupDelegate) {
            DashboardItemGroupCollection toGroups = dashboardMerger.OriginalDashboard.Groups;
            DashboardItemGroup groupCopy = CreateGroupCopy(originalGroup);
            if(toGroups.Any(g => g.ComponentName == originalGroup.ComponentName)) {
                if(ResolveGroupNamesConflict(groupCopy, originalGroup.ComponentName, toGroups, dashboardMerger.GroupNamesMap))
                    addGroupDelegate(groupCopy);
            } else {
                addGroupDelegate(groupCopy);
            }
        }
        static bool ResolveGroupNamesConflict(DashboardItemGroup groupCopy, string originalGroupName, IEnumerable<DashboardItem> toGroups, IDictionary<string, string> groupNamesMap) {
            
            // Provide your group component name confilict resolution logic here

            string newName = NamesGenerator.GenerateName(originalGroupName, 1, toGroups.Select(g => g.ComponentName));
            groupNamesMap.Add(originalGroupName, newName);
            groupCopy.ComponentName = newName;
            return true;
        }
        static void AddItemCopy(DashboardItem originalItem, DashboardMerger dashboardMerger, Action<DashboardItem> addItemDelegate) {
            DashboardItemCollection toItems = dashboardMerger.OriginalDashboard.Items;
            IDictionary<string, string> dataSourceNamesMap = dashboardMerger.DataSourceNamesMap;
            DataSourceCollection existingDataSources = dashboardMerger.OriginalDashboard.DataSources;
            DashboardItem dashboardItemCopy = originalItem.CreateCopy();

            bool shouldAddItem = false;
            if(toItems.Any(item => item.ComponentName == originalItem.ComponentName)) {
                if(ResolveDashboardItemNameConflict(dashboardItemCopy, originalItem.ComponentName, toItems, dashboardMerger.DashboardItemNamesMap))
                    shouldAddItem = true;
            } else {
                dashboardItemCopy.ComponentName = originalItem.ComponentName;
                shouldAddItem = true;
            }
            if(shouldAddItem) {
                DataDashboardItem dataDashboardItem = dashboardItemCopy as DataDashboardItem;
                if(dataDashboardItem != null && dataDashboardItem.DataSource != null) {
                    string newDataSourceName = String.Empty;
                    if(!dataSourceNamesMap.TryGetValue(dataDashboardItem.DataSource.ComponentName, out newDataSourceName)) {
                        newDataSourceName = dataDashboardItem.DataSource.ComponentName;
                    }
                    dataDashboardItem.DataSource = existingDataSources[newDataSourceName];
                }
                addItemDelegate(dashboardItemCopy);
            }
        }
        static bool ResolveDashboardItemNameConflict(DashboardItem dashboardItemCopy, string originalItemName, DashboardItemCollection toItems, IDictionary<string, string> dashboardItemNamesMap) {

            // Provide your item component name confilict resolution logic here

            string newName = NamesGenerator.GenerateName(originalItemName, 1, toItems.Select(item => item.ComponentName));
            dashboardItemNamesMap.Add(originalItemName, newName);
            dashboardItemCopy.ComponentName = newName;
            return true;
        }
        static DashboardItemGroup CreateGroupCopy(DashboardItemGroup group) {
            DashboardItemGroup groupCopy = new DashboardItemGroup();
            groupCopy.InteractivityOptions.IgnoreMasterFilters = group.InteractivityOptions.IgnoreMasterFilters;
            groupCopy.InteractivityOptions.IsMasterFilter = group.InteractivityOptions.IsMasterFilter;
            groupCopy.Name = group.Name;
            groupCopy.ShowCaption = group.ShowCaption;
            return groupCopy;
        }
    }
}
