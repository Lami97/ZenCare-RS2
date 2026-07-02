namespace ZenCare.WinUI.Forms;

partial class ReviewForm
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
        btnDelete = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        btnSearch = new Button();
        cmbStatus = new ComboBox();
        lblStatus = new Label();
        nudRating = new NumericUpDown();
        lblRating = new Label();
        cmbProduct = new ComboBox();
        lblProduct = new Label();
        cmbAppointment = new ComboBox();
        lblAppointment = new Label();
        cmbUser = new ComboBox();
        lblUser = new Label();
        dgvReviews = new DataGridView();
        colUserName = new DataGridViewTextBoxColumn();
        colProductName = new DataGridViewTextBoxColumn();
        colAppointmentId = new DataGridViewTextBoxColumn();
        colRating = new DataGridViewTextBoxColumn();
        colStatus = new DataGridViewTextBoxColumn();
        colComment = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudRating).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvReviews).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(cmbStatus);
        pnlTop.Controls.Add(lblStatus);
        pnlTop.Controls.Add(nudRating);
        pnlTop.Controls.Add(lblRating);
        pnlTop.Controls.Add(cmbProduct);
        pnlTop.Controls.Add(lblProduct);
        pnlTop.Controls.Add(cmbAppointment);
        pnlTop.Controls.Add(lblAppointment);
        pnlTop.Controls.Add(cmbUser);
        pnlTop.Controls.Add(lblUser);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(24, 16, 24, 12);
        pnlTop.Size = new Size(1000, 116);
        pnlTop.TabIndex = 0;
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
        // cmbStatus
        // 
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.FormattingEnabled = true;
        cmbStatus.Location = new Point(628, 70);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(140, 23);
        cmbStatus.TabIndex = 14;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(580, 74);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 13;
        lblStatus.Text = "Status";
        // 
        // nudRating
        // 
        nudRating.Location = new Point(500, 70);
        nudRating.Maximum = 5;
        nudRating.Name = "nudRating";
        nudRating.Size = new Size(56, 23);
        nudRating.TabIndex = 12;
        // 
        // lblRating
        // 
        lblRating.AutoSize = true;
        lblRating.Location = new Point(448, 74);
        lblRating.Name = "lblRating";
        lblRating.Size = new Size(41, 15);
        lblRating.TabIndex = 11;
        lblRating.Text = "Rating";
        // 
        // cmbProduct
        // 
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProduct.FormattingEnabled = true;
        cmbProduct.Location = new Point(312, 70);
        cmbProduct.Name = "cmbProduct";
        cmbProduct.Size = new Size(116, 23);
        cmbProduct.TabIndex = 10;
        // 
        // lblProduct
        // 
        lblProduct.AutoSize = true;
        lblProduct.Location = new Point(256, 74);
        lblProduct.Name = "lblProduct";
        lblProduct.Size = new Size(49, 15);
        lblProduct.TabIndex = 9;
        lblProduct.Text = "Product";
        // 
        // cmbAppointment
        // 
        cmbAppointment.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAppointment.FormattingEnabled = true;
        cmbAppointment.Location = new Point(112, 70);
        cmbAppointment.Name = "cmbAppointment";
        cmbAppointment.Size = new Size(124, 23);
        cmbAppointment.TabIndex = 8;
        // 
        // lblAppointment
        // 
        lblAppointment.AutoSize = true;
        lblAppointment.Location = new Point(24, 74);
        lblAppointment.Name = "lblAppointment";
        lblAppointment.Size = new Size(77, 15);
        lblAppointment.TabIndex = 7;
        lblAppointment.Text = "Appointment";
        // 
        // cmbUser
        // 
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(64, 22);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(212, 23);
        cmbUser.TabIndex = 6;
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 26);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(30, 15);
        lblUser.TabIndex = 5;
        lblUser.Text = "User";
        // 
        // dgvReviews
        // 
        dgvReviews.AllowUserToAddRows = false;
        dgvReviews.AllowUserToDeleteRows = false;
        dgvReviews.AutoGenerateColumns = false;
        dgvReviews.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvReviews.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvReviews.Columns.AddRange(new DataGridViewColumn[] { colUserName, colProductName, colAppointmentId, colRating, colStatus, colComment });
        dgvReviews.Dock = DockStyle.Fill;
        dgvReviews.Location = new Point(0, 116);
        dgvReviews.MultiSelect = false;
        dgvReviews.Name = "dgvReviews";
        dgvReviews.ReadOnly = true;
        dgvReviews.RowHeadersVisible = false;
        dgvReviews.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvReviews.Size = new Size(1000, 684);
        dgvReviews.TabIndex = 1;
        // 
        // colUserName
        // 
        colUserName.DataPropertyName = "UserName";
        colUserName.HeaderText = "User";
        colUserName.Name = "colUserName";
        colUserName.ReadOnly = true;
        // 
        // colProductName
        // 
        colProductName.DataPropertyName = "ProductName";
        colProductName.HeaderText = "Product";
        colProductName.Name = "colProductName";
        colProductName.ReadOnly = true;
        // 
        // colAppointmentId
        // 
        colAppointmentId.DataPropertyName = "AppointmentId";
        colAppointmentId.HeaderText = "Appointment";
        colAppointmentId.Name = "colAppointmentId";
        colAppointmentId.ReadOnly = true;
        // 
        // colRating
        // 
        colRating.DataPropertyName = "Rating";
        colRating.HeaderText = "Rating";
        colRating.Name = "colRating";
        colRating.ReadOnly = true;
        // 
        // colStatus
        // 
        colStatus.DataPropertyName = "Status";
        colStatus.HeaderText = "Status";
        colStatus.Name = "colStatus";
        colStatus.ReadOnly = true;
        // 
        // colComment
        // 
        colComment.DataPropertyName = "Comment";
        colComment.HeaderText = "Comment";
        colComment.Name = "colComment";
        colComment.ReadOnly = true;
        // 
        // ReviewForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvReviews);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "ReviewForm";
        Text = "Reviews";
        Load += ReviewForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudRating).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvReviews).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private ComboBox cmbStatus;
    private Label lblStatus;
    private NumericUpDown nudRating;
    private Label lblRating;
    private ComboBox cmbProduct;
    private Label lblProduct;
    private ComboBox cmbAppointment;
    private Label lblAppointment;
    private ComboBox cmbUser;
    private Label lblUser;
    private DataGridView dgvReviews;
    private DataGridViewTextBoxColumn colUserName;
    private DataGridViewTextBoxColumn colProductName;
    private DataGridViewTextBoxColumn colAppointmentId;
    private DataGridViewTextBoxColumn colRating;
    private DataGridViewTextBoxColumn colStatus;
    private DataGridViewTextBoxColumn colComment;
}
