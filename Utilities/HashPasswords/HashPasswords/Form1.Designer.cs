namespace HashPasswords
{
    partial class Form1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.rdbWinAuth = new System.Windows.Forms.RadioButton();
            this.rdbSqlAuth = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHash = new System.Windows.Forms.Button();
            this.rdbAllClear = new System.Windows.Forms.RadioButton();
            this.rdbOneCandidate = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCandidateId = new System.Windows.Forms.TextBox();
            this.lblProcess = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUser);
            this.groupBox1.Controls.Add(this.rdbWinAuth);
            this.groupBox1.Controls.Add(this.rdbSqlAuth);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtDB);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(750, 298);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection Parameters";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(494, 174);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(178, 23);
            this.btnTest.TabIndex = 10;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(113, 196);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(283, 20);
            this.txtPassword.TabIndex = 9;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(113, 163);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(282, 20);
            this.txtUser.TabIndex = 8;
            // 
            // rdbWinAuth
            // 
            this.rdbWinAuth.AutoSize = true;
            this.rdbWinAuth.Location = new System.Drawing.Point(259, 113);
            this.rdbWinAuth.Name = "rdbWinAuth";
            this.rdbWinAuth.Size = new System.Drawing.Size(140, 17);
            this.rdbWinAuth.TabIndex = 7;
            this.rdbWinAuth.Text = "Windows Authentication";
            this.rdbWinAuth.UseVisualStyleBackColor = true;
            this.rdbWinAuth.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rdbSqlAuth
            // 
            this.rdbSqlAuth.AutoSize = true;
            this.rdbSqlAuth.Checked = true;
            this.rdbSqlAuth.Location = new System.Drawing.Point(41, 113);
            this.rdbSqlAuth.Name = "rdbSqlAuth";
            this.rdbSqlAuth.Size = new System.Drawing.Size(151, 17);
            this.rdbSqlAuth.TabIndex = 6;
            this.rdbSqlAuth.TabStop = true;
            this.rdbSqlAuth.Text = "SQL Server Authentication";
            this.rdbSqlAuth.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User name:";
            // 
            // txtDB
            // 
            this.txtDB.Location = new System.Drawing.Point(113, 62);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(286, 20);
            this.txtDB.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "DB Name:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(113, 30);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(286, 20);
            this.txtServer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblProcess);
            this.groupBox2.Controls.Add(this.txtCandidateId);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnHash);
            this.groupBox2.Controls.Add(this.rdbAllClear);
            this.groupBox2.Controls.Add(this.rdbOneCandidate);
            this.groupBox2.Location = new System.Drawing.Point(12, 281);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(750, 299);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Hash Candidates Password";
            // 
            // btnHash
            // 
            this.btnHash.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHash.Location = new System.Drawing.Point(244, 227);
            this.btnHash.Name = "btnHash";
            this.btnHash.Size = new System.Drawing.Size(215, 42);
            this.btnHash.TabIndex = 2;
            this.btnHash.Text = "HASH NOW";
            this.btnHash.UseVisualStyleBackColor = true;
            this.btnHash.Click += new System.EventHandler(this.btnHash_Click);
            // 
            // rdbAllClear
            // 
            this.rdbAllClear.AutoSize = true;
            this.rdbAllClear.Checked = true;
            this.rdbAllClear.Location = new System.Drawing.Point(35, 89);
            this.rdbAllClear.Name = "rdbAllClear";
            this.rdbAllClear.Size = new System.Drawing.Size(220, 17);
            this.rdbAllClear.TabIndex = 1;
            this.rdbAllClear.TabStop = true;
            this.rdbAllClear.Text = "Hash for ALL with \'Clear\' password format";
            this.rdbAllClear.UseVisualStyleBackColor = true;
            // 
            // rdbOneCandidate
            // 
            this.rdbOneCandidate.AutoSize = true;
            this.rdbOneCandidate.Location = new System.Drawing.Point(35, 42);
            this.rdbOneCandidate.Name = "rdbOneCandidate";
            this.rdbOneCandidate.Size = new System.Drawing.Size(141, 17);
            this.rdbOneCandidate.TabIndex = 0;
            this.rdbOneCandidate.Text = "Hash for ONE candidate";
            this.rdbOneCandidate.UseVisualStyleBackColor = true;
            this.rdbOneCandidate.CheckedChanged += new System.EventHandler(this.rdbOneCandidate_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(321, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Candidate Id: ";
            // 
            // txtCandidateId
            // 
            this.txtCandidateId.Enabled = false;
            this.txtCandidateId.Location = new System.Drawing.Point(400, 41);
            this.txtCandidateId.Name = "txtCandidateId";
            this.txtCandidateId.Size = new System.Drawing.Size(149, 20);
            this.txtCandidateId.TabIndex = 4;
            // 
            // lblProcess
            // 
            this.lblProcess.AutoSize = true;
            this.lblProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcess.ForeColor = System.Drawing.Color.Purple;
            this.lblProcess.Location = new System.Drawing.Point(32, 179);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(0, 13);
            this.lblProcess.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 604);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Hash Passwords utility";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.RadioButton rdbWinAuth;
        private System.Windows.Forms.RadioButton rdbSqlAuth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnHash;
        private System.Windows.Forms.RadioButton rdbAllClear;
        private System.Windows.Forms.RadioButton rdbOneCandidate;
        private System.Windows.Forms.TextBox txtCandidateId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblProcess;
    }
}

