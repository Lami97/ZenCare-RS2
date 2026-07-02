namespace ZenCare.WinUI.Forms;

partial class EmployeeDetailsForm
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
        lblUser = new Label();
        cmbUser = new ComboBox();
        lblSpecialization = new Label();
        txtSpecialization = new TextBox();
        lblBio = new Label();
        txtBio = new TextBox();
        chkHasHireDate = new CheckBox();
        dtpHireDate = new DateTimePicker();
        chkIsAvailable = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(24, 24);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(30, 15);
        lblUser.TabIndex = 0;
        lblUser.Text = "User";
        // 
        // cmbUser
        // 
        cmbUser.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUser.FormattingEnabled = true;
        cmbUser.Location = new Point(24, 46);
        cmbUser.Name = "cmbUser";
        cmbUser.Size = new Size(416, 23);
        cmbUser.TabIndex = 1;
        // 
        // lblSpecialization
        // 
        lblSpecialization.AutoSize = true;
        lblSpecialization.Location = new Point(24, 84);
        lblSpecialization.Name = "lblSpecialization";
        lblSpecialization.Size = new Size(78, 15);
        lblSpecialization.TabIndex = 2;
        lblSpecialization.Text = "Specialization";
        // 
        // txtSpecialization
        // 
        txtSpecialization.Location = new Point(24, 106);
        txtSpecialization.Name = "txtSpecialization";
        txtSpecialization.Size = new Size(416, 23);
        txtSpecialization.TabIndex = 3;
        // 
        // lblBio
        // 
        lblBio.AutoSize = true;
        lblBio.Location = new Point(24, 144);
        lblBio.Name = "lblBio";
        lblBio.Size = new Size(24, 15);
        lblBio.TabIndex = 4;
        lblBio.Text = "Bio";
        // 
        // txtBio
        // 
        txtBio.Location = new Point(24, 166);
        txtBio.Multiline = true;
        txtBio.Name = "txtBio";
        txtBio.Size = new Size(416, 72);
        txtBio.TabIndex = 5;
        // 
        // chkHasHireDate
        // 
        chkHasHireDate.AutoSize = true;
        chkHasHireDate.Location = new Point(24, 258);
        chkHasHireDate.Name = "chkHasHireDate";
        chkHasHireDate.Size = new Size(94, 19);
        chkHasHireDate.TabIndex = 6;
        chkHasHireDate.Text = "Has hire date";
        chkHasHireDate.UseVisualStyleBackColor = true;
        chkHasHireDate.CheckedChanged += chkHasHireDate_CheckedChanged;
        // 
        // dtpHireDate
        // 
        dtpHireDate.Format = DateTimePickerFormat.Short;
        dtpHireDate.Location = new Point(144, 256);
        dtpHireDate.Name = "dtpHireDate";
        dtpHireDate.Size = new Size(296, 23);
        dtpHireDate.TabIndex = 7;
        // 
        // chkIsAvailable
        // 
        chkIsAvailable.AutoSize = true;
        chkIsAvailable.Location = new Point(24, 304);
        chkIsAvailable.Name = "chkIsAvailable";
        chkIsAvailable.Size = new Size(81, 19);
        chkIsAvailable.TabIndex = 8;
        chkIsAvailable.Text = "IsAvailable";
        chkIsAvailable.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 356);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 9;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 356);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 10;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // EmployeeDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 413);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsAvailable);
        Controls.Add(dtpHireDate);
        Controls.Add(chkHasHireDate);
        Controls.Add(txtBio);
        Controls.Add(lblBio);
        Controls.Add(txtSpecialization);
        Controls.Add(lblSpecialization);
        Controls.Add(cmbUser);
        Controls.Add(lblUser);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "EmployeeDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Employee";
        Load += EmployeeDetailsForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblUser;
    private ComboBox cmbUser;
    private Label lblSpecialization;
    private TextBox txtSpecialization;
    private Label lblBio;
    private TextBox txtBio;
    private CheckBox chkHasHireDate;
    private DateTimePicker dtpHireDate;
    private CheckBox chkIsAvailable;
    private Button btnSave;
    private Button btnCancel;
}
