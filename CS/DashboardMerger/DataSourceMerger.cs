using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon;

namespace DashboardMerger {
    public static class DataSourceMerger {
        public static void MergeDataSources(DataSourceCollection fromDataSources, DashboardMerger dashboardMerger) {
            DataSourceCollection toDataSources = dashboardMerger.OriginalDashboard.DataSources;

            foreach(IDashboardDataSource dataSource in fromDataSources) {
                AddDataSourceCopy(dataSource, dashboardMerger, (dataSourceCopy) => {
                    toDataSources.Add(dataSourceCopy);
                });
            }
        }

        static void AddDataSourceCopy(IDashboardDataSource dataSource, DashboardMerger dashboardMerger, Action<IDashboardDataSource> addDataSourceDelegate) {
            DataSourceCollection toDataSources = dashboardMerger.OriginalDashboard.DataSources;
            IDictionary<string, string> dataSourceNamesMap = dashboardMerger.DataSourceNamesMap;
            IDashboardDataSource dataSourceCopy = CreateDataSourceCopy(dataSource);
            if(dataSourceCopy != null) {
                if(toDataSources.Any(d => d.ComponentName == dataSourceCopy.ComponentName)) {
                    if(ResolveNamesConflict(dataSourceCopy, toDataSources, dataSourceNamesMap))
                        addDataSourceDelegate(dataSourceCopy);
                } else {
                    addDataSourceDelegate(dataSourceCopy);
                }
            }
        }

        static bool ResolveNamesConflict(IDashboardDataSource dataSourceCopy, DataSourceCollection toDataSources, IDictionary<string, string> dataSourceNamesMap) {
            
            // Provide your data source component names confilict resolution logic here

            string newName = NamesGenerator.GenerateName(dataSourceCopy.ComponentName, 1, toDataSources.Select(ds => ds.ComponentName));
            dataSourceNamesMap.Add(dataSourceCopy.ComponentName, newName);
            dataSourceCopy.ComponentName = newName;
            return true;
        }

        static IDashboardDataSource CreateDataSourceCopy(IDashboardDataSource dataSourceToCopy) {
            DashboardEFDataSource efDataSource = dataSourceToCopy as DashboardEFDataSource;
            if(efDataSource != null) {
                XElement element = efDataSource.SaveToXml();
                DashboardEFDataSource newDataSource = new DashboardEFDataSource();
                newDataSource.LoadFromXml(element);
                //newDataSource.Fill();
                return newDataSource;
            }

            DashboardExcelDataSource excelDataSource = dataSourceToCopy as DashboardExcelDataSource;
            if(excelDataSource != null) {
                XElement element = excelDataSource.SaveToXml();
                DashboardExcelDataSource newDataSource = new DashboardExcelDataSource();
                newDataSource.LoadFromXml(element);
                //newDataSource.Fill();
                return newDataSource;
            }

            DashboardExtractDataSource extractDataSource = dataSourceToCopy as DashboardExtractDataSource;
            if(extractDataSource != null) {
                XElement element = extractDataSource.SaveToXml();
                DashboardExtractDataSource newDataSource = new DashboardExtractDataSource();
                //newDataSource.LoadFromXml(element);
                return newDataSource;
            }

            DashboardObjectDataSource objectDataSource = dataSourceToCopy as DashboardObjectDataSource;
            if(objectDataSource != null) {
                XElement element = objectDataSource.SaveToXml();
                DashboardObjectDataSource newDataSource = new DashboardObjectDataSource();
                newDataSource.LoadFromXml(element);
                //newDataSource.Fill();
                return newDataSource;
            }

            DashboardOlapDataSource olapDataSource = dataSourceToCopy as DashboardOlapDataSource;
            if(olapDataSource != null) {
                XElement element = olapDataSource.SaveToXml();
                DashboardOlapDataSource newDataSource = new DashboardOlapDataSource();
                newDataSource.LoadFromXml(element);
                //newDataSource.Fill();
                return newDataSource;
            }

            DashboardSqlDataSource sqlDataSource = dataSourceToCopy as DashboardSqlDataSource;
            if(sqlDataSource != null) {
                XElement element = sqlDataSource.SaveToXml();
                DashboardSqlDataSource newDataSource = new DashboardSqlDataSource();
                newDataSource.LoadFromXml(element);
                //newDataSource.Fill();
                return newDataSource;
            }
            return null;
        }
    }
}
