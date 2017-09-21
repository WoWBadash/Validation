namespace Falcon_Validation
{
    partial class frmContainerQtyUpdate
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
            this.txtSerialNoEntered = new System.Windows.Forms.TextBox();
            this.lblSerialEntered = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gbContainerQty = new System.Windows.Forms.GroupBox();
            this.lblJobNum = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblLineNum = new System.Windows.Forms.Label();
            this.lblContainerNum = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbContainerQty.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSerialNoEntered
            // 
            this.txtSerialNoEntered.Location = new System.Drawing.Point(174, 8);
            this.txtSerialNoEntered.Name = "txtSerialNoEntered";
            this.txtSerialNoEntered.Size = new System.Drawing.Size(164, 20);
            this.txtSerialNoEntered.TabIndex = 600;
            // 
            // lblSerialEntered
            // 
            this.lblSerialEntered.AutoSize = true;
            this.lblSerialEntered.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSerialEntered.ForeColor = System.Drawing.Color.White;
            this.lblSerialEntered.Location = new System.Drawing.Point(12, 9);
            this.lblSerialEntered.Name = "lblSerialEntered";
            this.lblSerialEntered.Size = new System.Drawing.Size(156, 20);
            this.lblSerialEntered.TabIndex = 602;
            this.lblSerialEntered.Text = "Enter Container #:";
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.ForeColor = System.Drawing.Color.Indigo;
            this.btnLoad.Location = new System.Drawing.Point(357, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 25);
            this.btnLoad.TabIndex = 601;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(2, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 20);
            this.label1.TabIndex = 603;
            this.label1.Text = "Container #:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(2, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 605;
            this.label2.Text = "Line Num:";
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(119, 115);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(164, 23);
            this.txtQty.TabIndex = 608;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(2, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 20);
            this.label3.TabIndex = 607;
            this.label3.Text = "Qty:";
            // 
            // gbContainerQty
            // 
            this.gbContainerQty.Controls.Add(this.lblJobNum);
            this.gbContainerQty.Controls.Add(this.label5);
            this.gbContainerQty.Controls.Add(this.lblLineNum);
            this.gbContainerQty.Controls.Add(this.lblContainerNum);
            this.gbContainerQty.Controls.Add(this.btnCancel);
            this.gbContainerQty.Controls.Add(this.btnUpdate);
            this.gbContainerQty.Controls.Add(this.txtQty);
            this.gbContainerQty.Controls.Add(this.label3);
            this.gbContainerQty.Controls.Add(this.label2);
            this.gbContainerQty.Controls.Add(this.label1);
            this.gbContainerQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbContainerQty.ForeColor = System.Drawing.Color.White;
            this.gbContainerQty.Location = new System.Drawing.Point(10, 52);
            this.gbContainerQty.Name = "gbContainerQty";
            this.gbContainerQty.Size = new System.Drawing.Size(342, 193);
            this.gbContainerQty.TabIndex = 609;
            this.gbContainerQty.TabStop = false;
            this.gbContainerQty.Text = "Container Quantity";
            // 
            // lblJobNum
            // 
            this.lblJobNum.AutoSize = true;
            this.lblJobNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblJobNum.ForeColor = System.Drawing.Color.White;
            this.lblJobNum.Location = new System.Drawing.Point(116, 88);
            this.lblJobNum.Name = "lblJobNum";
            this.lblJobNum.Size = new System.Drawing.Size(33, 20);
            this.lblJobNum.TabIndex = 615;
            this.lblJobNum.Text = "XX";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(3, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 20);
            this.label5.TabIndex = 614;
            this.label5.Text = "Job Num:";
            // 
            // lblLineNum
            // 
            this.lblLineNum.AutoSize = true;
            this.lblLineNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLineNum.ForeColor = System.Drawing.Color.White;
            this.lblLineNum.Location = new System.Drawing.Point(115, 33);
            this.lblLineNum.Name = "lblLineNum";
            this.lblLineNum.Size = new System.Drawing.Size(33, 20);
            this.lblLineNum.TabIndex = 613;
            this.lblLineNum.Text = "XX";
            // 
            // lblContainerNum
            // 
            this.lblContainerNum.AutoSize = true;
            this.lblContainerNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContainerNum.ForeColor = System.Drawing.Color.White;
            this.lblContainerNum.Location = new System.Drawing.Point(115, 63);
            this.lblContainerNum.Name = "lblContainerNum";
            this.lblContainerNum.Size = new System.Drawing.Size(33, 20);
            this.lblContainerNum.TabIndex = 612;
            this.lblContainerNum.Text = "XX";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.Indigo;
            this.btnCancel.Location = new System.Drawing.Point(59, 153);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 611;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.ForeColor = System.Drawing.Color.Indigo;
            this.btnUpdate.Location = new System.Drawing.Point(169, 153);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 610;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.Indigo;
            this.btnClose.Location = new System.Drawing.Point(512, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 25);
            this.btnClose.TabIndex = 610;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmContainerQtyUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(627, 257);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gbContainerQty);
            this.Controls.Add(this.txtSerialNoEntered);
            this.Controls.Add(this.lblSerialEntered);
            this.Controls.Add(this.btnLoad);
            this.Name = "frmContainerQtyUpdate";
            this.Text = "Container Quantity Update";
            this.Load += new System.EventHandler(this.frmContainerQtyUpdate_Load);
            this.gbContainerQty.ResumeLayout(false);
            this.gbContainerQty.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSerialNoEntered;
        private System.Windows.Forms.Label lblSerialEntered;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gbContainerQty;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblLineNum;
        private System.Windows.Forms.Label lblContainerNum;
        private System.Windows.Forms.Label lblJobNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnClose;
    }
}