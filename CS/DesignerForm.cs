using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DashboardMergeExample
{
    public partial class DesignerForm : XtraForm {
        bool dashboardChanged = false;
        public DesignerForm() {
            InitializeComponent();
            dashboardDesigner.CreateRibbon();
            dashboardDesigner.CustomizeDashboardTitle += DashboardDesignerCustomizeDashboardTitle;
            dashboardDesigner.UpdateDashboardTitle();
            dashboardDesigner.DashboardClosing += DashboardDesignerDashboardClosing;
        }

        // The method that performs the merge.
        void MergeDashboard(DashboardToolbarItemClickEventArgs args) {
            // Invoke a file dialog to select multiple dashboards to merge.
            // If one or several selected dashboards have tabbed layout and cannot be merged, a message is shown. 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dashboard files (*.xml)|*.xml";
            openFileDialog.InitialDirectory = Application.StartupPath + "\\Dashboards";
            openFileDialog.Multiselect = true;
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                dashboardDesigner.Dashboard.BeginUpdate();
                try {
                    List<string> rejectedDashboard = new List<string>();
                    foreach(string fileName in openFileDialog.FileNames) {
                        using(Dashboard dashboard = new Dashboard()) {
                            dashboard.LoadFromXml(fileName);
                            // The DashboardMerger instance is the key object that performs the merge.
                            DashboardMerger dashboardMerger = new DashboardMerger(dashboardDesigner.Dashboard);
                            if(!dashboardMerger.MergeDashboard(dashboard)) {
                                rejectedDashboard.Add(Path.GetFileName(fileName));
                            }
                        }
                    }
                    if(rejectedDashboard.Count > 0)
                        MessageBox.Show(String.Format("Cannot merge the following dashboard(s): {0}{1}", Environment.NewLine, String.Join(Environment.NewLine, rejectedDashboard)));
                    if((openFileDialog.FileNames.Length - rejectedDashboard.Count) > 0)
                        dashboardChanged = true;
                } finally {
                    dashboardDesigner.Dashboard.EndUpdate();
                    dashboardDesigner.ReloadData();
                }
            }
        }

        void DashboardDesignerDashboardClosing(object sender, DashboardClosingEventArgs e) {
            if(dashboardChanged)
                e.IsDashboardModified = true;
        }

        // The DashboardDesigner.CustomizeDashboardTitle event handler.
        // Create a custom button in the dashboard title with a click action that calls the MergeDashboard method.
        void DashboardDesignerCustomizeDashboardTitle(object sender, CustomizeDashboardTitleEventArgs e) {
            DashboardToolbarItem mergeItem = new DashboardToolbarItem("Open the dashboard(s) to merge", MergeDashboard);
            mergeItem.SvgImage = (SvgImage)Properties.Resources.MergeIcon;
            mergeItem.Caption = "Merge Dashboard";
            e.Items.Insert(0, mergeItem);
        }
    }
}
