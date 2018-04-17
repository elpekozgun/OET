namespace OET_UI
{
    partial class frmEntity
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnOkArea = new System.Windows.Forms.Button();
            this.btnColorArea = new System.Windows.Forms.Button();
            this.lblThickness = new System.Windows.Forms.Label();
            this.txtThickness = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnOkSegment = new System.Windows.Forms.Button();
            this.btnColorSegment = new System.Windows.Forms.Button();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.txtRebarSize = new System.Windows.Forms.TextBox();
            this.txtRebarCount = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnOkDot = new System.Windows.Forms.Button();
            this.btnColorDot = new System.Windows.Forms.Button();
            this.lblLoadY = new System.Windows.Forms.Label();
            this.lblLoadX = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkY = new System.Windows.Forms.CheckBox();
            this.chkX = new System.Windows.Forms.CheckBox();
            this.txtLoadX = new System.Windows.Forms.TextBox();
            this.txtLoadY = new System.Windows.Forms.TextBox();
            this.lbXRestrain = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.txtDID = new System.Windows.Forms.TextBox();
            this.lblID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAID = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(-4, -22);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(207, 176);
            this.tabControl.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtAID);
            this.tabPage1.Controls.Add(this.btnOkArea);
            this.tabPage1.Controls.Add(this.btnColorArea);
            this.tabPage1.Controls.Add(this.lblThickness);
            this.tabPage1.Controls.Add(this.txtThickness);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(199, 150);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Area";
            // 
            // btnOkArea
            // 
            this.btnOkArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOkArea.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOkArea.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkArea.Image = global::OET_UI.resources.Checkmark_16x;
            this.btnOkArea.Location = new System.Drawing.Point(36, 117);
            this.btnOkArea.Name = "btnOkArea";
            this.btnOkArea.Size = new System.Drawing.Size(50, 25);
            this.btnOkArea.TabIndex = 6;
            this.btnOkArea.UseVisualStyleBackColor = false;
            // 
            // btnColorArea
            // 
            this.btnColorArea.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnColorArea.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColorArea.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnColorArea.Location = new System.Drawing.Point(111, 117);
            this.btnColorArea.Name = "btnColorArea";
            this.btnColorArea.Size = new System.Drawing.Size(50, 25);
            this.btnColorArea.TabIndex = 5;
            this.btnColorArea.Text = "color";
            this.btnColorArea.UseVisualStyleBackColor = true;
            // 
            // lblThickness
            // 
            this.lblThickness.AutoSize = true;
            this.lblThickness.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblThickness.Location = new System.Drawing.Point(33, 63);
            this.lblThickness.Name = "lblThickness";
            this.lblThickness.Size = new System.Drawing.Size(62, 13);
            this.lblThickness.TabIndex = 3;
            this.lblThickness.Text = "Thickness :";
            this.lblThickness.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtThickness
            // 
            this.txtThickness.BackColor = System.Drawing.Color.Gray;
            this.txtThickness.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtThickness.Location = new System.Drawing.Point(111, 61);
            this.txtThickness.Name = "txtThickness";
            this.txtThickness.Size = new System.Drawing.Size(46, 20);
            this.txtThickness.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.txtSID);
            this.tabPage2.Controls.Add(this.btnOkSegment);
            this.tabPage2.Controls.Add(this.btnColorSegment);
            this.tabPage2.Controls.Add(this.lblSize);
            this.tabPage2.Controls.Add(this.lblCount);
            this.tabPage2.Controls.Add(this.txtRebarSize);
            this.tabPage2.Controls.Add(this.txtRebarCount);
            this.tabPage2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(199, 150);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Segment";
            // 
            // btnOkSegment
            // 
            this.btnOkSegment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOkSegment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOkSegment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkSegment.Image = global::OET_UI.resources.Checkmark_16x;
            this.btnOkSegment.Location = new System.Drawing.Point(36, 117);
            this.btnOkSegment.Name = "btnOkSegment";
            this.btnOkSegment.Size = new System.Drawing.Size(50, 25);
            this.btnOkSegment.TabIndex = 14;
            this.btnOkSegment.UseVisualStyleBackColor = false;
            // 
            // btnColorSegment
            // 
            this.btnColorSegment.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnColorSegment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColorSegment.Location = new System.Drawing.Point(110, 117);
            this.btnColorSegment.Name = "btnColorSegment";
            this.btnColorSegment.Size = new System.Drawing.Size(50, 25);
            this.btnColorSegment.TabIndex = 13;
            this.btnColorSegment.Text = "color";
            this.btnColorSegment.UseVisualStyleBackColor = true;
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblSize.Location = new System.Drawing.Point(44, 76);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(60, 13);
            this.lblSize.TabIndex = 10;
            this.lblSize.Text = "rebar Size :";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblCount.Location = new System.Drawing.Point(36, 45);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(68, 13);
            this.lblCount.TabIndex = 9;
            this.lblCount.Text = "rebar Count :";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtRebarSize
            // 
            this.txtRebarSize.BackColor = System.Drawing.Color.Gray;
            this.txtRebarSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRebarSize.Location = new System.Drawing.Point(110, 76);
            this.txtRebarSize.Name = "txtRebarSize";
            this.txtRebarSize.Size = new System.Drawing.Size(46, 20);
            this.txtRebarSize.TabIndex = 7;
            // 
            // txtRebarCount
            // 
            this.txtRebarCount.BackColor = System.Drawing.Color.Gray;
            this.txtRebarCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRebarCount.Location = new System.Drawing.Point(110, 43);
            this.txtRebarCount.Name = "txtRebarCount";
            this.txtRebarCount.Size = new System.Drawing.Size(46, 20);
            this.txtRebarCount.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tabPage3.Controls.Add(this.lblID);
            this.tabPage3.Controls.Add(this.txtDID);
            this.tabPage3.Controls.Add(this.btnOkDot);
            this.tabPage3.Controls.Add(this.btnColorDot);
            this.tabPage3.Controls.Add(this.lblLoadY);
            this.tabPage3.Controls.Add(this.lblLoadX);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.chkY);
            this.tabPage3.Controls.Add(this.chkX);
            this.tabPage3.Controls.Add(this.txtLoadX);
            this.tabPage3.Controls.Add(this.txtLoadY);
            this.tabPage3.Controls.Add(this.lbXRestrain);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(199, 150);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Dot";
            // 
            // btnOkDot
            // 
            this.btnOkDot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnOkDot.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOkDot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkDot.Image = global::OET_UI.resources.Checkmark_16x;
            this.btnOkDot.Location = new System.Drawing.Point(36, 117);
            this.btnOkDot.Name = "btnOkDot";
            this.btnOkDot.Size = new System.Drawing.Size(50, 25);
            this.btnOkDot.TabIndex = 27;
            this.btnOkDot.UseVisualStyleBackColor = false;
            // 
            // btnColorDot
            // 
            this.btnColorDot.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnColorDot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColorDot.Location = new System.Drawing.Point(112, 117);
            this.btnColorDot.Name = "btnColorDot";
            this.btnColorDot.Size = new System.Drawing.Size(50, 25);
            this.btnColorDot.TabIndex = 26;
            this.btnColorDot.Text = "color";
            this.btnColorDot.UseVisualStyleBackColor = true;
            // 
            // lblLoadY
            // 
            this.lblLoadY.AutoSize = true;
            this.lblLoadY.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblLoadY.Location = new System.Drawing.Point(18, 81);
            this.lblLoadY.Name = "lblLoadY";
            this.lblLoadY.Size = new System.Drawing.Size(20, 13);
            this.lblLoadY.TabIndex = 25;
            this.lblLoadY.Text = "Y :";
            this.lblLoadY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLoadX
            // 
            this.lblLoadX.AutoSize = true;
            this.lblLoadX.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblLoadX.Location = new System.Drawing.Point(18, 55);
            this.lblLoadX.Name = "lblLoadX";
            this.lblLoadX.Size = new System.Drawing.Size(20, 13);
            this.lblLoadX.TabIndex = 24;
            this.lblLoadX.Text = "X :";
            this.lblLoadX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(33, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Loads";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkY
            // 
            this.chkY.AutoSize = true;
            this.chkY.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.chkY.Location = new System.Drawing.Point(112, 80);
            this.chkY.Name = "chkY";
            this.chkY.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkY.Size = new System.Drawing.Size(57, 17);
            this.chkY.TabIndex = 22;
            this.chkY.Text = "      : Y";
            this.chkY.UseVisualStyleBackColor = true;
            // 
            // chkX
            // 
            this.chkX.AutoSize = true;
            this.chkX.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.chkX.Location = new System.Drawing.Point(112, 54);
            this.chkX.Name = "chkX";
            this.chkX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkX.Size = new System.Drawing.Size(57, 17);
            this.chkX.TabIndex = 21;
            this.chkX.Text = "      : X";
            this.chkX.UseVisualStyleBackColor = true;
            // 
            // txtLoadX
            // 
            this.txtLoadX.BackColor = System.Drawing.Color.Gray;
            this.txtLoadX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLoadX.Location = new System.Drawing.Point(40, 52);
            this.txtLoadX.Name = "txtLoadX";
            this.txtLoadX.Size = new System.Drawing.Size(46, 20);
            this.txtLoadX.TabIndex = 18;
            // 
            // txtLoadY
            // 
            this.txtLoadY.BackColor = System.Drawing.Color.Gray;
            this.txtLoadY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLoadY.Location = new System.Drawing.Point(40, 79);
            this.txtLoadY.Name = "txtLoadY";
            this.txtLoadY.Size = new System.Drawing.Size(46, 20);
            this.txtLoadY.TabIndex = 19;
            // 
            // lbXRestrain
            // 
            this.lbXRestrain.AutoSize = true;
            this.lbXRestrain.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lbXRestrain.Location = new System.Drawing.Point(120, 33);
            this.lbXRestrain.Name = "lbXRestrain";
            this.lbXRestrain.Size = new System.Drawing.Size(54, 13);
            this.lbXRestrain.TabIndex = 15;
            this.lbXRestrain.Text = "Restraints";
            this.lbXRestrain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDID
            // 
            this.txtDID.BackColor = System.Drawing.Color.Gray;
            this.txtDID.Location = new System.Drawing.Point(40, 10);
            this.txtDID.Name = "txtDID";
            this.txtDID.Size = new System.Drawing.Size(43, 20);
            this.txtDID.TabIndex = 28;
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblID.Location = new System.Drawing.Point(10, 13);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(24, 13);
            this.lblID.TabIndex = 29;
            this.lblID.Text = "ID :";
            this.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(9, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "ID :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSID
            // 
            this.txtSID.BackColor = System.Drawing.Color.Gray;
            this.txtSID.Location = new System.Drawing.Point(39, 12);
            this.txtSID.Name = "txtSID";
            this.txtSID.Size = new System.Drawing.Size(43, 20);
            this.txtSID.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(13, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "ID :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAID
            // 
            this.txtAID.BackColor = System.Drawing.Color.Gray;
            this.txtAID.Location = new System.Drawing.Point(43, 12);
            this.txtAID.Name = "txtAID";
            this.txtAID.Size = new System.Drawing.Size(43, 20);
            this.txtAID.TabIndex = 30;
            // 
            // frmEntity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(198, 149);
            this.Controls.Add(this.tabControl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmEntity";
            this.Text = "frmDot";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblThickness;
        private System.Windows.Forms.TextBox txtThickness;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.TextBox txtRebarSize;
        private System.Windows.Forms.TextBox txtRebarCount;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lbXRestrain;
        private System.Windows.Forms.CheckBox chkY;
        private System.Windows.Forms.CheckBox chkX;
        private System.Windows.Forms.TextBox txtLoadX;
        private System.Windows.Forms.TextBox txtLoadY;
        private System.Windows.Forms.Label lblLoadY;
        private System.Windows.Forms.Label lblLoadX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnColorArea;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnColorSegment;
        private System.Windows.Forms.Button btnColorDot;
        public System.Windows.Forms.Button btnOkDot;
        public System.Windows.Forms.Button btnOkArea;
        public System.Windows.Forms.Button btnOkSegment;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSID;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.TextBox txtDID;
    }
}