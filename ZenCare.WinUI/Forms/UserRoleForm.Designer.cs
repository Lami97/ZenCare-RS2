namespace ZenCare.WinUI.Forms;

partial class UserRoleForm
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
        cmbRole = new ComboBox();
        lblRole = new Label();
        cmbUser = new ComboBox();
        lblUser = new Label();
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        dgvUserRoles = new DataGridView();
        colUserName = new DataGridViewTextBoxColumn();
        colRoleName = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUserRoles).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Controls.Add(cmbRole);
        pnlTop.Controls.Add(lblRole);
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
        // cmbRole
        // 
        cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRole.FormattingEnabled = true;
        cmbRole.Location = new Point(420, 62);
        cmbRole.Name = "cmbRole";
        cmbRole.Size = new Size(200, 23);
        cmbRole.TabIndex = 8;
        // 
        // lblRole
        // 
        lblRole.AutoSize = true;
        lblRole.Location = new Point(364, 66);
        lblRole.Name = "lblRole";
        lblRole.Size = new Size(30, 15);
        lblRole.TabIndex = 7;
        lblRole.Text = "Role";
        // 
        // cmbUser
        // 
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(88, 62);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(240, 23);
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
        // dgvUserRoles
        // 
        dgvUserRoles.AllowUserToAddRows = false;
        dgvUserRoles.AllowUserToDeleteRows = false;
        dgvUserRoles.AutoGenerateColumns = false;
        dgvUserRoles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUserRoles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvUserRoles.Columns.AddRange(new DataGridViewColumn[] { colUserName, colRoleName });
        dgvUserRoles.Dock = DockStyle.Fill;
        dgvUserRoles.Location = new Point(0, 116);
        dgvUserRoles.MultiSelect = false;
        dgvUserRoles.Name = "dgvUserRoles";
        dgvUserRoles.ReadOnly = true;
        dgvUserRoles.RowHeadersVisible = false;
        dgvUserRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUserRoles.Size = new Size(1000, 684);
        dgvUserRoles.TabIndex = 1;
        // 
        // colUserName
        // 
        colUserName.DataPropertyName = "UserName";
        colUserName.HeaderText = "User";
        colUserName.Name = "colUserName";
        colUserName.ReadOnly = true;
        // 
        // colRoleName
        // 
        colRoleName.DataPropertyName = "RoleName";
        colRoleName.HeaderText = "Role";
        colRoleName.Name = "colRoleName";
        colRoleName.ReadOnly = true;
        // 
        // UserRoleForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvUserRoles);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "UserRoleForm";
        Text = "User Roles";
        Load += UserRoleForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUserRoles).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private TextBox txtSearch;
    private Label lblSearch;
    private ComboBox cmbRole;
    private Label lblRole;
    private ComboBox cmbUser;
    private Label lblUser;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private DataGridView dgvUserRoles;
    private DataGridViewTextBoxColumn colUserName;
    private DataGridViewTextBoxColumn colRoleName;
}
