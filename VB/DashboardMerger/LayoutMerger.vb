Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.DashboardCommon

Namespace DashboardMergeExample
    Public NotInheritable Class LayoutMerger

        Private Sub New()
        End Sub

        Public Shared Sub MergeLayout(ByVal layoutRoot As DashboardLayoutGroup, ByVal newPageName As String, ByVal dashboardMerger As DashboardMerger)
            Dim tabContainer As TabContainerDashboardItem = dashboardMerger.TabContainer
            Dim targetDashboard As Dashboard = dashboardMerger.TargetDashboard
            Dim dashboardItemNamesMap As IDictionary(Of String, String) = dashboardMerger.DashboardItemNamesMap
            Dim groupNamesMap As IDictionary(Of String, String) = dashboardMerger.GroupNamesMap
            Dim newItems As IEnumerable(Of DashboardItem) = dashboardMerger.NewItems
            Dim newTabPage As DashboardTabPage = tabContainer.CreateTabPage()
            Dim layoutPage As New DashboardLayoutTabPage(newTabPage)
            For Each node As DashboardLayoutNode In layoutRoot.GetNodesRecursive()
                If node.DashboardItem IsNot Nothing Then
                    Dim group As DashboardItemGroup = TryCast(node.DashboardItem, DashboardItemGroup)
                    If group IsNot Nothing Then
                        Dim groupComponentName As String = group.ComponentName
                        Dim newGroupComponentName As String = String.Empty
                        If Not groupNamesMap.TryGetValue(group.ComponentName, newGroupComponentName) Then
                            newGroupComponentName = group.ComponentName
                        End If
                        node.DashboardItem = newItems.Single(Function(itm) itm.ComponentName = newGroupComponentName)
                    Else
                        Dim item As DashboardItem = node.DashboardItem
                        Dim newItemName As String = String.Empty
                        If Not dashboardItemNamesMap.TryGetValue(item.ComponentName, newItemName) Then
                            newItemName = item.ComponentName
                        End If
                        node.DashboardItem = newItems.Single(Function(itm) itm.ComponentName = newItemName)
                    End If
                End If
            Next node
            layoutPage.ChildNodes.Add(layoutRoot)
            For Each item As DashboardItem In newItems
                If item.ParentContainer Is Nothing Then
                    item.ParentContainer = newTabPage
                Else
                    Dim container As IDashboardItemContainer = item.ParentContainer
                    If TypeOf container Is DashboardItemGroup Then
                        Dim newGroupName As String = String.Empty
                        If Not groupNamesMap.TryGetValue(container.ComponentName, newGroupName) Then
                            newGroupName = container.ComponentName
                        End If
                        item.ParentContainer = targetDashboard.Groups(newGroupName)
                    Else
                        item.ParentContainer = newTabPage
                    End If
                End If
            Next item
            Dim layoutTabContainer As DashboardLayoutTabContainer = targetDashboard.LayoutRoot.FindRecursive(tabContainer)
            layoutTabContainer.ChildNodes.Add(layoutPage)
            newTabPage.Name = newPageName
        End Sub
    End Class
End Namespace
