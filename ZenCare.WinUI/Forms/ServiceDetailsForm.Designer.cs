namespace ZenCare.WinUI.Forms;

partial class ServiceDetailsForm
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
        lblName = new Label();
        txtName = new TextBox();
        lblDescription = new Label();
        txtDescription = new TextBox();
        lblDurationMinutes = new Label();
        nudDurationMinutes = new NumericUpDown();
        lblPrice = new Label();
        nudPrice = new NumericUpDown();
        lblServiceCategory = new Label();
        cmbServiceCategory = new ComboBox();
        chkIsActive = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudDurationMinutes).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudPrice).BeginInit();
        SuspendLayout();
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(24, 24);
        lblName.Name = "lblName";
        lblName.Size = new Size(39, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Name";
        // 
        // txtName
        // 
        txtName.Location = new Point(24, 46);
        txtName.Name = "txtName";
        txtName.Size = new Size(416, 23);
        txtName.TabIndex = 1;
        // 
        // lblDescription
        // 
        lblDescription.AutoSize = true;
        lblDescription.Location = new Point(24, 84);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(67, 15);
        lblDescription.TabIndex = 2;
        lblDescription.Text = "Description";
        // 
        // txtDescription
        // 
        txtDescription.Location = new Point(24, 106);
        txtDescription.Multiline = true;
        txtDescription.Name = "txtDescription";
        txtDescription.Size = new Size(416, 60);
        txtDescription.TabIndex = 3;
        // 
        // lblDurationMinutes
        // 
        lblDurationMinutes.AutoSize = true;
        lblDurationMinutes.Location = new Point(24, 184);
        lblDurationMinutes.Name = "lblDurationMinutes";
        lblDurationMinutes.Size = new Size(102, 15);
        lblDurationMinutes.TabIndex = 4;
        lblDurationMinutes.Text = "Duration minutes";
        // 
        // nudDurationMinutes
        // 
        nudDurationMinutes.Location = new Point(24, 206);
        nudDurationMinutes.Maximum = 10000;
        nudDurationMinutes.Minimum = 1;
        nudDurationMinutes.Name = "nudDurationMinutes";
        nudDurationMinutes.Size = new Size(196, 23);
        nudDurationMinutes.TabIndex = 5;
        nudDurationMinutes.Value = 1;
        // 
        // lblPrice
        // 
        lblPrice.AutoSize = true;
        lblPrice.Location = new Point(244, 184);
        lblPrice.Name = "lblPrice";
        lblPrice.Size = new Size(33, 15);
        lblPrice.TabIndex = 6;
        lblPrice.Text = "Price";
        // 
        // nudPrice
        // 
        nudPrice.DecimalPlaces = 2;
        nudPrice.Location = new Point(244, 206);
        nudPrice.Maximum = 1000000;
        nudPrice.Minimum = 0.01m;
        nudPrice.Name = "nudPrice";
        nudPrice.Size = new Size(196, 23);
        nudPrice.TabIndex = 7;
        nudPrice.Value = 0.01m;
        // 
        // lblServiceCategory
        // 
        lblServiceCategory.AutoSize = true;
        lblServiceCategory.Location = new Point(24, 248);
        lblServiceCategory.Name = "lblServiceCategory";
        lblServiceCategory.Size = new Size(94, 15);
        lblServiceCategory.TabIndex = 8;
        lblServiceCategory.Text = "Service category";
        // 
        // cmbServiceCategory
        // 
        cmbServiceCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbServiceCategory.FormattingEnabled = true;
        cmbServiceCategory.Location = new Point(24, 270);
        cmbServiceCategory.Name = "cmbServiceCategory";
        cmbServiceCategory.Size = new Size(416, 23);
        cmbServiceCategory.TabIndex = 9;
        // 
        // chkIsActive
        // 
        chkIsActive.AutoSize = true;
        chkIsActive.Location = new Point(24, 312);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Size = new Size(68, 19);
        chkIsActive.TabIndex = 10;
        chkIsActive.Text = "IsActive";
        chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 356);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 11;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 356);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 12;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // ServiceDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 413);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkIsActive);
        Controls.Add(cmbServiceCategory);
        Controls.Add(lblServiceCategory);
        Controls.Add(nudPrice);
        Controls.Add(lblPrice);
        Controls.Add(nudDurationMinutes);
        Controls.Add(lblDurationMinutes);
        Controls.Add(txtDescription);
        Controls.Add(lblDescription);
        Controls.Add(txtName);
        Controls.Add(lblName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ServiceDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Service";
        Load += ServiceDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)nudDurationMinutes).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudPrice).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblName;
    private TextBox txtName;
    private Label lblDescription;
    private TextBox txtDescription;
    private Label lblDurationMinutes;
    private NumericUpDown nudDurationMinutes;
    private Label lblPrice;
    private NumericUpDown nudPrice;
    private Label lblServiceCategory;
    private ComboBox cmbServiceCategory;
    private CheckBox chkIsActive;
    private Button btnSave;
    private Button btnCancel;
}
