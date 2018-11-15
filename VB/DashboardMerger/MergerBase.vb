Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace DashboardMergeExample
    Public NotInheritable Class NamesGenerator

        Private Sub New()
        End Sub

        Public Shared Function GenerateName(ByVal name As String, ByVal index As Integer, ByVal occupiedNames As IEnumerable(Of String)) As String
            Dim result As String = String.Format("{0}_{1}", name, index)
            If occupiedNames.Contains(result) Then
                index += 1
                Return GenerateName(name, index, occupiedNames)
            End If
            Return result
        End Function
    End Class
End Namespace
