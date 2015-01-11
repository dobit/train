using  LFNet.TrainTicket.Controls;
namespace LFNet.TrainTicket
{
    partial class PassengersForm
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
            this.passengersSetCtrl1 = new LFNet.TrainTicket.Controls.PassengersSetCtrl();
            this.SuspendLayout();
            // 
            // passengersSetCtrl1
            // 
            this.passengersSetCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passengersSetCtrl1.Location = new System.Drawing.Point(0, 0);
            this.passengersSetCtrl1.Name = "passengersSetCtrl1";
            this.passengersSetCtrl1.Size = new System.Drawing.Size(472, 298);
            this.passengersSetCtrl1.TabIndex = 0;
            // 
            // PassengersForm
            // 
            this.ClientSize = new System.Drawing.Size(472, 298);
            this.Controls.Add(this.passengersSetCtrl1);
            this.Name = "PassengersForm";
            this.ResumeLayout(false);

        }

        #endregion

        private LFNet.TrainTicket.Controls.PassengersSetCtrl passengersSetCtrl1;

        
    }
}