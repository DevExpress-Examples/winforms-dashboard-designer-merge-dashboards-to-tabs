Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Xml.Linq
Imports DevExpress.DashboardCommon

Namespace DashboardMergeExample
	Public NotInheritable Class DataSourceMerger

		Private Sub New()
		End Sub

		Public Shared Sub MergeDataSources(ByVal fromDataSources As DataSourceCollection, ByVal dashboardMerger As DashboardMerger)
			Dim toDataSources As DataSourceCollection = dashboardMerger.TargetDashboard.DataSources

			For Each dataSource As IDashboardDataSource In fromDataSources
				AddDataSourceCopy(dataSource, dashboardMerger, Sub(dataSourceCopy)
					toDataSources.Add(dataSourceCopy)
				End Sub)
			Next dataSource
		End Sub

		Private Shared Sub AddDataSourceCopy(ByVal dataSource As IDashboardDataSource, ByVal dashboardMerger As DashboardMerger, ByVal addDataSourceDelegate As Action(Of IDashboardDataSource))
			Dim toDataSources As DataSourceCollection = dashboardMerger.TargetDashboard.DataSources
			Dim dataSourceNamesMap As IDictionary(Of String, String) = dashboardMerger.DataSourceNamesMap
			Dim dataSourceCopy As IDashboardDataSource = CreateDataSourceCopy(dataSource)
			If dataSourceCopy IsNot Nothing Then
				If toDataSources.Any(Function(d) d.ComponentName = dataSourceCopy.ComponentName) Then
					If ResolveNamesConflict(dataSourceCopy, toDataSources, dataSourceNamesMap) Then
						addDataSourceDelegate(dataSourceCopy)
					End If
				Else
					addDataSourceDelegate(dataSourceCopy)
				End If
			End If
		End Sub

		Private Shared Function ResolveNamesConflict(ByVal dataSourceCopy As IDashboardDataSource, ByVal toDataSources As DataSourceCollection, ByVal dataSourceNamesMap As IDictionary(Of String, String)) As Boolean

			' Provide your data source component names conflict resolution logic here

			Dim newName As String = NamesGenerator.GenerateName(dataSourceCopy.ComponentName, 1, toDataSources.Select(Function(ds) ds.ComponentName))
			dataSourceNamesMap.Add(dataSourceCopy.ComponentName, newName)
			dataSourceCopy.ComponentName = newName
			Return True
		End Function

		Private Shared Function CreateDataSourceCopy(ByVal dataSourceToCopy As IDashboardDataSource) As IDashboardDataSource
			Dim efDataSource As DashboardEFDataSource = TryCast(dataSourceToCopy, DashboardEFDataSource)
			If efDataSource IsNot Nothing Then
				Dim element As XElement = efDataSource.SaveToXml()
				Dim newDataSource As New DashboardEFDataSource()
				newDataSource.LoadFromXml(element)
				newDataSource.Fill()
				Return newDataSource
			End If

			Dim extractDataSource As DashboardExtractDataSource = TryCast(dataSourceToCopy, DashboardExtractDataSource)
			If extractDataSource IsNot Nothing Then
				Dim element As XElement = extractDataSource.SaveToXml()
				Dim newDataSource As New DashboardExtractDataSource()
				newDataSource.LoadFromXml(element)
				Return newDataSource
			End If

			Dim objectDataSource As DashboardObjectDataSource = TryCast(dataSourceToCopy, DashboardObjectDataSource)
			If objectDataSource IsNot Nothing Then
				Dim element As XElement = objectDataSource.SaveToXml()
				Dim newDataSource As New DashboardObjectDataSource()
				newDataSource.LoadFromXml(element)
				newDataSource.Fill()
				Return newDataSource
			End If

			Dim olapDataSource As DashboardOlapDataSource = TryCast(dataSourceToCopy, DashboardOlapDataSource)
			If olapDataSource IsNot Nothing Then
				Dim element As XElement = olapDataSource.SaveToXml()
				Dim newDataSource As New DashboardOlapDataSource()
				newDataSource.LoadFromXml(element)
				newDataSource.Fill()
				Return newDataSource
			End If

			Dim sqlDataSource As DashboardSqlDataSource = TryCast(dataSourceToCopy, DashboardSqlDataSource)
			If sqlDataSource IsNot Nothing Then
				Dim element As XElement = sqlDataSource.SaveToXml()
				Dim newDataSource As New DashboardSqlDataSource()
				newDataSource.LoadFromXml(element)
				newDataSource.Fill()
				Return newDataSource
			End If
			Return Nothing
		End Function
	End Class
End Namespace
