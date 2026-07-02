namespace ZenCare.WinUI.Forms;

partial class ReportsForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        pnlTop = new Panel();
        btnRefresh = new Button();
        lblTitle = new Label();
        lblStatus = new Label();
        pnlGeneral = new Panel();
        lblTotalProductsValue = new Label();
        lblTotalProducts = new Label();
        lblTotalServicesValue = new Label();
        lblTotalServices = new Label();
        lblTotalAppointmentsValue = new Label();
        lblTotalAppointments = new Label();
        lblTotalEmployeesValue = new Label();
        lblTotalEmployees = new Label();
        lblTotalUsersValue = new Label();
        lblTotalUsers = new Label();
        lblGeneralTitle = new Label();
        pnlAppointments = new Panel();
        lblCancelledAppointmentsValue = new Label();
        lblCancelledAppointments = new Label();
        lblCompletedAppointmentsValue = new Label();
        lblCompletedAppointments = new Label();
        lblConfirmedAppointmentsValue = new Label();
        lblConfirmedAppointments = new Label();
        lblPendingAppointmentsValue = new Label();
        lblPendingAppointments = new Label();
        lblAppointmentTitle = new Label();
        pnlPopularServices = new Panel();
        lblPopularServicesEmpty = new Label();
        dgvPopularServices = new DataGridView();
        colServiceName = new DataGridViewTextBoxColumn();
        colServiceAppointments = new DataGridViewTextBoxColumn();
        lblPopularServicesTitle = new Label();
        pnlEmployeeWorkload = new Panel();
        lblEmployeeWorkloadEmpty = new Label();
        dgvEmployeeWorkload = new DataGridView();
        colEmployee = new DataGridViewTextBoxColumn();
        colEmployeeAppointments = new DataGridViewTextBoxColumn();
        lblEmployeeWorkloadTitle = new Label();
        pnlUserActivity = new Panel();
        lblUserActivityEmpty = new Label();
        dgvUserActivity = new DataGridView();
        colUser = new DataGridViewTextBoxColumn();
        colUserAppointments = new DataGridViewTextBoxColumn();
        lblUserActivityTitle = new Label();
        pnlTop.SuspendLayout();
        pnlGeneral.SuspendLayout();
        pnlAppointments.SuspendLayout();
        pnlPopularServices.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPopularServices).BeginInit();
        pnlEmployeeWorkload.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEmployeeWorkload).BeginInit();
        pnlUserActivity.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUserActivity).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(lblTitle);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(1000, 72);
        pnlTop.TabIndex = 0;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(864, 22);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 1;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.Location = new Point(24, 20);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(99, 32);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Reports";
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.ForeColor = Color.DimGray;
        lblStatus.Location = new Point(24, 760);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 6;
        lblStatus.Text = "Ready";
        // 
        // pnlGeneral
        // 
        pnlGeneral.BorderStyle = BorderStyle.FixedSingle;
        pnlGeneral.Controls.Add(lblTotalProductsValue);
        pnlGeneral.Controls.Add(lblTotalProducts);
        pnlGeneral.Controls.Add(lblTotalServicesValue);
        pnlGeneral.Controls.Add(lblTotalServices);
        pnlGeneral.Controls.Add(lblTotalAppointmentsValue);
        pnlGeneral.Controls.Add(lblTotalAppointments);
        pnlGeneral.Controls.Add(lblTotalEmployeesValue);
        pnlGeneral.Controls.Add(lblTotalEmployees);
        pnlGeneral.Controls.Add(lblTotalUsersValue);
        pnlGeneral.Controls.Add(lblTotalUsers);
        pnlGeneral.Controls.Add(lblGeneralTitle);
        pnlGeneral.Location = new Point(24, 88);
        pnlGeneral.Name = "pnlGeneral";
        pnlGeneral.Size = new Size(456, 156);
        pnlGeneral.TabIndex = 1;
        // 
        // general labels
        // 
        lblGeneralTitle.AutoSize = true;
        lblGeneralTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblGeneralTitle.Location = new Point(16, 14);
        lblGeneralTitle.Name = "lblGeneralTitle";
        lblGeneralTitle.Size = new Size(143, 19);
        lblGeneralTitle.TabIndex = 0;
        lblGeneralTitle.Text = "General statistics";
        lblTotalUsers.Location = new Point(16, 48);
        lblTotalUsers.Name = "lblTotalUsers";
        lblTotalUsers.Size = new Size(140, 20);
        lblTotalUsers.TabIndex = 1;
        lblTotalUsers.Text = "Total users";
        lblTotalUsersValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotalUsersValue.Location = new Point(164, 48);
        lblTotalUsersValue.Name = "lblTotalUsersValue";
        lblTotalUsersValue.Size = new Size(52, 20);
        lblTotalUsersValue.TabIndex = 2;
        lblTotalUsersValue.Text = "0";
        lblTotalEmployees.Location = new Point(236, 48);
        lblTotalEmployees.Name = "lblTotalEmployees";
        lblTotalEmployees.Size = new Size(140, 20);
        lblTotalEmployees.TabIndex = 3;
        lblTotalEmployees.Text = "Total employees";
        lblTotalEmployeesValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotalEmployeesValue.Location = new Point(384, 48);
        lblTotalEmployeesValue.Name = "lblTotalEmployeesValue";
        lblTotalEmployeesValue.Size = new Size(52, 20);
        lblTotalEmployeesValue.TabIndex = 4;
        lblTotalEmployeesValue.Text = "0";
        lblTotalAppointments.Location = new Point(16, 82);
        lblTotalAppointments.Name = "lblTotalAppointments";
        lblTotalAppointments.Size = new Size(140, 20);
        lblTotalAppointments.TabIndex = 5;
        lblTotalAppointments.Text = "Total appointments";
        lblTotalAppointmentsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotalAppointmentsValue.Location = new Point(164, 82);
        lblTotalAppointmentsValue.Name = "lblTotalAppointmentsValue";
        lblTotalAppointmentsValue.Size = new Size(52, 20);
        lblTotalAppointmentsValue.TabIndex = 6;
        lblTotalAppointmentsValue.Text = "0";
        lblTotalServices.Location = new Point(236, 82);
        lblTotalServices.Name = "lblTotalServices";
        lblTotalServices.Size = new Size(140, 20);
        lblTotalServices.TabIndex = 7;
        lblTotalServices.Text = "Total services";
        lblTotalServicesValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotalServicesValue.Location = new Point(384, 82);
        lblTotalServicesValue.Name = "lblTotalServicesValue";
        lblTotalServicesValue.Size = new Size(52, 20);
        lblTotalServicesValue.TabIndex = 8;
        lblTotalServicesValue.Text = "0";
        lblTotalProducts.Location = new Point(16, 116);
        lblTotalProducts.Name = "lblTotalProducts";
        lblTotalProducts.Size = new Size(140, 20);
        lblTotalProducts.TabIndex = 9;
        lblTotalProducts.Text = "Total products";
        lblTotalProductsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotalProductsValue.Location = new Point(164, 116);
        lblTotalProductsValue.Name = "lblTotalProductsValue";
        lblTotalProductsValue.Size = new Size(52, 20);
        lblTotalProductsValue.TabIndex = 10;
        lblTotalProductsValue.Text = "0";
        // 
        // pnlAppointments
        // 
        pnlAppointments.BorderStyle = BorderStyle.FixedSingle;
        pnlAppointments.Controls.Add(lblCancelledAppointmentsValue);
        pnlAppointments.Controls.Add(lblCancelledAppointments);
        pnlAppointments.Controls.Add(lblCompletedAppointmentsValue);
        pnlAppointments.Controls.Add(lblCompletedAppointments);
        pnlAppointments.Controls.Add(lblConfirmedAppointmentsValue);
        pnlAppointments.Controls.Add(lblConfirmedAppointments);
        pnlAppointments.Controls.Add(lblPendingAppointmentsValue);
        pnlAppointments.Controls.Add(lblPendingAppointments);
        pnlAppointments.Controls.Add(lblAppointmentTitle);
        pnlAppointments.Location = new Point(504, 88);
        pnlAppointments.Name = "pnlAppointments";
        pnlAppointments.Size = new Size(456, 156);
        pnlAppointments.TabIndex = 2;
        lblAppointmentTitle.AutoSize = true;
        lblAppointmentTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblAppointmentTitle.Location = new Point(16, 14);
        lblAppointmentTitle.Name = "lblAppointmentTitle";
        lblAppointmentTitle.Size = new Size(165, 19);
        lblAppointmentTitle.TabIndex = 0;
        lblAppointmentTitle.Text = "Appointment statistics";
        lblPendingAppointments.Location = new Point(16, 48);
        lblPendingAppointments.Name = "lblPendingAppointments";
        lblPendingAppointments.Size = new Size(140, 20);
        lblPendingAppointments.TabIndex = 1;
        lblPendingAppointments.Text = "Pending";
        lblPendingAppointmentsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPendingAppointmentsValue.Location = new Point(164, 48);
        lblPendingAppointmentsValue.Name = "lblPendingAppointmentsValue";
        lblPendingAppointmentsValue.Size = new Size(52, 20);
        lblPendingAppointmentsValue.TabIndex = 2;
        lblPendingAppointmentsValue.Text = "0";
        lblConfirmedAppointments.Location = new Point(236, 48);
        lblConfirmedAppointments.Name = "lblConfirmedAppointments";
        lblConfirmedAppointments.Size = new Size(140, 20);
        lblConfirmedAppointments.TabIndex = 3;
        lblConfirmedAppointments.Text = "Confirmed";
        lblConfirmedAppointmentsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblConfirmedAppointmentsValue.Location = new Point(384, 48);
        lblConfirmedAppointmentsValue.Name = "lblConfirmedAppointmentsValue";
        lblConfirmedAppointmentsValue.Size = new Size(52, 20);
        lblConfirmedAppointmentsValue.TabIndex = 4;
        lblConfirmedAppointmentsValue.Text = "0";
        lblCompletedAppointments.Location = new Point(16, 82);
        lblCompletedAppointments.Name = "lblCompletedAppointments";
        lblCompletedAppointments.Size = new Size(140, 20);
        lblCompletedAppointments.TabIndex = 5;
        lblCompletedAppointments.Text = "Completed";
        lblCompletedAppointmentsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCompletedAppointmentsValue.Location = new Point(164, 82);
        lblCompletedAppointmentsValue.Name = "lblCompletedAppointmentsValue";
        lblCompletedAppointmentsValue.Size = new Size(52, 20);
        lblCompletedAppointmentsValue.TabIndex = 6;
        lblCompletedAppointmentsValue.Text = "0";
        lblCancelledAppointments.Location = new Point(236, 82);
        lblCancelledAppointments.Name = "lblCancelledAppointments";
        lblCancelledAppointments.Size = new Size(140, 20);
        lblCancelledAppointments.TabIndex = 7;
        lblCancelledAppointments.Text = "Cancelled";
        lblCancelledAppointmentsValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCancelledAppointmentsValue.Location = new Point(384, 82);
        lblCancelledAppointmentsValue.Name = "lblCancelledAppointmentsValue";
        lblCancelledAppointmentsValue.Size = new Size(52, 20);
        lblCancelledAppointmentsValue.TabIndex = 8;
        lblCancelledAppointmentsValue.Text = "0";
        // 
        // pnlPopularServices
        // 
        pnlPopularServices.BorderStyle = BorderStyle.FixedSingle;
        pnlPopularServices.Controls.Add(lblPopularServicesEmpty);
        pnlPopularServices.Controls.Add(dgvPopularServices);
        pnlPopularServices.Controls.Add(lblPopularServicesTitle);
        pnlPopularServices.Location = new Point(24, 272);
        pnlPopularServices.Name = "pnlPopularServices";
        pnlPopularServices.Size = new Size(296, 456);
        pnlPopularServices.TabIndex = 3;
        lblPopularServicesTitle.AutoSize = true;
        lblPopularServicesTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblPopularServicesTitle.Location = new Point(16, 14);
        lblPopularServicesTitle.Name = "lblPopularServicesTitle";
        lblPopularServicesTitle.Size = new Size(157, 19);
        lblPopularServicesTitle.TabIndex = 0;
        lblPopularServicesTitle.Text = "Most popular services";
        dgvPopularServices.AllowUserToAddRows = false;
        dgvPopularServices.AllowUserToDeleteRows = false;
        dgvPopularServices.AutoGenerateColumns = false;
        dgvPopularServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPopularServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvPopularServices.Columns.AddRange(new DataGridViewColumn[] { colServiceName, colServiceAppointments });
        dgvPopularServices.Location = new Point(16, 48);
        dgvPopularServices.Name = "dgvPopularServices";
        dgvPopularServices.ReadOnly = true;
        dgvPopularServices.RowHeadersVisible = false;
        dgvPopularServices.Size = new Size(264, 364);
        dgvPopularServices.TabIndex = 1;
        colServiceName.DataPropertyName = "ServiceName";
        colServiceName.HeaderText = "Service";
        colServiceName.Name = "colServiceName";
        colServiceName.ReadOnly = true;
        colServiceAppointments.DataPropertyName = "Appointments";
        colServiceAppointments.HeaderText = "Appointments";
        colServiceAppointments.Name = "colServiceAppointments";
        colServiceAppointments.ReadOnly = true;
        lblPopularServicesEmpty.AutoSize = true;
        lblPopularServicesEmpty.ForeColor = Color.DimGray;
        lblPopularServicesEmpty.Location = new Point(16, 424);
        lblPopularServicesEmpty.Name = "lblPopularServicesEmpty";
        lblPopularServicesEmpty.Size = new Size(103, 15);
        lblPopularServicesEmpty.TabIndex = 2;
        lblPopularServicesEmpty.Text = "No data available.";
        lblPopularServicesEmpty.Visible = false;
        // 
        // pnlEmployeeWorkload
        // 
        pnlEmployeeWorkload.BorderStyle = BorderStyle.FixedSingle;
        pnlEmployeeWorkload.Controls.Add(lblEmployeeWorkloadEmpty);
        pnlEmployeeWorkload.Controls.Add(dgvEmployeeWorkload);
        pnlEmployeeWorkload.Controls.Add(lblEmployeeWorkloadTitle);
        pnlEmployeeWorkload.Location = new Point(352, 272);
        pnlEmployeeWorkload.Name = "pnlEmployeeWorkload";
        pnlEmployeeWorkload.Size = new Size(296, 456);
        pnlEmployeeWorkload.TabIndex = 4;
        lblEmployeeWorkloadTitle.AutoSize = true;
        lblEmployeeWorkloadTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblEmployeeWorkloadTitle.Location = new Point(16, 14);
        lblEmployeeWorkloadTitle.Name = "lblEmployeeWorkloadTitle";
        lblEmployeeWorkloadTitle.Size = new Size(143, 19);
        lblEmployeeWorkloadTitle.TabIndex = 0;
        lblEmployeeWorkloadTitle.Text = "Employee workload";
        dgvEmployeeWorkload.AllowUserToAddRows = false;
        dgvEmployeeWorkload.AllowUserToDeleteRows = false;
        dgvEmployeeWorkload.AutoGenerateColumns = false;
        dgvEmployeeWorkload.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvEmployeeWorkload.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvEmployeeWorkload.Columns.AddRange(new DataGridViewColumn[] { colEmployee, colEmployeeAppointments });
        dgvEmployeeWorkload.Location = new Point(16, 48);
        dgvEmployeeWorkload.Name = "dgvEmployeeWorkload";
        dgvEmployeeWorkload.ReadOnly = true;
        dgvEmployeeWorkload.RowHeadersVisible = false;
        dgvEmployeeWorkload.Size = new Size(264, 364);
        dgvEmployeeWorkload.TabIndex = 1;
        colEmployee.DataPropertyName = "Employee";
        colEmployee.HeaderText = "Employee";
        colEmployee.Name = "colEmployee";
        colEmployee.ReadOnly = true;
        colEmployeeAppointments.DataPropertyName = "Appointments";
        colEmployeeAppointments.HeaderText = "Appointments";
        colEmployeeAppointments.Name = "colEmployeeAppointments";
        colEmployeeAppointments.ReadOnly = true;
        lblEmployeeWorkloadEmpty.AutoSize = true;
        lblEmployeeWorkloadEmpty.ForeColor = Color.DimGray;
        lblEmployeeWorkloadEmpty.Location = new Point(16, 424);
        lblEmployeeWorkloadEmpty.Name = "lblEmployeeWorkloadEmpty";
        lblEmployeeWorkloadEmpty.Size = new Size(103, 15);
        lblEmployeeWorkloadEmpty.TabIndex = 2;
        lblEmployeeWorkloadEmpty.Text = "No data available.";
        lblEmployeeWorkloadEmpty.Visible = false;
        // 
        // pnlUserActivity
        // 
        pnlUserActivity.BorderStyle = BorderStyle.FixedSingle;
        pnlUserActivity.Controls.Add(lblUserActivityEmpty);
        pnlUserActivity.Controls.Add(dgvUserActivity);
        pnlUserActivity.Controls.Add(lblUserActivityTitle);
        pnlUserActivity.Location = new Point(680, 272);
        pnlUserActivity.Name = "pnlUserActivity";
        pnlUserActivity.Size = new Size(280, 456);
        pnlUserActivity.TabIndex = 5;
        lblUserActivityTitle.AutoSize = true;
        lblUserActivityTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblUserActivityTitle.Location = new Point(16, 14);
        lblUserActivityTitle.Name = "lblUserActivityTitle";
        lblUserActivityTitle.Size = new Size(90, 19);
        lblUserActivityTitle.TabIndex = 0;
        lblUserActivityTitle.Text = "User activity";
        dgvUserActivity.AllowUserToAddRows = false;
        dgvUserActivity.AllowUserToDeleteRows = false;
        dgvUserActivity.AutoGenerateColumns = false;
        dgvUserActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUserActivity.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUserActivity.Columns.AddRange(new DataGridViewColumn[] { colUser, colUserAppointments });
        dgvUserActivity.Location = new Point(16, 48);
        dgvUserActivity.Name = "dgvUserActivity";
        dgvUserActivity.ReadOnly = true;
        dgvUserActivity.RowHeadersVisible = false;
        dgvUserActivity.Size = new Size(248, 364);
        dgvUserActivity.TabIndex = 1;
        colUser.DataPropertyName = "User";
        colUser.HeaderText = "User";
        colUser.Name = "colUser";
        colUser.ReadOnly = true;
        colUserAppointments.DataPropertyName = "Appointments";
        colUserAppointments.HeaderText = "Appointments";
        colUserAppointments.Name = "colUserAppointments";
        colUserAppointments.ReadOnly = true;
        lblUserActivityEmpty.AutoSize = true;
        lblUserActivityEmpty.ForeColor = Color.DimGray;
        lblUserActivityEmpty.Location = new Point(16, 424);
        lblUserActivityEmpty.Name = "lblUserActivityEmpty";
        lblUserActivityEmpty.Size = new Size(103, 15);
        lblUserActivityEmpty.TabIndex = 2;
        lblUserActivityEmpty.Text = "No data available.";
        lblUserActivityEmpty.Visible = false;
        // 
        // ReportsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(1000, 800);
        Controls.Add(lblStatus);
        Controls.Add(pnlUserActivity);
        Controls.Add(pnlEmployeeWorkload);
        Controls.Add(pnlPopularServices);
        Controls.Add(pnlAppointments);
        Controls.Add(pnlGeneral);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "ReportsForm";
        Text = "Reports";
        Load += ReportsForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlGeneral.ResumeLayout(false);
        pnlGeneral.PerformLayout();
        pnlAppointments.ResumeLayout(false);
        pnlAppointments.PerformLayout();
        pnlPopularServices.ResumeLayout(false);
        pnlPopularServices.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPopularServices).EndInit();
        pnlEmployeeWorkload.ResumeLayout(false);
        pnlEmployeeWorkload.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEmployeeWorkload).EndInit();
        pnlUserActivity.ResumeLayout(false);
        pnlUserActivity.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUserActivity).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel pnlTop;
    private Button btnRefresh;
    private Label lblTitle;
    private Label lblStatus;
    private Panel pnlGeneral;
    private Label lblTotalProductsValue;
    private Label lblTotalProducts;
    private Label lblTotalServicesValue;
    private Label lblTotalServices;
    private Label lblTotalAppointmentsValue;
    private Label lblTotalAppointments;
    private Label lblTotalEmployeesValue;
    private Label lblTotalEmployees;
    private Label lblTotalUsersValue;
    private Label lblTotalUsers;
    private Label lblGeneralTitle;
    private Panel pnlAppointments;
    private Label lblCancelledAppointmentsValue;
    private Label lblCancelledAppointments;
    private Label lblCompletedAppointmentsValue;
    private Label lblCompletedAppointments;
    private Label lblConfirmedAppointmentsValue;
    private Label lblConfirmedAppointments;
    private Label lblPendingAppointmentsValue;
    private Label lblPendingAppointments;
    private Label lblAppointmentTitle;
    private Panel pnlPopularServices;
    private Label lblPopularServicesEmpty;
    private DataGridView dgvPopularServices;
    private DataGridViewTextBoxColumn colServiceName;
    private DataGridViewTextBoxColumn colServiceAppointments;
    private Label lblPopularServicesTitle;
    private Panel pnlEmployeeWorkload;
    private Label lblEmployeeWorkloadEmpty;
    private DataGridView dgvEmployeeWorkload;
    private DataGridViewTextBoxColumn colEmployee;
    private DataGridViewTextBoxColumn colEmployeeAppointments;
    private Label lblEmployeeWorkloadTitle;
    private Panel pnlUserActivity;
    private Label lblUserActivityEmpty;
    private DataGridView dgvUserActivity;
    private DataGridViewTextBoxColumn colUser;
    private DataGridViewTextBoxColumn colUserAppointments;
    private Label lblUserActivityTitle;
}
