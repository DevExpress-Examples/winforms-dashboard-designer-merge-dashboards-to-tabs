Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DevExpress.DashboardCommon
Imports DevExpress.DataAccess
Imports DevExpress.DataAccess.Sql

Namespace DashboardMergeExample
    Public NotInheritable Class ParametersMerger

        Private Sub New()
        End Sub

        Public Shared Sub MergeParameters(ByVal fromParameters As DashboardParameterCollection, ByVal dashboardMerger As DashboardMerger)
            Dim toParameters As DashboardParameterCollection = dashboardMerger.TargetDashboard.Parameters

            For Each parameter As DashboardParameter In fromParameters
                AddParamterCopy(parameter, dashboardMerger, Sub(parameterCopy)
                    toParameters.Add(parameterCopy)
                End Sub)
            Next parameter
        End Sub
        Private Shared Sub AddParamterCopy(ByVal originalParameter As DashboardParameter, ByVal dashboardMerger As DashboardMerger, ByVal addParameterDelegate As Action(Of DashboardParameter))
            Dim parameterCopy As DashboardParameter = DirectCast(originalParameter.Clone(), DashboardParameter)
            Dim toParameters As DashboardParameterCollection = dashboardMerger.TargetDashboard.Parameters
            If toParameters.Any(Function(p) p.Name = parameterCopy.Name) Then
                If ResolveParameterNamesConflict(parameterCopy, originalParameter.Name, dashboardMerger) Then
                    addParameterDelegate(parameterCopy)
                End If
            Else
                addParameterDelegate(parameterCopy)
            End If
        End Sub
        Private Shared Function ResolveParameterNamesConflict(ByVal parameterCopy As DashboardParameter, ByVal originalName As String, ByVal dashboardMerger As DashboardMerger) As Boolean

            ' Provide your parameter name confilict resolution logic here

            parameterCopy.Name = NamesGenerator.GenerateName("RenamedParameter", 1, dashboardMerger.TargetDashboard.Parameters.Select(Function(p) p.Name))
            Dim dataDashboardItems As IEnumerable(Of DataDashboardItem) = dashboardMerger.TargetDashboard.Items.Where(Function(item) TypeOf item Is DataDashboardItem).Cast(Of DataDashboardItem)()
            Dim originalNamePattern As String = String.Format("?{0}", originalName)
            Dim copyNamePattern As String = String.Format("?{0}", parameterCopy.Name)
            For Each item As DataDashboardItem In dataDashboardItems
                If Not String.IsNullOrEmpty(item.FilterString) AndAlso item.FilterString.Contains(originalNamePattern) Then
                    item.FilterString = item.FilterString.Replace(originalNamePattern, copyNamePattern)
                End If
            Next item
            For Each dataSource As IDashboardDataSource In dashboardMerger.TargetDashboard.DataSources
                UpdateDataSourceParametersNames(dataSource, originalNamePattern, copyNamePattern)
            Next dataSource

            Return True
        End Function
        Private Shared Sub UpdateDataSourceParametersNames(ByVal dataSource As IDashboardDataSource, ByVal originalNamePattern As String, ByVal copyNamePattern As String)
            Dim sqlDataSource As DashboardSqlDataSource = TryCast(dataSource, DashboardSqlDataSource)
            If sqlDataSource IsNot Nothing Then
                UpdateSqlDataSourceParameters(sqlDataSource, originalNamePattern, copyNamePattern)
            End If
        End Sub
        Private Shared Sub UpdateSqlDataSourceParameters(ByVal sqlDataSource As DashboardSqlDataSource, ByVal originalNamePattern As String, ByVal copyNamePattern As String)
            For Each query As SqlQuery In sqlDataSource.Queries
                For Each parameter As QueryParameter In query.Parameters
                    UpdateParameterExpression(parameter, originalNamePattern, copyNamePattern)
                Next parameter
            Next query
        End Sub
        Private Shared Sub UpdateParameterExpression(ByVal parameter As QueryParameter, ByVal originalNamePattern As String, ByVal copyNamePattern As String)
            If parameter.Type.Name = "Expression" Then
                Dim parameterExpression As Expression = CType(parameter.Value, Expression)
                If parameterExpression.ExpressionString.Contains(originalNamePattern) Then
                    parameterExpression.ExpressionString = parameterExpression.ExpressionString.Replace(originalNamePattern, copyNamePattern)
                End If
            End If
        End Sub
    End Class
End Namespace
