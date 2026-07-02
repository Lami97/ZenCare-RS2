namespace ZenCare.WinUI.Forms;

partial class FAQForm
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
        chkIsActive = new CheckBox();
        cmbFAQCategory = new ComboBox();
        lblFAQCategory = new Label();
        txtQuestion = new TextBox();
        lblQuestion = new Label();
        dgvFAQs = new DataGridView();
        colQuestion = new DataGridViewTextBoxColumn();
        colAnswer = new DataGridViewTextBoxColumn();
        colFAQCategoryName = new DataGridViewTextBoxColumn();
        colDisplayOrder = new DataGridViewTextBoxColumn();
        colIsActive = new DataGridViewCheckBoxColumn();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvFAQs).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnDelete);
        pnlTop.Controls.Add(btnEdit);
        pnlTop.Controls.Add(btnAdd);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(chkIsActive);
        pnlTop.Controls.Add(cmbFAQCategory);
        pnlTop.Controls.Add(lblFAQCategory);
        pnlTop.Controls.Add(txtQuestion);
        pnlTop.Controls.Add(lblQuestion);
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
        btnRefresh.TabIndex = 7;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(632, 20);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(96, 28);
        btnDelete.TabIndex = 6;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnEdit
        // 
        btnEdit.Location = new Point(520, 20);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(96, 28);
        btnEdit.TabIndex = 5;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(408, 20);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(96, 28);
        btnAdd.TabIndex = 4;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnSearch
        // 
        btnSearch.Location = new Point(296, 20);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(96, 28);
        btnSearch.TabIndex = 3;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(360, 72);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(68, 19);
        chkIsActive.TabIndex = 9;
        chkIsActive.Text = "IsActive";
        chkIsActive.ThreeState = true;
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // cmbFAQCategory
        // 
        cmbFAQCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFAQCategory.FormattingEnabled = true;
        cmbFAQCategory.Location = new Point(104, 70);
        cmbFAQCategory.Name = "cmbFAQCategory";
        cmbFAQCategory.Size = new Size(220, 23);
        cmbFAQCategory.TabIndex = 8;
        // 
        // lblFAQCategory
        // 
        lblFAQCategory.AutoSize = true;
        lblFAQCategory.Location = new Point(24, 74);
        lblFAQCategory.Name = "lblFAQCategory";
        lblFAQCategory.Size = new Size(55, 15);
        lblFAQCategory.TabIndex = 7;
        lblFAQCategory.Text = "Category";
        // 
        // txtQuestion
        // 
        txtQuestion.Location = new Point(88, 22);
        txtQuestion.Name = "txtQuestion";
        txtQuestion.Size = new Size(188, 23);
        txtQuestion.TabIndex = 1;
        // 
        // lblQuestion
        // 
        lblQuestion.AutoSize = true;
        lblQuestion.Location = new Point(24, 26);
        lblQuestion.Name = "lblQuestion";
        lblQuestion.Size = new Size(55, 15);
        lblQuestion.TabIndex = 0;
        lblQuestion.Text = "Question";
        // 
        // dgvFAQs
        // 
        dgvFAQs.AllowUserToAddRows = false;
        dgvFAQs.AllowUserToDeleteRows = false;
        dgvFAQs.AutoGenerateColumns = false;
        dgvFAQs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvFAQs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvFAQs.Columns.AddRange(new DataGridViewColumn[] { colQuestion, colAnswer, colFAQCategoryName, colDisplayOrder, colIsActive });
        dgvFAQs.Dock = DockStyle.Fill;
        dgvFAQs.Location = new Point(0, 116);
        dgvFAQs.MultiSelect = false;
        dgvFAQs.Name = "dgvFAQs";
        dgvFAQs.ReadOnly = true;
        dgvFAQs.RowHeadersVisible = false;
        dgvFAQs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvFAQs.Size = new Size(1000, 684);
        dgvFAQs.TabIndex = 1;
        // 
        // colQuestion
        // 
        colQuestion.DataPropertyName = "Question";
        colQuestion.HeaderText = "Question";
        colQuestion.Name = "colQuestion";
        colQuestion.ReadOnly = true;
        // 
        // colAnswer
        // 
        colAnswer.DataPropertyName = "Answer";
        colAnswer.HeaderText = "Answer";
        colAnswer.Name = "colAnswer";
        colAnswer.ReadOnly = true;
        // 
        // colFAQCategoryName
        // 
        colFAQCategoryName.DataPropertyName = "FAQCategoryName";
        colFAQCategoryName.HeaderText = "Category";
        colFAQCategoryName.Name = "colFAQCategoryName";
        colFAQCategoryName.ReadOnly = true;
        // 
        // colDisplayOrder
        // 
        colDisplayOrder.DataPropertyName = "DisplayOrder";
        colDisplayOrder.HeaderText = "Order";
        colDisplayOrder.Name = "colDisplayOrder";
        colDisplayOrder.ReadOnly = true;
        // 
        // colIsActive
        // 
        colIsActive.DataPropertyName = "IsActive";
        colIsActive.HeaderText = "Active";
        colIsActive.Name = "colIsActive";
        colIsActive.ReadOnly = true;
        // 
        // FAQForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 800);
        Controls.Add(dgvFAQs);
        Controls.Add(pnlTop);
        FormBorderStyle = FormBorderStyle.None;
        Name = "FAQForm";
        Text = "FAQs";
        Load += FAQForm_Load;
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvFAQs).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel pnlTop;
    private Button btnRefresh;
    private Button btnDelete;
    private Button btnEdit;
    private Button btnAdd;
    private Button btnSearch;
    private CheckBox chkIsActive;
    private ComboBox cmbFAQCategory;
    private Label lblFAQCategory;
    private TextBox txtQuestion;
    private Label lblQuestion;
    private DataGridView dgvFAQs;
    private DataGridViewTextBoxColumn colQuestion;
    private DataGridViewTextBoxColumn colAnswer;
    private DataGridViewTextBoxColumn colFAQCategoryName;
    private DataGridViewTextBoxColumn colDisplayOrder;
    private DataGridViewCheckBoxColumn colIsActive;
}
