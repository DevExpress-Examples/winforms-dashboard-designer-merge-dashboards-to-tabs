using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;

namespace DashboardMerger {
    public partial class DesignerForm : XtraForm {
        bool dashboardChanged = false;
        public DesignerForm() {
            InitializeComponent();
            dashboardDesigner.CreateRibbon();
            dashboardDesigner.UpdateDashboardTitle();
        }

        void MergeDashboard(DashboardToolbarItemClickEventArgs args) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dashboard files (*.xml)|*.xml";
            openFileDialog.Multiselect = true;
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                dashboardDesigner.Dashboard.BeginUpdate();
                try {
                    List<string> rejectedDashboard = new List<string>();
                    foreach(string fileName in openFileDialog.FileNames) {
                        using(Dashboard dashboard = new Dashboard()) {
                            dashboard.LoadFromXml(fileName);
                            DashboardMerger dashboardMerger = new DashboardMerger(dashboardDesigner.Dashboard);
                            if(!dashboardMerger.MergeDashboard(dashboard)) {
                                rejectedDashboard.Add(Path.GetFileName(fileName));
                            }
                        }
                    }
                    if(rejectedDashboard.Count > 0)
                        MessageBox.Show(String.Format("The following dashboard has not been merged{0}{1}", Environment.NewLine, String.Join(Environment.NewLine, rejectedDashboard)));
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

        void DashboardDesignerCustomizeDashboardTitle(object sender, CustomizeDashboardTitleEventArgs e) {
            DashboardToolbarItem mergeItem = new DashboardToolbarItem("Open Dashboard to merge", MergeDashboard);
            mergeItem.SvgImage = (SvgImage)Properties.Resources.MergeIcon;
            mergeItem.Caption = "Merge Dashboard";
            e.Items.Insert(0, mergeItem);
        }
    }
}
