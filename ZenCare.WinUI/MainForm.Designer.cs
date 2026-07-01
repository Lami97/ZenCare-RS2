namespace ZenCare.WinUI;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblDashboardTitle = new Label();
        this.components = new System.ComponentModel.Container();
        SuspendLayout();
        // 
        // lblDashboardTitle
        // 
        lblDashboardTitle.AutoSize = true;
        lblDashboardTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblDashboardTitle.Location = new Point(32, 32);
        lblDashboardTitle.Name = "lblDashboardTitle";
        lblDashboardTitle.Size = new Size(297, 32);
        lblDashboardTitle.TabIndex = 0;
        lblDashboardTitle.Text = "ZenCare Admin Dashboard";
        // 
        // MainForm
        // 
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1000, 700);
        Controls.Add(lblDashboardTitle);
        this.Name = "MainForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "ZenCare Admin";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblDashboardTitle;
}
