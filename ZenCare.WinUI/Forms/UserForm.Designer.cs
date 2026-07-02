namespace ZenCare.WinUI.Forms;

partial class UserForm
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
        lblSearch = new Label();
        txtSearch = new TextBox();
        chkIsActive = new CheckBox();
        btnSearch = new Button();
        btnAdd = new Button();
        btnEdit = new Button();
        btnDelete = new Button();
        btnRefresh = new Button();
        dgvUsers = new DataGridView();
        colFirstName = new DataGridViewTextBoxColumn();
        colLastName = new DataGridViewTextBoxColumn();
        colUsername = new DataGridViewTextBoxColumn();
        colEmail = new DataGridViewTextBoxColumn();
        colPhoneNumber = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        colCreatedAt = new DataGridViewTextBoxColumn();
        colLastLoginAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(chkIsActive);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 20, 24, 12);
        pnlTop.Size = new Size(1000, 84);
        pnlTop.TabIndex = 0;
        // 
        // lblSearch
        // 
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(24, 34);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(60, 15);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Username";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(128, 30);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(148, 23);
        txtSearch.TabIndex = 1;
        // 
        // chkIsActive
        // 
        chkIsActive.CheckState = CheckState.Indeterminate;
        chkIsActive.Location = new Point(860, 30);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(92, 24);
        chkIsActive.TabIndex = 6;
        chkIsActive.Text = "Active";
        chkIsActive.ThreeState = true;
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(296, 28);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 2;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(408, 28);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 3;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(520, 28);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 4;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(632, 28);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 5;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnRefresh
        // 
        btnRefresh.Location = new Point(744, 28);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 6;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // dgvUsers
        // 
        dgvUsers.AllowUserToAddRows = false;
        dgvUsers.AllowUserToDeleteRows = false;
        dgvUsers.AutoGenerateColumns = false;
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUsers.Columns.AddRange(new DataGridViewColumn[] { colFirstName, colLastName, colUsername, colEmail, colPhoneNumber, colIsActive, colCreatedAt, colLastLoginAt });
        dgvUsers.Dock = DockStyle.Fill;
        dgvUsers.Location = new Point(0, 84);
        dgvUsers.MultiSelect = false;
        dgvUsers.Name = "dgvUsers";
        dgvUsers.ReadOnly = true;
        dgvUsers.RowHeadersVisible = false;
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.Size = new Size(1000, 716);
        dgvUsers.TabIndex = 1;
        // 
        // columns
        // 
        colFirstName.DataPropertyName = "FirstName";
        colFirstName.HeaderText = "First name";
        colFirstName.Name = "colFirstName";
        colFirstName.ReadOnly = true;
        colLastName.DataPropertyName = "LastName";
        colLastName.HeaderText = "Last name";
        colLastName.Name = "colLastName";
        colLastName.ReadOnly = true;
        colUsername.DataPropertyName = "Username";
        colUsername.HeaderText = "Username";
        colUsername.Name = "colUsername";
        colUsername.ReadOnly = true;
        colEmail.DataPropertyName = "Email";
        colEmail.HeaderText = "Email";
        colEmail.Name = "colEmail";
        colEmail.ReadOnly = true;
        colPhoneNumber.DataPropertyName = "PhoneNumber";
        colPhoneNumber.HeaderText = "Phone";
        colPhoneNumber.Name = "colPhoneNumber";
        colPhoneNumber.ReadOnly = true;
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "Active";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        colCreatedAt.DataPropertyName = "CreatedAt";
        colCreatedAt.HeaderText = "CreatedAt";
        colCreatedAt.Name = "colCreatedAt";
        colCreatedAt.ReadOnly = true;
        colLastLoginAt.DataPropertyName = "LastLoginAt";
        colLastLoginAt.HeaderText = "Last login";
        colLastLoginAt.Name = "colLastLoginAt";
        colLastLoginAt.ReadOnly = true;
        // 
        // UserForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvUsers);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "UserForm";
        Text = "Users";
        Load += UserForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private Label lblSearch;
    private TextBox txtSearch;
    private CheckBox chkIsActive;
    private Button btnSearch;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnRefresh;
    private DataGridView dgvUsers;
    private DataGridViewTextBoxColumn colFirstName;
    private DataGridViewTextBoxColumn colLastName;
    private DataGridViewTextBoxColumn colUsername;
    private DataGridViewTextBoxColumn colEmail;
    private DataGridViewTextBoxColumn colPhoneNumber;
    private DataGridViewCheckBoxColumn colIsActive;
    private DataGridViewTextBoxColumn colCreatedAt;
    private DataGridViewTextBoxColumn colLastLoginAt;
}
