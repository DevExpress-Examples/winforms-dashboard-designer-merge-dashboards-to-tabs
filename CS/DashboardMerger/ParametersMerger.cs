using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;

namespace DashboardMergeExample {
    public static class ParametersMerger {
        public static void MergeParameters(DashboardParameterCollection fromParameters, DashboardMerger dashboardMerger) {
            DashboardParameterCollection toParameters = dashboardMerger.TargetDashboard.Parameters;

            foreach(DashboardParameter parameter in fromParameters) {
                AddParamterCopy(parameter, dashboardMerger, (parameterCopy) => {
                    toParameters.Add(parameterCopy);
                });
            }
        }
        static void AddParamterCopy(DashboardParameter originalParameter, DashboardMerger dashboardMerger, Action<DashboardParameter> addParameterDelegate) {
            DashboardParameter parameterCopy = (DashboardParameter)originalParameter.Clone();
            DashboardParameterCollection toParameters = dashboardMerger.TargetDashboard.Parameters;
            if(toParameters.Any(p => p.Name == parameterCopy.Name)) {
                if(ResolveParameterNamesConflict(parameterCopy, originalParameter.Name, dashboardMerger))
                    addParameterDelegate(parameterCopy);
            } else {
                addParameterDelegate(parameterCopy);
            }
        }
        static bool ResolveParameterNamesConflict(DashboardParameter parameterCopy, string originalName, DashboardMerger dashboardMerger) {

            // Provide your parameter name confilict resolution logic here

            parameterCopy.Name = NamesGenerator.GenerateName("RenamedParameter", 1, dashboardMerger.TargetDashboard.Parameters.Select(p => p.Name));
            IEnumerable<DataDashboardItem> dataDashboardItems = dashboardMerger.TargetDashboard.Items.Where(item => item is DataDashboardItem).Cast<DataDashboardItem>();
            string originalNamePattern = String.Format("?{0}", originalName);
            string copyNamePattern = String.Format("?{0}", parameterCopy.Name);
            foreach(DataDashboardItem item in dataDashboardItems) {
                if(!String.IsNullOrEmpty(item.FilterString) && item.FilterString.Contains(originalNamePattern)) {
                    item.FilterString = item.FilterString.Replace(originalNamePattern, copyNamePattern);
                }
            }
            foreach(IDashboardDataSource dataSource in dashboardMerger.TargetDashboard.DataSources) {
                UpdateDataSourceParametersNames(dataSource, originalNamePattern, copyNamePattern);
            }

            return true;
        }
        static void UpdateDataSourceParametersNames(IDashboardDataSource dataSource, string originalNamePattern, string copyNamePattern) {
            DashboardSqlDataSource sqlDataSource = dataSource as DashboardSqlDataSource;
            if(sqlDataSource != null) {
                UpdateSqlDataSourceParameters(sqlDataSource, originalNamePattern, copyNamePattern);
            }
        }
        static void UpdateSqlDataSourceParameters(DashboardSqlDataSource sqlDataSource, string originalNamePattern, string copyNamePattern) {
            foreach(SqlQuery query in sqlDataSource.Queries) {
                foreach(QueryParameter parameter in query.Parameters) {
                    UpdateParameterExpression(parameter, originalNamePattern, copyNamePattern);
                }
            }
        }
        static void UpdateParameterExpression(QueryParameter parameter, string originalNamePattern, string copyNamePattern) {
            if(parameter.Type.Name == "Expression") {
                Expression parameterExpression = (Expression)parameter.Value;
                if(parameterExpression.ExpressionString.Contains(originalNamePattern))
                    parameterExpression.ExpressionString = parameterExpression.ExpressionString.Replace(originalNamePattern, copyNamePattern);
            }
        }
    }
}
