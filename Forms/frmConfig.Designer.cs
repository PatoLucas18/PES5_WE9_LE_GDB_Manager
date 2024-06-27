namespace PES5_WE9_LE_GDB_Manager
{
    partial class frmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
            this.txtOFPath = new System.Windows.Forms.TextBox();
            this.lblOFPath = new System.Windows.Forms.Label();
            this.btnOFPath = new System.Windows.Forms.Button();
            this.btnExePath = new System.Windows.Forms.Button();
            this.lblExePath = new System.Windows.Forms.Label();
            this.txtExePath = new System.Windows.Forms.TextBox();
            this.btnGDBFolder = new System.Windows.Forms.Button();
            this.lblGDBFolder = new System.Windows.Forms.Label();
            this.txtGDBFolder = new System.Windows.Forms.TextBox();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtOFPath
            // 
            this.txtOFPath.Location = new System.Drawing.Point(108, 32);
            this.txtOFPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtOFPath.Name = "txtOFPath";
            this.txtOFPath.ReadOnly = true;
            this.txtOFPath.Size = new System.Drawing.Size(234, 20);
            this.txtOFPath.TabIndex = 0;
            // 
            // lblOFPath
            // 
            this.lblOFPath.AutoSize = true;
            this.lblOFPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOFPath.Location = new System.Drawing.Point(28, 32);
            this.lblOFPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOFPath.Name = "lblOFPath";
            this.lblOFPath.Size = new System.Drawing.Size(53, 13);
            this.lblOFPath.TabIndex = 1;
            this.lblOFPath.Text = "OF Path";
            // 
            // btnOFPath
            // 
            this.btnOFPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOFPath.Location = new System.Drawing.Point(346, 32);
            this.btnOFPath.Margin = new System.Windows.Forms.Padding(2);
            this.btnOFPath.Name = "btnOFPath";
            this.btnOFPath.Size = new System.Drawing.Size(21, 18);
            this.btnOFPath.TabIndex = 2;
            this.btnOFPath.Text = "...";
            this.btnOFPath.UseVisualStyleBackColor = true;
            this.btnOFPath.Click += new System.EventHandler(this.btnOFPath_Click);
            // 
            // btnExePath
            // 
            this.btnExePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExePath.Location = new System.Drawing.Point(346, 65);
            this.btnExePath.Margin = new System.Windows.Forms.Padding(2);
            this.btnExePath.Name = "btnExePath";
            this.btnExePath.Size = new System.Drawing.Size(21, 18);
            this.btnExePath.TabIndex = 5;
            this.btnExePath.Text = "...";
            this.btnExePath.UseVisualStyleBackColor = true;
            this.btnExePath.Click += new System.EventHandler(this.btnExePath_Click);
            // 
            // lblExePath
            // 
            this.lblExePath.AutoSize = true;
            this.lblExePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExePath.Location = new System.Drawing.Point(28, 65);
            this.lblExePath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExePath.Name = "lblExePath";
            this.lblExePath.Size = new System.Drawing.Size(58, 13);
            this.lblExePath.TabIndex = 4;
            this.lblExePath.Text = "Exe Path";
            // 
            // txtExePath
            // 
            this.txtExePath.Location = new System.Drawing.Point(108, 65);
            this.txtExePath.Margin = new System.Windows.Forms.Padding(2);
            this.txtExePath.Name = "txtExePath";
            this.txtExePath.ReadOnly = true;
            this.txtExePath.Size = new System.Drawing.Size(234, 20);
            this.txtExePath.TabIndex = 3;
            // 
            // btnGDBFolder
            // 
            this.btnGDBFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGDBFolder.Location = new System.Drawing.Point(346, 98);
            this.btnGDBFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnGDBFolder.Name = "btnGDBFolder";
            this.btnGDBFolder.Size = new System.Drawing.Size(21, 18);
            this.btnGDBFolder.TabIndex = 8;
            this.btnGDBFolder.Text = "...";
            this.btnGDBFolder.UseVisualStyleBackColor = true;
            this.btnGDBFolder.Click += new System.EventHandler(this.btnGDBFolder_Click);
            // 
            // lblGDBFolder
            // 
            this.lblGDBFolder.AutoSize = true;
            this.lblGDBFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGDBFolder.Location = new System.Drawing.Point(28, 98);
            this.lblGDBFolder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGDBFolder.Name = "lblGDBFolder";
            this.lblGDBFolder.Size = new System.Drawing.Size(72, 13);
            this.lblGDBFolder.TabIndex = 7;
            this.lblGDBFolder.Text = "GDB Folder";
            // 
            // txtGDBFolder
            // 
            this.txtGDBFolder.Location = new System.Drawing.Point(108, 98);
            this.txtGDBFolder.Margin = new System.Windows.Forms.Padding(2);
            this.txtGDBFolder.Name = "txtGDBFolder";
            this.txtGDBFolder.ReadOnly = true;
            this.txtGDBFolder.Size = new System.Drawing.Size(234, 20);
            this.txtGDBFolder.TabIndex = 6;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(134, 177);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(2);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(64, 25);
            this.btnAccept.TabIndex = 9;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(218, 177);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 25);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 268);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnGDBFolder);
            this.Controls.Add(this.lblGDBFolder);
            this.Controls.Add(this.txtGDBFolder);
            this.Controls.Add(this.btnExePath);
            this.Controls.Add(this.lblExePath);
            this.Controls.Add(this.txtExePath);
            this.Controls.Add(this.btnOFPath);
            this.Controls.Add(this.lblOFPath);
            this.Controls.Add(this.txtOFPath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOFPath;
        private System.Windows.Forms.Label lblOFPath;
        private System.Windows.Forms.Button btnOFPath;
        private System.Windows.Forms.Button btnExePath;
        private System.Windows.Forms.Label lblExePath;
        private System.Windows.Forms.TextBox txtExePath;
        private System.Windows.Forms.Button btnGDBFolder;
        private System.Windows.Forms.Label lblGDBFolder;
        private System.Windows.Forms.TextBox txtGDBFolder;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
    }
}