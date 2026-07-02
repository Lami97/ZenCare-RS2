namespace ZenCare.WinUI.Forms;

partial class EmployeeForm
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
        chkIsAvailable = new CheckBox();
        txtSpecialization = new TextBox();
        lblSpecialization = new Label();
        cmbUser = new ComboBox();
        lblUser = new Label();
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        dgvEmployees = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colUserId = new DataGridViewTextBoxColumn();
        colUserName = new DataGridViewTextBoxColumn();
        colSpecialization = new DataGridViewTextBoxColumn();
        colBio = new DataGridViewTextBoxColumn();
        colHireDate = new DataGridViewTextBoxColumn();
        colIsAvailable = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        colUpdatedAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEmployees).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Controls.Add(chkIsAvailable);
        pnlTop.Controls.Add(txtSpecialization);
        pnlTop.Controls.Add(lblSpecialization);
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
        pnlTop.Size = new Size(1000, 116);
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
        // chkIsAvailable
        // 
        chkIsAvailable.CheckState = CheckState.Indeterminate;
        chkIsAvailable.Location = new Point(472, 62);
        chkIsAvailable.Name = "chkIsAvailable";
        chkIsAvailable.Size = new Size(100, 24);
        chkIsAvailable.TabIndex = 9;
        chkIsAvailable.Text = "Available";
        chkIsAvailable.ThreeState = true;
        chkIsAvailable.UseVisualStyleBackColor = true;
        // 
        // txtSpecialization
        // 
        txtSpecialization.Location = new Point(316, 64);
        txtSpecialization.Name = "txtSpecialization";
        txtSpecialization.Size = new Size(140, 23);
        txtSpecialization.TabIndex = 8;
        // 
        // lblSpecialization
        // 
        lblSpecialization.AutoSize = true;
        lblSpecialization.Location = new Point(224, 68);
        lblSpecialization.Name = "lblSpecialization";
        lblSpecialization.Size = new Size(78, 15);
        lblSpecialization.TabIndex = 7;
        lblSpecialization.Text = "Specialization";
        // 
        // cmbUser
        // 
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(64, 64);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(140, 23);
        cmbUser.TabIndex = 6;
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 68);
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
        // dgvEmployees
        // 
        dgvEmployees.AllowUserToAddRows = false;
        dgvEmployees.AllowUserToDeleteRows = false;
        dgvEmployees.AutoGenerateColumns = false;
        dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvEmployees.Columns.AddRange(new DataGridViewColumn[] { colId, colUserId, colUserName, colSpecialization, colBio, colHireDate, colIsAvailable, colCreatedAt, colUpdatedAt });
        dgvEmployees.Dock = DockStyle.Fill;
        dgvEmployees.Location = new Point(0, 116);
        dgvEmployees.Name = "dgvEmployees";
        dgvEmployees.ReadOnly = true;
        dgvEmployees.RowHeadersVisible = false;
        dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvEmployees.Size = new Size(1000, 684);
        dgvEmployees.TabIndex = 1;
        // 
        // columns
        // 
        colId.DataPropertyName = "Id";
        colId.HeaderText = "Id";
        colId.Name = "colId";
        colId.ReadOnly = true;
        colId.Visible = false;
        colUserId.DataPropertyName = "UserId";
        colUserId.HeaderText = "UserId";
        colUserId.Name = "colUserId";
        colUserId.ReadOnly = true;
        colUserId.Visible = false;
        colUserName.DataPropertyName = "UserName";
        colUserName.HeaderText = "User";
        colUserName.Name = "colUserName";
        colUserName.ReadOnly = true;
        colSpecialization.DataPropertyName = "Specialization";
        colSpecialization.HeaderText = "Specialization";
        colSpecialization.Name = "colSpecialization";
        colSpecialization.ReadOnly = true;
        colBio.DataPropertyName = "Bio";
        colBio.HeaderText = "Bio";
        colBio.Name = "colBio";
        colBio.ReadOnly = true;
        colHireDate.DataPropertyName = "HireDate";
        colHireDate.HeaderText = "Hire Date";
        colHireDate.Name = "colHireDate";
        colHireDate.ReadOnly = true;
        colIsAvailable.DataPropertyName = "IsAvailable";
        colIsAvailable.HeaderText = "Available";
        colIsAvailable.Name = "colIsAvailable";
        colIsAvailable.ReadOnly = true;
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "CreatedAt";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        colCreatedAt.Visible = false;
        colUpdatedAt.DataPropertyName = "UpdatedAt";
        colUpdatedAt.HeaderText = "UpdatedAt";
        colUpdatedAt.Name = "colUpdatedAt";
        colUpdatedAt.ReadOnly = true;
        colUpdatedAt.Visible = false;
        // 
        // EmployeeForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvEmployees);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "EmployeeForm";
        Text = "Employees";
        Load += EmployeeForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEmployees).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private TextBox txtSearch;
    private Label lblSearch;
    private CheckBox chkIsAvailable;
    private TextBox txtSpecialization;
    private Label lblSpecialization;
    private ComboBox cmbUser;
    private Label lblUser;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private DataGridView dgvEmployees;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colUserId;
    private DataGridViewTextBoxColumn colUserName;
    private DataGridViewTextBoxColumn colSpecialization;
    private DataGridViewTextBoxColumn colBio;
    private DataGridViewTextBoxColumn colHireDate;
    private DataGridViewCheckBoxColumn colIsAvailable;
    private DataGridViewTextBoxColumn colCreatedAt;
    private DataGridViewTextBoxColumn colUpdatedAt;
}
