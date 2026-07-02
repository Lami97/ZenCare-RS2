namespace ZenCare.WinUI.Forms;

partial class PurchaseForm
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

    private void InitializeComponent()
    {
        pnlTop = new Panel();
        btnRefresh = new Button();
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        cmbPaymentStatus = new ComboBox();
        lblPaymentStatus = new Label();
        cmbStatus = new ComboBox();
        lblStatus = new Label();
        txtPurchaseNumber = new TextBox();
        lblPurchaseNumber = new Label();
        cmbUser = new ComboBox();
        lblUser = new Label();
        dgvPurchases = new DataGridView();
        colUserName = new DataGridViewTextBoxColumn();
        colPurchaseNumber = new DataGridViewTextBoxColumn();
        colStatus = new DataGridViewTextBoxColumn();
        colPaymentStatus = new DataGridViewTextBoxColumn();
        colTotalAmount = new DataGridViewTextBoxColumn();
        colPaidAt = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPurchases).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(cmbPaymentStatus);
        pnlTop.Controls.Add(lblPaymentStatus);
        pnlTop.Controls.Add(cmbStatus);
        pnlTop.Controls.Add(lblStatus);
        pnlTop.Controls.Add(txtPurchaseNumber);
        pnlTop.Controls.Add(lblPurchaseNumber);
        pnlTop.Controls.Add(cmbUser);
        pnlTop.Controls.Add(lblUser);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 16, 24, 12);
        pnlTop.Size = new Size(1000, 116);
        pnlTop.TabIndex = 0;
        // 
        // buttons
        // 
        btnSearch.Location = new Point(296, 20);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 0;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        btnAdd.Location = new Point(408, 20);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 1;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        btnEdit.Location = new Point(520, 20);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 2;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        btnDelete.Location = new Point(632, 20);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 3;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        btnRefresh.Location = new Point(744, 20);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(96, 28);
        btnRefresh.TabIndex = 4;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // filters
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 26);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(30, 15);
        lblUser.TabIndex = 5;
        lblUser.Text = "User";
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(64, 22);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(212, 23);
        cmbUser.TabIndex = 6;
        lblPurchaseNumber.AutoSize = true;
        lblPurchaseNumber.Location = new Point(24, 74);
        lblPurchaseNumber.Name = "lblPurchaseNumber";
        lblPurchaseNumber.Size = new Size(99, 15);
        lblPurchaseNumber.TabIndex = 7;
        lblPurchaseNumber.Text = "Purchase number";
        txtPurchaseNumber.Location = new Point(136, 70);
        txtPurchaseNumber.Name = "txtPurchaseNumber";
        txtPurchaseNumber.Size = new Size(132, 23);
        txtPurchaseNumber.TabIndex = 8;
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(296, 74);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 9;
        lblStatus.Text = "Status";
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.FormattingEnabled = true;
        cmbStatus.Location = new Point(344, 70);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(140, 23);
        cmbStatus.TabIndex = 10;
        lblPaymentStatus.AutoSize = true;
        lblPaymentStatus.Location = new Point(512, 74);
        lblPaymentStatus.Name = "lblPaymentStatus";
        lblPaymentStatus.Size = new Size(87, 15);
        lblPaymentStatus.TabIndex = 11;
        lblPaymentStatus.Text = "Payment status";
        cmbPaymentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPaymentStatus.FormattingEnabled = true;
        cmbPaymentStatus.Location = new Point(612, 70);
        cmbPaymentStatus.Name = "cmbPaymentStatus";
        cmbPaymentStatus.Size = new Size(140, 23);
        cmbPaymentStatus.TabIndex = 12;
        // 
        // dgvPurchases
        // 
        dgvPurchases.AllowUserToAddRows = false;
        dgvPurchases.AllowUserToDeleteRows = false;
        dgvPurchases.AutoGenerateColumns = false;
        dgvPurchases.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPurchases.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvPurchases.Columns.AddRange(new DataGridViewColumn[] { colUserName, colPurchaseNumber, colStatus, colPaymentStatus, colTotalAmount, colPaidAt });
        dgvPurchases.Dock = DockStyle.Fill;
        dgvPurchases.Location = new Point(0, 116);
        dgvPurchases.MultiSelect = false;
        dgvPurchases.Name = "dgvPurchases";
        dgvPurchases.ReadOnly = true;
        dgvPurchases.RowHeadersVisible = false;
        dgvPurchases.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPurchases.Size = new Size(1000, 684);
        dgvPurchases.TabIndex = 1;
        colUserName.DataPropertyName = "UserName";
        colUserName.HeaderText = "User";
        colUserName.Name = "colUserName";
        colUserName.ReadOnly = true;
        colPurchaseNumber.DataPropertyName = "PurchaseNumber";
        colPurchaseNumber.HeaderText = "Purchase Number";
        colPurchaseNumber.Name = "colPurchaseNumber";
        colPurchaseNumber.ReadOnly = true;
        colStatus.DataPropertyName = "Status";
        colStatus.HeaderText = "Status";
        colStatus.Name = "colStatus";
        colStatus.ReadOnly = true;
        colPaymentStatus.DataPropertyName = "PaymentStatus";
        colPaymentStatus.HeaderText = "Payment";
        colPaymentStatus.Name = "colPaymentStatus";
        colPaymentStatus.ReadOnly = true;
        colTotalAmount.DataPropertyName = "TotalAmount";
        colTotalAmount.HeaderText = "Total";
        colTotalAmount.Name = "colTotalAmount";
        colTotalAmount.ReadOnly = true;
        colPaidAt.DataPropertyName = "PaidAt";
        colPaidAt.HeaderText = "Paid At";
        colPaidAt.Name = "colPaidAt";
        colPaidAt.ReadOnly = true;
        // 
        // PurchaseForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvPurchases);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "PurchaseForm";
        Text = "Purchases";
        Load += PurchaseForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPurchases).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private ComboBox cmbPaymentStatus;
    private Label lblPaymentStatus;
    private ComboBox cmbStatus;
    private Label lblStatus;
    private TextBox txtPurchaseNumber;
    private Label lblPurchaseNumber;
    private ComboBox cmbUser;
    private Label lblUser;
    private DataGridView dgvPurchases;
    private DataGridViewTextBoxColumn colUserName;
    private DataGridViewTextBoxColumn colPurchaseNumber;
    private DataGridViewTextBoxColumn colStatus;
    private DataGridViewTextBoxColumn colPaymentStatus;
    private DataGridViewTextBoxColumn colTotalAmount;
    private DataGridViewTextBoxColumn colPaidAt;
}
