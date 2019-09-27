Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.DashboardCommon

Namespace DashboardMergeExample
	Public Class DashboardMerger
'INSTANT VB NOTE: The variable newItems was renamed since Visual Basic does not allow variables and other class members to have the same name:
		Private newItems_Renamed As IList(Of DashboardItem) = New List(Of DashboardItem)()
'INSTANT VB NOTE: The variable dataSourceNamesMap was renamed since Visual Basic does not allow variables and other class members to have the same name:
		Private dataSourceNamesMap_Renamed As New Dictionary(Of String, String)()
'INSTANT VB NOTE: The variable groupNamesMap was renamed since Visual Basic does not allow variables and other class members to have the same name:
		Private groupNamesMap_Renamed As New Dictionary(Of String, String)()
'INSTANT VB NOTE: The variable dashboardItemNamesMap was renamed since Visual Basic does not allow variables and other class members to have the same name:
		Private dashboardItemNamesMap_Renamed As New Dictionary(Of String, String)()

		Private privateTabContainer As TabContainerDashboardItem
		Public Property TabContainer() As TabContainerDashboardItem
			Get
				Return privateTabContainer
			End Get
			Private Set(ByVal value As TabContainerDashboardItem)
				privateTabContainer = value
			End Set
		End Property
		Public ReadOnly Property TargetDashboard() As Dashboard
		Public ReadOnly Property NewItems() As IList(Of DashboardItem)
			Get
				Return newItems_Renamed
			End Get
		End Property

		Public ReadOnly Property DataSourceNamesMap() As Dictionary(Of String, String)
			Get
				Return dataSourceNamesMap_Renamed
			End Get
		End Property
		Public ReadOnly Property GroupNamesMap() As Dictionary(Of String, String)
			Get
				Return groupNamesMap_Renamed
			End Get
		End Property
		Public ReadOnly Property DashboardItemNamesMap() As Dictionary(Of String, String)
			Get
				Return dashboardItemNamesMap_Renamed
			End Get
		End Property

		Private ReadOnly Property ItemsAndGroups() As IEnumerable(Of DashboardItem)
			Get
				Return TargetDashboard.Items.Union(TargetDashboard.Groups).Where(Function(item) Not (TypeOf item Is TabContainerDashboardItem))
			End Get
		End Property

		Public Sub New(ByVal targetDashboard As Dashboard)
			Me.TargetDashboard = targetDashboard
		End Sub

		Public Function MergeDashboard(ByVal dashboard As Dashboard) As Boolean
			' Check whether the dashboard has a tabbed layout.
			If Not CheckDashboard(dashboard) Then
				Return False
			End If
			' If the target dashboard loaded in the designer has no tab container, creates an empty tab container.
			UpdateTabContainer()
			' Copy data sources from the specified dashboard to the target dashboard.
			' Resolve name conflicts and add new and original names to the DataSourceNamesMap dictionary.
			DataSourceMerger.MergeDataSources(dashboard.DataSources, Me)
			' Copy groups from the specified dashboard to the target dashboard.
			' Resolve name conflicts and add new and original names to the GroupNamesMap dictionary.
			ItemsMerger.MergeGroups(dashboard.Groups, Me)
			' Copy dashboard items from the specified dashboard to the target dashboard.
			' Resolve name conflicts and add new and original names to the DashboardItemNamesMap dictionary.
			' Update data source names using the DataSourceNamesMap dictionary.
			ItemsMerger.MergeItems(dashboard.Items, Me)
			' Copy parameters from the specified dashboard to the target dashboard.
			' Resolve name conflicts.
			' Update parameter names in expressions used in data source queries.
			ParametersMerger.MergeParameters(dashboard.Parameters, Me)
			' Change item names in the dashboard layout using GroupNamesMap and DashboardItemNamesMap dictionaries.
			' Set the source dashboard's layout root as a child node of a new layout tab page in the target dashboard.
			' Specify the target dashboard's layout tab page as the parent container instead of the former layout root group.
			LayoutMerger.MergeLayout(dashboard.LayoutRoot, dashboard.Title.Text, Me)
			Return True
		End Function

		Private Function CheckDashboard(ByVal dashboard As Dashboard) As Boolean
			Return dashboard.Items.All(Function(item) Not (TypeOf item Is TabContainerDashboardItem))
		End Function
		Private Sub UpdateTabContainer()
			TabContainer = TryCast(TargetDashboard.Items.FirstOrDefault(Function(item) TypeOf item Is TabContainerDashboardItem), TabContainerDashboardItem)
			If TabContainer Is Nothing Then
				CreateTabContainer()
				Dim layoutTabContainer As New DashboardLayoutTabContainer(TabContainer, 1)
				If ItemsAndGroups.Count() > 0 Then
					Dim tabPage As DashboardTabPage = TabContainer.CreateTabPage()
					tabPage.Name = TargetDashboard.Title.Text
					Dim layoutPage As New DashboardLayoutTabPage(tabPage)
					layoutTabContainer.ChildNodes.Add(layoutPage)
					MoveRootToTabPage(layoutPage)
					SetParentContainer(tabPage)
				End If
				TargetDashboard.LayoutRoot = New DashboardLayoutGroup()
				TargetDashboard.LayoutRoot.ChildNodes.Add(layoutTabContainer)
			End If
		End Sub

		Private Sub CreateTabContainer()
			TabContainer = New TabContainerDashboardItem()
			TargetDashboard.Items.Add(TabContainer)
		End Sub
		Private Sub MoveRootToTabPage(ByVal layoutPage As DashboardLayoutTabPage)
			Dim rootGroup As DashboardLayoutGroup = TargetDashboard.LayoutRoot
			TargetDashboard.LayoutRoot = Nothing
			layoutPage.ChildNodes.Add(rootGroup)
		End Sub
		Private Sub SetParentContainer(ByVal container As IDashboardItemContainer)
			For Each item As DashboardItem In ItemsAndGroups
				If item.ParentContainer Is Nothing Then
					If Not (TypeOf item Is TabContainerDashboardItem) Then
						item.ParentContainer = container
					End If
				End If
			Next item
		End Sub
	End Class
End Namespace
