Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace DashboardMergeExample
	Friend NotInheritable Class Program

		Private Sub New()
		End Sub

		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		<STAThread>
		Shared Sub Main()
			DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = "The Bezier"
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			Application.Run(New DesignerForm())
		End Sub
	End Class
End Namespace
