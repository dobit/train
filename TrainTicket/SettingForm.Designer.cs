namespace LFNet.TrainTicket
{
    partial class SettingForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.randCodenumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.clicknumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.querynumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.randCodenumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clicknumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.querynumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "验证码等待:";
            // 
            // randCodenumericUpDown
            // 
            this.randCodenumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource, "RandCodeWaitSeconds", true));
            this.randCodenumericUpDown.Location = new System.Drawing.Point(103, 23);
            this.randCodenumericUpDown.Name = "randCodenumericUpDown";
            this.randCodenumericUpDown.Size = new System.Drawing.Size(58, 21);
            this.randCodenumericUpDown.TabIndex = 1;
            this.randCodenumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(168, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "秒";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "单击等待:";
            // 
            // clicknumericUpDown
            // 
            this.clicknumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource, "ClickWaitSeconds", true));
            this.clicknumericUpDown.Location = new System.Drawing.Point(103, 62);
            this.clicknumericUpDown.Name = "clicknumericUpDown";
            this.clicknumericUpDown.Size = new System.Drawing.Size(58, 21);
            this.clicknumericUpDown.TabIndex = 1;
            this.clicknumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(168, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "秒";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "确定(&O)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(110, 133);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "取消(&C)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(35, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "查询间隔:";
            // 
            // querynumericUpDown
            // 
            this.querynumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.bindingSource, "QueryWaitSeconds", true));
            this.querynumericUpDown.Location = new System.Drawing.Point(103, 95);
            this.querynumericUpDown.Name = "querynumericUpDown";
            this.querynumericUpDown.Size = new System.Drawing.Size(58, 21);
            this.querynumericUpDown.TabIndex = 1;
            this.querynumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(168, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "秒";
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(LFNet.TrainTicket.Config.SystemConfig);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 168);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.querynumericUpDown);
            this.Controls.Add(this.clicknumericUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.randCodenumericUpDown);
            this.Controls.Add(this.label1);
            this.Name = "SettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统设置";
            ((System.ComponentModel.ISupportInitialize)(this.randCodenumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clicknumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.querynumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown randCodenumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown clicknumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown querynumericUpDown;
        private System.Windows.Forms.Label label6;
    }
}