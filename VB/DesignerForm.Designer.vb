Namespace DashboardMergeExample
	Partial Public Class DesignerForm
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.dashboardDesigner = New DevExpress.DashboardWin.DashboardDesigner()
			Me.SuspendLayout()
			' 
			' dashboardDesigner
			' 
			Me.dashboardDesigner.AllowMaximizeAnimation = True
			Me.dashboardDesigner.AllowMaximizeDashboardItems = True
			Me.dashboardDesigner.AllowPrintDashboard = True
			Me.dashboardDesigner.AllowPrintDashboardItems = True
			Me.dashboardDesigner.Dock = System.Windows.Forms.DockStyle.Fill
			Me.dashboardDesigner.Location = New System.Drawing.Point(0, 0)
			Me.dashboardDesigner.Name = "dashboardDesigner"
			Me.dashboardDesigner.Size = New System.Drawing.Size(962, 584)
			Me.dashboardDesigner.TabIndex = 0
			' 
			' DesignerForm
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(962, 584)
			Me.Controls.Add(Me.dashboardDesigner)
			Me.Name = "DesignerForm"
			Me.Text = "Dashboard Merger"
			Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
			Me.ResumeLayout(False)

		End Sub

		#End Region

		Private dashboardDesigner As DevExpress.DashboardWin.DashboardDesigner
	End Class
End Namespace

