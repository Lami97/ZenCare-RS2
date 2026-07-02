namespace ZenCare.WinUI.Forms;

partial class ReviewDetailsForm
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
        lblAppointment = new Label();
        cmbAppointment = new ComboBox();
        lblProduct = new Label();
        cmbProduct = new ComboBox();
        lblRating = new Label();
        nudRating = new NumericUpDown();
        lblStatus = new Label();
        cmbStatus = new ComboBox();
        lblComment = new Label();
        txtComment = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudRating).BeginInit();
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
        // lblAppointment
        // 
        lblAppointment.AutoSize = true;
        lblAppointment.Location = new Point(24, 84);
        lblAppointment.Name = "lblAppointment";
        lblAppointment.Size = new Size(77, 15);
        lblAppointment.TabIndex = 2;
        lblAppointment.Text = "Appointment";
        // 
        // cmbAppointment
        // 
        cmbAppointment.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAppointment.FormattingEnabled = true;
        cmbAppointment.Location = new Point(24, 106);
        cmbAppointment.Name = "cmbAppointment";
        cmbAppointment.Size = new Size(416, 23);
        cmbAppointment.TabIndex = 3;
        // 
        // lblProduct
        // 
        lblProduct.AutoSize = true;
        lblProduct.Location = new Point(24, 144);
        lblProduct.Name = "lblProduct";
        lblProduct.Size = new Size(49, 15);
        lblProduct.TabIndex = 4;
        lblProduct.Text = "Product";
        // 
        // cmbProduct
        // 
        cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProduct.FormattingEnabled = true;
        cmbProduct.Location = new Point(24, 166);
        cmbProduct.Name = "cmbProduct";
        cmbProduct.Size = new Size(416, 23);
        cmbProduct.TabIndex = 5;
        // 
        // lblRating
        // 
        lblRating.AutoSize = true;
        lblRating.Location = new Point(24, 204);
        lblRating.Name = "lblRating";
        lblRating.Size = new Size(41, 15);
        lblRating.TabIndex = 6;
        lblRating.Text = "Rating";
        // 
        // nudRating
        // 
        nudRating.Location = new Point(24, 226);
        nudRating.Maximum = 5;
        nudRating.Minimum = 1;
        nudRating.Name = "nudRating";
        nudRating.Size = new Size(196, 23);
        nudRating.TabIndex = 7;
        nudRating.Value = 1;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(244, 204);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 15);
        lblStatus.TabIndex = 8;
        lblStatus.Text = "Status";
        // 
        // cmbStatus
        // 
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.FormattingEnabled = true;
        cmbStatus.Location = new Point(244, 226);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(196, 23);
        cmbStatus.TabIndex = 9;
        // 
        // lblComment
        // 
        lblComment.AutoSize = true;
        lblComment.Location = new Point(24, 268);
        lblComment.Name = "lblComment";
        lblComment.Size = new Size(61, 15);
        lblComment.TabIndex = 10;
        lblComment.Text = "Comment";
        // 
        // txtComment
        // 
        txtComment.Location = new Point(24, 290);
        txtComment.Multiline = true;
        txtComment.Name = "txtComment";
        txtComment.ScrollBars = ScrollBars.Vertical;
        txtComment.Size = new Size(416, 100);
        txtComment.TabIndex = 11;
        // 
        // btnSave
        // 
        btnSave.Location = new Point(264, 416);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(84, 32);
        btnSave.TabIndex = 12;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(356, 416);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(84, 32);
        btnCancel.TabIndex = 13;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // ReviewDetailsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 473);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtComment);
        Controls.Add(lblComment);
        Controls.Add(cmbStatus);
        Controls.Add(lblStatus);
        Controls.Add(nudRating);
        Controls.Add(lblRating);
        Controls.Add(cmbProduct);
        Controls.Add(lblProduct);
        Controls.Add(cmbAppointment);
        Controls.Add(lblAppointment);
        Controls.Add(cmbUser);
        Controls.Add(lblUser);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ReviewDetailsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Review";
        Load += ReviewDetailsForm_Load;
        ((System.ComponentModel.ISupportInitialize)nudRating).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblUser;
    private ComboBox cmbUser;
    private Label lblAppointment;
    private ComboBox cmbAppointment;
    private Label lblProduct;
    private ComboBox cmbProduct;
    private Label lblRating;
    private NumericUpDown nudRating;
    private Label lblStatus;
    private ComboBox cmbStatus;
    private Label lblComment;
    private TextBox txtComment;
    private Button btnSave;
    private Button btnCancel;
}
