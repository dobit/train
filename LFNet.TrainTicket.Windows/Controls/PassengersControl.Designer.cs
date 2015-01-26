namespace LFNet.TrainTicket.Controls
{
    partial class PassengersControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.passengersCheckedBoxList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // passengersCheckedBoxList
            // 
            this.passengersCheckedBoxList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.passengersCheckedBoxList.CheckOnClick = true;
            this.passengersCheckedBoxList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passengersCheckedBoxList.FormattingEnabled = true;
            this.passengersCheckedBoxList.Location = new System.Drawing.Point(0, 0);
            this.passengersCheckedBoxList.MultiColumn = true;
            this.passengersCheckedBoxList.Name = "passengersCheckedBoxList";
            this.passengersCheckedBoxList.Size = new System.Drawing.Size(726, 35);
            this.passengersCheckedBoxList.TabIndex = 50;
            this.passengersCheckedBoxList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.passengersCheckedBoxList_ItemCheck);
            // 
            // PassengersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.passengersCheckedBoxList);
            this.Name = "PassengersControl";
            this.Size = new System.Drawing.Size(726, 35);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox passengersCheckedBoxList;
    }
}
