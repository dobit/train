using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket
{
    partial class PassengerForm
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
            this.components = new System.ComponentModel.Container();
            this.passengerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbSeatDetailType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCardNo = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.tbCardType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbMobileNo = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.passengerBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // passengerBindingSource
            // 
            this.passengerBindingSource.DataSource = typeof(Passenger);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbSeatDetailType);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbCardNo);
            this.groupBox1.Controls.Add(this.tbUserName);
            this.groupBox1.Controls.Add(this.tbCardType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.tbMobileNo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 204);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // tbSeatDetailType
            // 
            this.tbSeatDetailType.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.passengerBindingSource, "SeatDetailType", true));
            this.tbSeatDetailType.FormattingEnabled = true;
            this.tbSeatDetailType.Location = new System.Drawing.Point(68, 129);
            this.tbSeatDetailType.Name = "tbSeatDetailType";
            this.tbSeatDetailType.Size = new System.Drawing.Size(121, 20);
            this.tbSeatDetailType.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "坐  席:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(118, 175);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 18);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "姓  名:";
            // 
            // tbCardNo
            // 
            this.tbCardNo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.passengerBindingSource, "CardNo", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.tbCardNo.Location = new System.Drawing.Point(68, 76);
            this.tbCardNo.Name = "tbCardNo";
            this.tbCardNo.Size = new System.Drawing.Size(116, 21);
            this.tbCardNo.TabIndex = 9;
            // 
            // tbUserName
            // 
            this.tbUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.passengerBindingSource, "Name", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.tbUserName.Location = new System.Drawing.Point(68, 23);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(116, 21);
            this.tbUserName.TabIndex = 1;
            // 
            // tbCardType
            // 
            this.tbCardType.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.passengerBindingSource, "CardType", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.tbCardType.FormattingEnabled = true;
            this.tbCardType.Location = new System.Drawing.Point(68, 50);
            this.tbCardType.Name = "tbCardType";
            this.tbCardType.Size = new System.Drawing.Size(116, 20);
            this.tbCardType.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "证  号:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "证  件:";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(27, 174);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(53, 18);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "确定(&O)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tbMobileNo
            // 
            this.tbMobileNo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.passengerBindingSource, "MobileNo", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.tbMobileNo.Location = new System.Drawing.Point(68, 103);
            this.tbMobileNo.Mask = "00000000000";
            this.tbMobileNo.Name = "tbMobileNo";
            this.tbMobileNo.Size = new System.Drawing.Size(116, 21);
            this.tbMobileNo.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "手  机:";
            // 
            // PassengerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 204);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PassengerForm";
            this.Text = "乘客设置";
            ((System.ComponentModel.ISupportInitialize)(this.passengerBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCardNo;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.ComboBox tbCardType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.MaskedTextBox tbMobileNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.BindingSource passengerBindingSource;
        private System.Windows.Forms.ComboBox tbSeatDetailType;
        private System.Windows.Forms.Label label5;

    }
}