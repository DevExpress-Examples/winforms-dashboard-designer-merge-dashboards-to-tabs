namespace DashboardMergeExample {
    partial class DesignerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.dashboardDesigner = new DevExpress.DashboardWin.DashboardDesigner();
            this.SuspendLayout();
            // 
            // dashboardDesigner
            // 
            this.dashboardDesigner.AllowMaximizeAnimation = true;
            this.dashboardDesigner.AllowMaximizeDashboardItems = true;
            this.dashboardDesigner.AllowPrintDashboard = true;
            this.dashboardDesigner.AllowPrintDashboardItems = true;
            this.dashboardDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardDesigner.Location = new System.Drawing.Point(0, 0);
            this.dashboardDesigner.Name = "dashboardDesigner";
            this.dashboardDesigner.Size = new System.Drawing.Size(962, 584);
            this.dashboardDesigner.TabIndex = 0;
            // 
            // DesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 584);
            this.Controls.Add(this.dashboardDesigner);
            this.Name = "DesignerForm";
            this.Text = "Dashboard Merger";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.DashboardWin.DashboardDesigner dashboardDesigner;
    }
}

