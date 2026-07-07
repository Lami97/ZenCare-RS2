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
        pnlNavigation = new Panel();
        btnLogout = new Button();
        btnReports = new Button();
        btnSuppliers = new Button();
        btnPurchaseItems = new Button();
        btnPurchases = new Button();
        btnReviews = new Button();
        btnAppointments = new Button();
        btnEmployees = new Button();
        btnUserRoles = new Button();
        btnUsers = new Button();
        btnFAQs = new Button();
        btnFAQCategories = new Button();
        btnServiceCategories = new Button();
        btnServices = new Button();
        btnUnits = new Button();
        btnProductTypes = new Button();
        btnProductsManagement = new Button();
        btnProducts = new Button();
        lblNavigationTitle = new Label();
        pnlContent = new Panel();
        lblDashboardTitle = new Label();
        this.components = new System.ComponentModel.Container();
        pnlNavigation.SuspendLayout();
        pnlContent.SuspendLayout();
        SuspendLayout();
        // 
        // pnlNavigation
        // 
        pnlNavigation.BackColor = Color.FromArgb(32, 45, 57);
        pnlNavigation.Controls.Add(btnLogout);
        pnlNavigation.Controls.Add(btnReports);
        pnlNavigation.Controls.Add(btnSuppliers);
        pnlNavigation.Controls.Add(btnPurchaseItems);
        pnlNavigation.Controls.Add(btnPurchases);
        pnlNavigation.Controls.Add(btnReviews);
        pnlNavigation.Controls.Add(btnAppointments);
        pnlNavigation.Controls.Add(btnEmployees);
        pnlNavigation.Controls.Add(btnUserRoles);
        pnlNavigation.Controls.Add(btnUsers);
        pnlNavigation.Controls.Add(btnFAQs);
        pnlNavigation.Controls.Add(btnFAQCategories);
        pnlNavigation.Controls.Add(btnServiceCategories);
        pnlNavigation.Controls.Add(btnServices);
        pnlNavigation.Controls.Add(btnUnits);
        pnlNavigation.Controls.Add(btnProductTypes);
        pnlNavigation.Controls.Add(btnProductsManagement);
        pnlNavigation.Controls.Add(btnProducts);
        pnlNavigation.Controls.Add(lblNavigationTitle);
        pnlNavigation.Dock = DockStyle.Left;
        pnlNavigation.Location = new Point(0, 0);
        pnlNavigation.Name = "pnlNavigation";
        pnlNavigation.Size = new Size(200, 800);
        pnlNavigation.TabIndex = 0;
        // 
        // btnLogout
        // 
        btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(20, 736);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(160, 40);
        btnLogout.TabIndex = 6;
        btnLogout.Text = "Logout";
        btnLogout.UseVisualStyleBackColor = true;
        btnLogout.Click += btnLogout_Click;
        // 
        // btnReports
        // 
        btnReports.FlatStyle = FlatStyle.Flat;
        btnReports.ForeColor = Color.White;
        btnReports.Location = new Point(20, 700);
        btnReports.Name = "btnReports";
        btnReports.Size = new Size(160, 34);
        btnReports.TabIndex = 5;
        btnReports.Text = "Reports";
        btnReports.UseVisualStyleBackColor = true;
        btnReports.Click += btnReports_Click;
        // 
        // btnSuppliers
        // 
        btnSuppliers.FlatStyle = FlatStyle.Flat;
        btnSuppliers.ForeColor = Color.White;
        btnSuppliers.Location = new Point(20, 664);
        btnSuppliers.Name = "btnSuppliers";
        btnSuppliers.Size = new Size(160, 34);
        btnSuppliers.TabIndex = 18;
        btnSuppliers.Text = "Suppliers";
        btnSuppliers.UseVisualStyleBackColor = true;
        btnSuppliers.Click += btnSuppliers_Click;
        // 
        // btnPurchaseItems
        // 
        btnPurchaseItems.FlatStyle = FlatStyle.Flat;
        btnPurchaseItems.ForeColor = Color.White;
        btnPurchaseItems.Location = new Point(20, 628);
        btnPurchaseItems.Name = "btnPurchaseItems";
        btnPurchaseItems.Size = new Size(160, 34);
        btnPurchaseItems.TabIndex = 17;
        btnPurchaseItems.Text = "Purchase Items";
        btnPurchaseItems.UseVisualStyleBackColor = true;
        btnPurchaseItems.Click += btnPurchaseItems_Click;
        // 
        // btnPurchases
        // 
        btnPurchases.FlatStyle = FlatStyle.Flat;
        btnPurchases.ForeColor = Color.White;
        btnPurchases.Location = new Point(20, 592);
        btnPurchases.Name = "btnPurchases";
        btnPurchases.Size = new Size(160, 34);
        btnPurchases.TabIndex = 16;
        btnPurchases.Text = "Purchases";
        btnPurchases.UseVisualStyleBackColor = true;
        btnPurchases.Click += btnPurchases_Click;
        // 
        // btnReviews
        // 
        btnReviews.FlatStyle = FlatStyle.Flat;
        btnReviews.ForeColor = Color.White;
        btnReviews.Location = new Point(20, 556);
        btnReviews.Name = "btnReviews";
        btnReviews.Size = new Size(160, 34);
        btnReviews.TabIndex = 15;
        btnReviews.Text = "Reviews";
        btnReviews.UseVisualStyleBackColor = true;
        btnReviews.Click += btnReviews_Click;
        // 
        // btnAppointments
        // 
        btnAppointments.FlatStyle = FlatStyle.Flat;
        btnAppointments.ForeColor = Color.White;
        btnAppointments.Location = new Point(20, 516);
        btnAppointments.Name = "btnAppointments";
        btnAppointments.Size = new Size(160, 34);
        btnAppointments.TabIndex = 4;
        btnAppointments.Text = "Appointments";
        btnAppointments.UseVisualStyleBackColor = true;
        btnAppointments.Click += btnAppointments_Click;
        // 
        // btnEmployees
        // 
        btnEmployees.FlatStyle = FlatStyle.Flat;
        btnEmployees.ForeColor = Color.White;
        btnEmployees.Location = new Point(20, 476);
        btnEmployees.Name = "btnEmployees";
        btnEmployees.Size = new Size(160, 34);
        btnEmployees.TabIndex = 3;
        btnEmployees.Text = "Employees";
        btnEmployees.UseVisualStyleBackColor = true;
        btnEmployees.Click += btnEmployees_Click;
        // 
        // btnUserRoles
        // 
        btnUserRoles.FlatStyle = FlatStyle.Flat;
        btnUserRoles.ForeColor = Color.White;
        btnUserRoles.Location = new Point(20, 436);
        btnUserRoles.Name = "btnUserRoles";
        btnUserRoles.Size = new Size(160, 34);
        btnUserRoles.TabIndex = 12;
        btnUserRoles.Text = "User Roles";
        btnUserRoles.UseVisualStyleBackColor = true;
        btnUserRoles.Click += btnUserRoles_Click;
        // 
        // btnUsers
        // 
        btnUsers.FlatStyle = FlatStyle.Flat;
        btnUsers.ForeColor = Color.White;
        btnUsers.Location = new Point(20, 396);
        btnUsers.Name = "btnUsers";
        btnUsers.Size = new Size(160, 34);
        btnUsers.TabIndex = 11;
        btnUsers.Text = "Users";
        btnUsers.UseVisualStyleBackColor = true;
        btnUsers.Click += btnUsers_Click;
        // 
        // btnFAQs
        // 
        btnFAQs.FlatStyle = FlatStyle.Flat;
        btnFAQs.ForeColor = Color.White;
        btnFAQs.Location = new Point(20, 356);
        btnFAQs.Name = "btnFAQs";
        btnFAQs.Size = new Size(160, 34);
        btnFAQs.TabIndex = 14;
        btnFAQs.Text = "FAQ";
        btnFAQs.UseVisualStyleBackColor = true;
        btnFAQs.Click += btnFAQs_Click;
        // 
        // btnFAQCategories
        // 
        btnFAQCategories.FlatStyle = FlatStyle.Flat;
        btnFAQCategories.ForeColor = Color.White;
        btnFAQCategories.Location = new Point(20, 316);
        btnFAQCategories.Name = "btnFAQCategories";
        btnFAQCategories.Size = new Size(160, 34);
        btnFAQCategories.TabIndex = 13;
        btnFAQCategories.Text = "FAQ Categories";
        btnFAQCategories.UseVisualStyleBackColor = true;
        btnFAQCategories.Click += btnFAQCategories_Click;
        // 
        // btnServiceCategories
        // 
        btnServiceCategories.FlatStyle = FlatStyle.Flat;
        btnServiceCategories.ForeColor = Color.White;
        btnServiceCategories.Location = new Point(20, 276);
        btnServiceCategories.Name = "btnServiceCategories";
        btnServiceCategories.Size = new Size(160, 34);
        btnServiceCategories.TabIndex = 10;
        btnServiceCategories.Text = "Service Categories";
        btnServiceCategories.UseVisualStyleBackColor = true;
        btnServiceCategories.Click += btnServiceCategories_Click;
        // 
        // btnServices
        // 
        btnServices.FlatStyle = FlatStyle.Flat;
        btnServices.ForeColor = Color.White;
        btnServices.Location = new Point(20, 236);
        btnServices.Name = "btnServices";
        btnServices.Size = new Size(160, 34);
        btnServices.TabIndex = 2;
        btnServices.Text = "Services Management";
        btnServices.UseVisualStyleBackColor = true;
        btnServices.Click += btnServices_Click;
        // 
        // btnUnits
        // 
        btnUnits.FlatStyle = FlatStyle.Flat;
        btnUnits.ForeColor = Color.White;
        btnUnits.Location = new Point(20, 196);
        btnUnits.Name = "btnUnits";
        btnUnits.Size = new Size(160, 34);
        btnUnits.TabIndex = 8;
        btnUnits.Text = "Units";
        btnUnits.UseVisualStyleBackColor = true;
        btnUnits.Click += btnUnits_Click;
        // 
        // btnProductTypes
        // 
        btnProductTypes.FlatStyle = FlatStyle.Flat;
        btnProductTypes.ForeColor = Color.White;
        btnProductTypes.Location = new Point(20, 156);
        btnProductTypes.Name = "btnProductTypes";
        btnProductTypes.Size = new Size(160, 34);
        btnProductTypes.TabIndex = 7;
        btnProductTypes.Text = "Product Types";
        btnProductTypes.UseVisualStyleBackColor = true;
        btnProductTypes.Click += btnProductTypes_Click;
        // 
        // btnProductsManagement
        // 
        btnProductsManagement.FlatStyle = FlatStyle.Flat;
        btnProductsManagement.ForeColor = Color.White;
        btnProductsManagement.Location = new Point(20, 116);
        btnProductsManagement.Name = "btnProductsManagement";
        btnProductsManagement.Size = new Size(160, 34);
        btnProductsManagement.TabIndex = 9;
        btnProductsManagement.Text = "Products Management";
        btnProductsManagement.UseVisualStyleBackColor = true;
        btnProductsManagement.Click += btnProductsManagement_Click;
        // 
        // btnProducts
        // 
        btnProducts.FlatStyle = FlatStyle.Flat;
        btnProducts.ForeColor = Color.White;
        btnProducts.Location = new Point(20, 76);
        btnProducts.Name = "btnProducts";
        btnProducts.Size = new Size(160, 34);
        btnProducts.TabIndex = 1;
        btnProducts.Text = "Products";
        btnProducts.UseVisualStyleBackColor = true;
        btnProducts.Click += btnProducts_Click;
        // 
        // lblNavigationTitle
        // 
        lblNavigationTitle.AutoSize = true;
        lblNavigationTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblNavigationTitle.ForeColor = Color.White;
        lblNavigationTitle.Location = new Point(20, 24);
        lblNavigationTitle.Name = "lblNavigationTitle";
        lblNavigationTitle.Size = new Size(126, 21);
        lblNavigationTitle.TabIndex = 0;
        lblNavigationTitle.Text = "ZenCare Admin";
        // 
        // pnlContent
        // 
        pnlContent.BackColor = Color.White;
        pnlContent.Controls.Add(lblDashboardTitle);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(200, 0);
        pnlContent.Name = "pnlContent";
        pnlContent.Size = new Size(1000, 800);
        pnlContent.TabIndex = 1;
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
        this.ClientSize = new System.Drawing.Size(1200, 800);
        Controls.Add(pnlContent);
        Controls.Add(pnlNavigation);
        this.Name = "MainForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "ZenCare Admin";
        pnlNavigation.ResumeLayout(false);
        pnlNavigation.PerformLayout();
        pnlContent.ResumeLayout(false);
        pnlContent.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlNavigation;
    private Button btnLogout;
    private Button btnReports;
    private Button btnSuppliers;
    private Button btnPurchaseItems;
    private Button btnPurchases;
    private Button btnReviews;
    private Button btnAppointments;
    private Button btnEmployees;
    private Button btnUserRoles;
    private Button btnUsers;
    private Button btnFAQs;
    private Button btnFAQCategories;
    private Button btnServiceCategories;
    private Button btnServices;
    private Button btnUnits;
    private Button btnProductTypes;
    private Button btnProductsManagement;
    private Button btnProducts;
    private Label lblNavigationTitle;
    private Panel pnlContent;
    private Label lblDashboardTitle;
}
