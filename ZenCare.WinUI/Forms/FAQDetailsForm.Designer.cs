namespace ZenCare.WinUI.Forms;

partial class FAQDetailsForm
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
        lblQuestion = new Label();
        txtQuestion = new TextBox();
        lblAnswer = new Label();
        txtAnswer = new TextBox();
        lblFAQCategory = new Label();
        cmbFAQCategory = new ComboBox();
        lblDisplayOrder = new Label();
        nudDisplayOrder = new NumericUpDown();
        chkIsActive = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudDisplayOrder).BeginInit();
        SuspendLayout();
        // 
        // lblQuestion
        // 
        lblQuestion.AutoSize = true;
        lblQuestion.Location = new Point(24, 24);
        lblQuestion.Name = "lblQuestion";
        lblQuestion.Size = new Size(55, 15);
        lblQuestion.TabIndex = 0;
        lblQuestion.Text = "Question";
        // 
        // txtQuestion
        // 
        txtQuestion.Location = new Point(24, 46);
        txtQuestion.Name = "txtQuestion";
        txtQuestion.Size = new Size(416, 23);
        txtQuestion.TabIndex = 1;
        // 
        // lblAnswer
        // 
        lblAnswer.AutoSize = true;
        lblAnswer.Location = new Point(24, 84);
        lblAnswer.Name = "lblAnswer";
        lblAnswer.Size = new Size(45, 15);
        lblAnswer.TabIndex = 2;
        lblAnswer.Text = "Answer";
        // 
        // txtAnswer
        // 
        txtAnswer.Location = new Point(24, 106);
        txtAnswer.Multiline = true;
        txtAnswer.Name = "txtAnswer";
        txtAnswer.ScrollBars = ScrollBars.Vertical;
        txtAnswer.Size = new Size(416, 140);
        txtAnswer.TabIndex = 3;
        // 
        // lblFAQCategory
        // 
        lblFAQCategory.AutoSize = true;
        lblFAQCategory.Location = new Point(24, 264);
        lblFAQCategory.Name = "lblFAQCategory";
        lblFAQCategory.Size = new Size(55, 15);
        lblFAQCategory.TabIndex = 4;
        lblFAQCategory.Text = "Category";
        // 
        // cmbFAQCategory
        // 
        cmbFAQCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFAQCategory.FormattingEnabled = true;
        cmbFAQCategory.Location = new Point(24, 286);
        cmbFAQCategory.Name = "cmbFAQCategory";
        cmbFAQCategory.Size = new Size(416, 23);
        cmbFAQCategory.TabIndex = 5;
        // 
        // lblDisplayOrder
        // 
        lblDisplayOrder.AutoSize = true;
        lblDisplayOrder.Location = new Point(24, 328);
        lblDisplayOrder.Name = "lblDisplayOrder";
        lblDisplayOrder.Size = new Size(78, 15);
        lblDisplayOrder.TabIndex = 6;
        lblDisplayOrder.Text = "Display order";
        // 
        // nudDisplayOrder
        // 
        nudDisplayOrder.Location = new Point(24, 350);
        nudDisplayOrder.Maximum = 100000;
        nudDisplayOrder.Name = "nudDisplayOrder";
        nudDisplayOrder.Size = new Size(196, 23);
        nudDisplayOrder.TabIndex = 7;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(244, 352);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(59, 19);
        chkIsActive.TabIndex = 8;
        chkIsActive.Text = "Active";
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 404);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 9;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 404);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 10;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // FAQDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 461);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsActive);
        Controls.Add(nudDisplayOrder);
        Controls.Add(lblDisplayOrder);
        Controls.Add(cmbFAQCategory);
        Controls.Add(lblFAQCategory);
        Controls.Add(txtAnswer);
        Controls.Add(lblAnswer);
        Controls.Add(txtQuestion);
        Controls.Add(lblQuestion);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FAQDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "FAQ";
        Load += FAQDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)nudDisplayOrder).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblQuestion;
    private TextBox txtQuestion;
    private Label lblAnswer;
    private TextBox txtAnswer;
    private Label lblFAQCategory;
    private ComboBox cmbFAQCategory;
    private Label lblDisplayOrder;
    private NumericUpDown nudDisplayOrder;
    private CheckBox chkIsActive;
    private Button btnSave;
    private Button btnCancel;
}
