namespace ZenCare.WinUI.Forms;

partial class AppointmentForm
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
        txtSearch = new TextBox();
        lblSearch = new Label();
        dtpAppointmentDate = new DateTimePicker();
        chkAppointmentDate = new CheckBox();
        cmbStatus = new ComboBox();
        lblStatus = new Label();
        cmbService = new ComboBox();
        lblService = new Label();
        cmbServiceCategory = new ComboBox();
        lblServiceCategory = new Label();
        cmbEmployee = new ComboBox();
        lblEmployee = new Label();
        cmbUser = new ComboBox();
        lblUser = new Label();
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        dgvAppointments = new DataGridView();
        colUserName = new DataGridViewTextBoxColumn();
        colEmployeeName = new DataGridViewTextBoxColumn();
        colServiceName = new DataGridViewTextBoxColumn();
        colAppointmentDate = new DataGridViewTextBoxColumn();
        colStartTime = new DataGridViewTextBoxColumn();
        colEndTime = new DataGridViewTextBoxColumn();
        colStatus = new DataGridViewTextBoxColumn();
        colNotes = new DataGridViewTextBoxColumn();
        colCancellationReason = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAppointments).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Controls.Add(dtpAppointmentDate);
        pnlTop.Controls.Add(chkAppointmentDate);
        pnlTop.Controls.Add(cmbStatus);
        pnlTop.Controls.Add(lblStatus);
        pnlTop.Controls.Add(cmbService);
        pnlTop.Controls.Add(lblService);
        pnlTop.Controls.Add(cmbServiceCategory);
        pnlTop.Controls.Add(lblServiceCategory);
        pnlTop.Controls.Add(cmbEmployee);
        pnlTop.Controls.Add(lblEmployee);
        pnlTop.Controls.Add(cmbUser);
        pnlTop.Controls.Add(lblUser);
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 16, 24, 12);
        pnlTop.Size = new Size(1000, 148);
        pnlTop.TabIndex = 0;
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(76, 22);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(200, 23);
        txtSearch.TabIndex = 1;
        // 
        // lblSearch
        // 
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(24, 26);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(42, 15);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Search";
        // 
        // dtpAppointmentDate
        // 
        dtpAppointmentDate.Format = DateTimePickerFormat.Short;
        dtpAppointmentDate.Location = new Point(640, 102);
        dtpAppointmentDate.Name = "dtpAppointmentDate";
        dtpAppointmentDate.Size = new Size(116, 23);
        dtpAppointmentDate.TabIndex = 14;
        // 
        // chkAppointmentDate
        // 
        chkAppointmentDate.AutoSize = true;
        chkAppointmentDate.Location = new Point(572, 104);
        chkAppointmentDate.Name = "chkAppointmentDate";
        chkAppointmentDate.Size = new Size(50, 19);
        chkAppointmentDate.TabIndex = 13;
        chkAppointmentDate.Text = "Date";
        chkAppointmentDate.UseVisualStyleBackColor = true;
        // 
        // cmbStatus
        // 
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.FormattingEnabled = true;
        cmbStatus.Location = new Point(396, 102);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(120, 23);
        cmbStatus.TabIndex = 12;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(344, 106);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 11;
        lblStatus.Text = "Status";
        // 
        // cmbService
        // 
        cmbService.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbService.FormattingEnabled = true;
        cmbService.Location = new Point(88, 102);
        cmbService.Name = "cmbService";
        cmbService.Size = new Size(120, 23);
        cmbService.TabIndex = 10;
        // 
        // lblService
        // 
        lblService.AutoSize = true;
        lblService.Location = new Point(24, 106);
        lblService.Name = "lblService";
        lblService.Size = new Size(44, 15);
        lblService.TabIndex = 9;
        lblService.Text = "Service";
        // 
        // cmbServiceCategory
        // 
        cmbServiceCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbServiceCategory.FormattingEnabled = true;
        cmbServiceCategory.Location = new Point(456, 62);
        cmbServiceCategory.Name = "cmbServiceCategory";
        cmbServiceCategory.Size = new Size(160, 23);
        cmbServiceCategory.TabIndex = 16;
        // 
        // lblServiceCategory
        // 
        lblServiceCategory.AutoSize = true;
        lblServiceCategory.Location = new Point(344, 66);
        lblServiceCategory.Name = "lblServiceCategory";
        lblServiceCategory.Size = new Size(94, 15);
        lblServiceCategory.TabIndex = 15;
        lblServiceCategory.Text = "Service category";
        // 
        // cmbEmployee
        // 
        cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEmployee.FormattingEnabled = true;
        cmbEmployee.Location = new Point(208, 62);
        cmbEmployee.Name = "cmbEmployee";
        cmbEmployee.Size = new Size(112, 23);
        cmbEmployee.TabIndex = 8;
        // 
        // lblEmployee
        // 
        lblEmployee.AutoSize = true;
        lblEmployee.Location = new Point(136, 66);
        lblEmployee.Name = "lblEmployee";
        lblEmployee.Size = new Size(59, 15);
        lblEmployee.TabIndex = 7;
        lblEmployee.Text = "Employee";
        // 
        // cmbUser
        // 
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(64, 62);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(56, 23);
        cmbUser.TabIndex = 6;
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 66);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(30, 15);
        lblUser.TabIndex = 5;
        lblUser.Text = "User";
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(744, 20);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 4;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(632, 20);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 3;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(520, 20);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 2;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(408, 20);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 1;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(296, 20);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 0;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // 
        // dgvAppointments
        // 
        dgvAppointments.AllowUserToAddRows = false;
        dgvAppointments.AllowUserToDeleteRows = false;
        dgvAppointments.AutoGenerateColumns = false;
        dgvAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAppointments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvAppointments.Columns.AddRange(new DataGridViewColumn[] { colUserName, colEmployeeName, colServiceName, colAppointmentDate, colStartTime, colEndTime, colStatus, colNotes, colCancellationReason });
        dgvAppointments.Dock = DockStyle.Fill;
        dgvAppointments.Location = new Point(0, 148);
        dgvAppointments.MultiSelect = false;
        dgvAppointments.Name = "dgvAppointments";
        dgvAppointments.ReadOnly = true;
        dgvAppointments.RowHeadersVisible = false;
        dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAppointments.Size = new Size(1000, 652);
        dgvAppointments.TabIndex = 1;
        // 
        // columns
        // 
        colUserName.DataPropertyName = "UserName";
        colUserName.HeaderText = "Client";
        colUserName.Name = "colUserName";
        colUserName.ReadOnly = true;
        colEmployeeName.DataPropertyName = "EmployeeName";
        colEmployeeName.HeaderText = "Employee";
        colEmployeeName.Name = "colEmployeeName";
        colEmployeeName.ReadOnly = true;
        colServiceName.DataPropertyName = "ServiceName";
        colServiceName.HeaderText = "Service";
        colServiceName.Name = "colServiceName";
        colServiceName.ReadOnly = true;
        colAppointmentDate.DataPropertyName = "AppointmentDate";
        colAppointmentDate.HeaderText = "Date";
        colAppointmentDate.Name = "colAppointmentDate";
        colAppointmentDate.ReadOnly = true;
        colStartTime.DataPropertyName = "StartTime";
        colStartTime.HeaderText = "Start";
        colStartTime.Name = "colStartTime";
        colStartTime.ReadOnly = true;
        colEndTime.DataPropertyName = "EndTime";
        colEndTime.HeaderText = "End";
        colEndTime.Name = "colEndTime";
        colEndTime.ReadOnly = true;
        colStatus.DataPropertyName = "Status";
        colStatus.HeaderText = "Status";
        colStatus.Name = "colStatus";
        colStatus.ReadOnly = true;
        colNotes.DataPropertyName = "Notes";
        colNotes.HeaderText = "Notes";
        colNotes.Name = "colNotes";
        colNotes.ReadOnly = true;
        colCancellationReason.DataPropertyName = "CancellationReason";
        colCancellationReason.HeaderText = "Cancellation";
        colCancellationReason.Name = "colCancellationReason";
        colCancellationReason.ReadOnly = true;
        // 
        // AppointmentForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvAppointments);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "AppointmentForm";
        Text = "Appointments";
        Load += AppointmentForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAppointments).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private TextBox txtSearch;
    private Label lblSearch;
    private DateTimePicker dtpAppointmentDate;
    private CheckBox chkAppointmentDate;
    private ComboBox cmbStatus;
    private Label lblStatus;
    private ComboBox cmbService;
    private Label lblService;
    private ComboBox cmbServiceCategory;
    private Label lblServiceCategory;
    private ComboBox cmbEmployee;
    private Label lblEmployee;
    private ComboBox cmbUser;
    private Label lblUser;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private DataGridView dgvAppointments;
    private DataGridViewTextBoxColumn colUserName;
    private DataGridViewTextBoxColumn colEmployeeName;
    private DataGridViewTextBoxColumn colServiceName;
    private DataGridViewTextBoxColumn colAppointmentDate;
    private DataGridViewTextBoxColumn colStartTime;
    private DataGridViewTextBoxColumn colEndTime;
    private DataGridViewTextBoxColumn colStatus;
    private DataGridViewTextBoxColumn colNotes;
    private DataGridViewTextBoxColumn colCancellationReason;
}
