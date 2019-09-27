Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.DashboardCommon

Namespace DashboardMergeExample
	Public NotInheritable Class ItemsMerger

		Private Sub New()
		End Sub

		Public Shared Sub MergeGroups(ByVal fromGroups As DashboardItemGroupCollection, ByVal dashboardMerger As DashboardMerger)
			Dim toGroups As DashboardItemGroupCollection = dashboardMerger.TargetDashboard.Groups
			Dim newItems As IList(Of DashboardItem) = dashboardMerger.NewItems
			For Each group As DashboardItemGroup In fromGroups
				AddGroupCopy(group, dashboardMerger, Sub(groupCopy)
					toGroups.Add(groupCopy)
					newItems.Add(groupCopy)
				End Sub)
			Next group
		End Sub
		Public Shared Sub MergeItems(ByVal fromItems As DashboardItemCollection, ByVal dashboardMerger As DashboardMerger)
			Dim toItems As DashboardItemCollection = dashboardMerger.TargetDashboard.Items
			Dim newItems As IList(Of DashboardItem) = dashboardMerger.NewItems

			For Each dashboardItem As DashboardItem In fromItems
				AddItemCopy(dashboardItem, dashboardMerger, Sub(dashboardItemCopy)
					toItems.Add(dashboardItemCopy)
					newItems.Add(dashboardItemCopy)
				End Sub)
			Next dashboardItem
		End Sub
		Private Shared Sub AddGroupCopy(ByVal originalGroup As DashboardItemGroup, ByVal dashboardMerger As DashboardMerger, ByVal addGroupDelegate As Action(Of DashboardItemGroup))
			Dim toGroups As DashboardItemGroupCollection = dashboardMerger.TargetDashboard.Groups
			Dim groupCopy As DashboardItemGroup = CreateGroupCopy(originalGroup)
			If toGroups.Any(Function(g) g.ComponentName = originalGroup.ComponentName) Then
				If ResolveGroupNamesConflict(groupCopy, originalGroup.ComponentName, toGroups, dashboardMerger.GroupNamesMap) Then
					addGroupDelegate(groupCopy)
				End If
			Else
				addGroupDelegate(groupCopy)
			End If
		End Sub
		Private Shared Function ResolveGroupNamesConflict(ByVal groupCopy As DashboardItemGroup, ByVal originalGroupName As String, ByVal toGroups As IEnumerable(Of DashboardItem), ByVal groupNamesMap As IDictionary(Of String, String)) As Boolean

			' Provide your group component name conflict resolution logic here.

			Dim newName As String = NamesGenerator.GenerateName(originalGroupName, 1, toGroups.Select(Function(g) g.ComponentName))
			groupNamesMap.Add(originalGroupName, newName)
			groupCopy.ComponentName = newName
			Return True
		End Function
		Private Shared Sub AddItemCopy(ByVal originalItem As DashboardItem, ByVal dashboardMerger As DashboardMerger, ByVal addItemDelegate As Action(Of DashboardItem))
			Dim toItems As DashboardItemCollection = dashboardMerger.TargetDashboard.Items
			Dim dataSourceNamesMap As IDictionary(Of String, String) = dashboardMerger.DataSourceNamesMap
			Dim existingDataSources As DataSourceCollection = dashboardMerger.TargetDashboard.DataSources
			Dim dashboardItemCopy As DashboardItem = originalItem.CreateCopy()

			Dim shouldAddItem As Boolean = False
			If toItems.Any(Function(item) item.ComponentName = originalItem.ComponentName) Then
				If ResolveDashboardItemNameConflict(dashboardItemCopy, originalItem.ComponentName, toItems, dashboardMerger.DashboardItemNamesMap) Then
					shouldAddItem = True
				End If
			Else
				dashboardItemCopy.ComponentName = originalItem.ComponentName
				shouldAddItem = True
			End If
			If shouldAddItem Then
				Dim dataDashboardItem As DataDashboardItem = TryCast(dashboardItemCopy, DataDashboardItem)
				If dataDashboardItem IsNot Nothing AndAlso dataDashboardItem.DataSource IsNot Nothing Then
					Dim newDataSourceName As String = String.Empty
					If Not dataSourceNamesMap.TryGetValue(dataDashboardItem.DataSource.ComponentName, newDataSourceName) Then
						newDataSourceName = dataDashboardItem.DataSource.ComponentName
					End If
					dataDashboardItem.DataSource = existingDataSources(newDataSourceName)
				End If
				addItemDelegate(dashboardItemCopy)
			End If
		End Sub
		Private Shared Function ResolveDashboardItemNameConflict(ByVal dashboardItemCopy As DashboardItem, ByVal originalItemName As String, ByVal toItems As DashboardItemCollection, ByVal dashboardItemNamesMap As IDictionary(Of String, String)) As Boolean

			' Provide your item component name confilict resolution logic here

			Dim newName As String = NamesGenerator.GenerateName(originalItemName, 1, toItems.Select(Function(item) item.ComponentName))
			dashboardItemNamesMap.Add(originalItemName, newName)
			dashboardItemCopy.ComponentName = newName
			Return True
		End Function
		Private Shared Function CreateGroupCopy(ByVal group As DashboardItemGroup) As DashboardItemGroup
			Dim groupCopy As New DashboardItemGroup()
			groupCopy.InteractivityOptions.IgnoreMasterFilters = group.InteractivityOptions.IgnoreMasterFilters
			groupCopy.InteractivityOptions.IsMasterFilter = group.InteractivityOptions.IsMasterFilter
			groupCopy.Name = group.Name
			groupCopy.ShowCaption = group.ShowCaption
			Return groupCopy
		End Function
	End Class
End Namespace
