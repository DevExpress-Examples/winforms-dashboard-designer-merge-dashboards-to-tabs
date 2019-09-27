Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWin
Imports DevExpress.Utils.Svg
Imports DevExpress.XtraEditors
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms

Namespace DashboardMergeExample
	Partial Public Class DesignerForm
		Inherits XtraForm

		Private dashboardChanged As Boolean = False
		Public Sub New()
			InitializeComponent()
			dashboardDesigner.CreateRibbon()
			AddHandler dashboardDesigner.CustomizeDashboardTitle, AddressOf DashboardDesignerCustomizeDashboardTitle
			dashboardDesigner.UpdateDashboardTitle()
			AddHandler dashboardDesigner.DashboardClosing, AddressOf DashboardDesignerDashboardClosing
		End Sub

		' The method that performs the merge.
		Private Sub MergeDashboard(ByVal args As DashboardToolbarItemClickEventArgs)
			' Invoke a file dialog to select multiple dashboards to merge.
			' If one or several selected dashboards have tabbed layout and cannot be merged, a message is shown. 
			Dim openFileDialog As New OpenFileDialog()
			openFileDialog.Filter = "Dashboard files (*.xml)|*.xml"
			openFileDialog.InitialDirectory = Application.StartupPath & "\Dashboards"
			openFileDialog.Multiselect = True
			If openFileDialog.ShowDialog() = DialogResult.OK Then
				dashboardDesigner.Dashboard.BeginUpdate()
				Try
					Dim rejectedDashboard As New List(Of String)()
					For Each fileName As String In openFileDialog.FileNames
						Using dashboard As New Dashboard()
							dashboard.LoadFromXml(fileName)
							' The DashboardMerger instance is the key object that performs the merge.
							Dim dashboardMerger As New DashboardMerger(dashboardDesigner.Dashboard)
							If Not dashboardMerger.MergeDashboard(dashboard) Then
								rejectedDashboard.Add(Path.GetFileName(fileName))
							End If
						End Using
					Next fileName
					If rejectedDashboard.Count > 0 Then
						MessageBox.Show(String.Format("Cannot merge the following dashboard(s): {0}{1}", Environment.NewLine, String.Join(Environment.NewLine, rejectedDashboard)))
					End If
					If (openFileDialog.FileNames.Length - rejectedDashboard.Count) > 0 Then
						dashboardChanged = True
					End If
				Finally
					dashboardDesigner.Dashboard.EndUpdate()
					dashboardDesigner.ReloadData()
				End Try
			End If
		End Sub

		Private Sub DashboardDesignerDashboardClosing(ByVal sender As Object, ByVal e As DashboardClosingEventArgs)
			If dashboardChanged Then
				e.IsDashboardModified = True
			End If
		End Sub

		' The DashboardDesigner.CustomizeDashboardTitle event handler.
		' Create a custom button in the dashboard title with a click action that calls the MergeDashboard method.
		Private Sub DashboardDesignerCustomizeDashboardTitle(ByVal sender As Object, ByVal e As CustomizeDashboardTitleEventArgs)
			Dim mergeItem As New DashboardToolbarItem("Open the dashboard(s) to merge", AddressOf MergeDashboard)
			mergeItem.SvgImage = CType(My.Resources.MergeIcon, SvgImage)
			mergeItem.Caption = "Merge Dashboard"
			e.Items.Insert(0, mergeItem)
		End Sub
	End Class
End Namespace
