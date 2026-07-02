namespace ZenCare.WinUI.Forms;

partial class AppointmentDetailsForm
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
        lblEmployee = new Label();
        cmbEmployee = new ComboBox();
        lblServiceCategory = new Label();
        cmbServiceCategory = new ComboBox();
        lblService = new Label();
        cmbService = new ComboBox();
        lblAppointmentDate = new Label();
        dtpAppointmentDate = new DateTimePicker();
        lblStartTime = new Label();
        dtpStartTime = new DateTimePicker();
        lblEndTime = new Label();
        dtpEndTime = new DateTimePicker();
        lblStatus = new Label();
        cmbStatus = new ComboBox();
        lblNotes = new Label();
        txtNotes = new TextBox();
        lblCancellationReason = new Label();
        txtCancellationReason = new TextBox();
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
        // lblEmployee
        // 
        lblEmployee.AutoSize = true;
        lblEmployee.Location = new Point(24, 84);
        lblEmployee.Name = "lblEmployee";
        lblEmployee.Size = new Size(59, 15);
        lblEmployee.TabIndex = 2;
        lblEmployee.Text = "Employee";
        // 
        // cmbEmployee
        // 
        cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEmployee.FormattingEnabled = true;
        cmbEmployee.Location = new Point(24, 106);
        cmbEmployee.Name = "cmbEmployee";
        cmbEmployee.Size = new Size(416, 23);
        cmbEmployee.TabIndex = 3;
        // 
        // lblService
        // 
        lblService.AutoSize = true;
        lblService.Location = new Point(24, 204);
        lblService.Name = "lblService";
        lblService.Size = new Size(44, 15);
        lblService.TabIndex = 6;
        lblService.Text = "Service";
        // 
        // cmbService
        // 
        cmbService.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbService.FormattingEnabled = true;
        cmbService.Location = new Point(24, 226);
        cmbService.Name = "cmbService";
        cmbService.Size = new Size(416, 23);
        cmbService.TabIndex = 7;
        // 
        // lblServiceCategory
        // 
        lblServiceCategory.AutoSize = true;
        lblServiceCategory.Location = new Point(24, 144);
        lblServiceCategory.Name = "lblServiceCategory";
        lblServiceCategory.Size = new Size(93, 15);
        lblServiceCategory.TabIndex = 4;
        lblServiceCategory.Text = "Service category";
        // 
        // cmbServiceCategory
        // 
        cmbServiceCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbServiceCategory.FormattingEnabled = true;
        cmbServiceCategory.Location = new Point(24, 166);
        cmbServiceCategory.Name = "cmbServiceCategory";
        cmbServiceCategory.Size = new Size(416, 23);
        cmbServiceCategory.TabIndex = 5;
        cmbServiceCategory.SelectedIndexChanged += cmbServiceCategory_SelectedIndexChanged;
        // 
        // lblAppointmentDate
        // 
        lblAppointmentDate.AutoSize = true;
        lblAppointmentDate.Location = new Point(24, 264);
        lblAppointmentDate.Name = "lblAppointmentDate";
        lblAppointmentDate.Size = new Size(31, 15);
        lblAppointmentDate.TabIndex = 8;
        lblAppointmentDate.Text = "Date";
        // 
        // dtpAppointmentDate
        // 
        dtpAppointmentDate.Format = DateTimePickerFormat.Short;
        dtpAppointmentDate.Location = new Point(24, 286);
        dtpAppointmentDate.Name = "dtpAppointmentDate";
        dtpAppointmentDate.Size = new Size(124, 23);
        dtpAppointmentDate.TabIndex = 9;
        // 
        // lblStartTime
        // 
        lblStartTime.AutoSize = true;
        lblStartTime.Location = new Point(172, 264);
        lblStartTime.Name = "lblStartTime";
        lblStartTime.Size = new Size(58, 15);
        lblStartTime.TabIndex = 10;
        lblStartTime.Text = "Start time";
        // 
        // dtpStartTime
        // 
        dtpStartTime.CustomFormat = "HH:mm";
        dtpStartTime.Format = DateTimePickerFormat.Custom;
        dtpStartTime.Location = new Point(172, 286);
        dtpStartTime.Name = "dtpStartTime";
        dtpStartTime.ShowUpDown = true;
        dtpStartTime.Size = new Size(112, 23);
        dtpStartTime.TabIndex = 11;
        // 
        // lblEndTime
        // 
        lblEndTime.AutoSize = true;
        lblEndTime.Location = new Point(308, 264);
        lblEndTime.Name = "lblEndTime";
        lblEndTime.Size = new Size(54, 15);
        lblEndTime.TabIndex = 12;
        lblEndTime.Text = "End time";
        // 
        // dtpEndTime
        // 
        dtpEndTime.CustomFormat = "HH:mm";
        dtpEndTime.Format = DateTimePickerFormat.Custom;
        dtpEndTime.Location = new Point(308, 286);
        dtpEndTime.Name = "dtpEndTime";
        dtpEndTime.ShowUpDown = true;
        dtpEndTime.Size = new Size(132, 23);
        dtpEndTime.TabIndex = 13;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(24, 324);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 14;
        lblStatus.Text = "Status";
        // 
        // cmbStatus
        // 
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.FormattingEnabled = true;
        cmbStatus.Location = new Point(24, 346);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(416, 23);
        cmbStatus.TabIndex = 15;
        // 
        // lblNotes
        // 
        lblNotes.AutoSize = true;
        lblNotes.Location = new Point(24, 384);
        lblNotes.Name = "lblNotes";
        lblNotes.Size = new Size(38, 15);
        lblNotes.TabIndex = 16;
        lblNotes.Text = "Notes";
        // 
        // txtNotes
        // 
        txtNotes.Location = new Point(24, 406);
        txtNotes.Multiline = true;
        txtNotes.Name = "txtNotes";
        txtNotes.Size = new Size(416, 60);
        txtNotes.TabIndex = 17;
        // 
        // lblCancellationReason
        // 
        lblCancellationReason.AutoSize = true;
        lblCancellationReason.Location = new Point(24, 484);
        lblCancellationReason.Name = "lblCancellationReason";
        lblCancellationReason.Size = new Size(112, 15);
        lblCancellationReason.TabIndex = 18;
        lblCancellationReason.Text = "Cancellation reason";
        // 
        // txtCancellationReason
        // 
        txtCancellationReason.Location = new Point(24, 506);
        txtCancellationReason.Multiline = true;
        txtCancellationReason.Name = "txtCancellationReason";
        txtCancellationReason.Size = new Size(416, 60);
        txtCancellationReason.TabIndex = 19;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 588);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 20;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 588);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 21;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // AppointmentDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 645);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtCancellationReason);
        Controls.Add(lblCancellationReason);
        Controls.Add(txtNotes);
        Controls.Add(lblNotes);
        Controls.Add(cmbStatus);
        Controls.Add(lblStatus);
        Controls.Add(dtpEndTime);
        Controls.Add(lblEndTime);
        Controls.Add(dtpStartTime);
        Controls.Add(lblStartTime);
        Controls.Add(dtpAppointmentDate);
        Controls.Add(lblAppointmentDate);
        Controls.Add(cmbService);
        Controls.Add(lblService);
        Controls.Add(cmbServiceCategory);
        Controls.Add(lblServiceCategory);
        Controls.Add(cmbEmployee);
        Controls.Add(lblEmployee);
        Controls.Add(cmbUser);
        Controls.Add(lblUser);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AppointmentDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Appointment";
        Load += AppointmentDetailsForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblUser;
    private ComboBox cmbUser;
    private Label lblEmployee;
    private ComboBox cmbEmployee;
    private Label lblServiceCategory;
    private ComboBox cmbServiceCategory;
    private Label lblService;
    private ComboBox cmbService;
    private Label lblAppointmentDate;
    private DateTimePicker dtpAppointmentDate;
    private Label lblStartTime;
    private DateTimePicker dtpStartTime;
    private Label lblEndTime;
    private DateTimePicker dtpEndTime;
    private Label lblStatus;
    private ComboBox cmbStatus;
    private Label lblNotes;
    private TextBox txtNotes;
    private Label lblCancellationReason;
    private TextBox txtCancellationReason;
    private Button btnSave;
    private Button btnCancel;
}
