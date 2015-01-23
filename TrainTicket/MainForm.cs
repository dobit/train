using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFNet.TrainTicket
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CreateNewTask();
        }

        private void NewTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.BeginInvoke(new Action(CreateNewTask));
            
        }

        private void CreateNewTask()
        {
            TabPage tabPage = new TabPage("新建任务");
            ClientControl clientControl = new ClientControl() {Dock = DockStyle.Fill};
            clientControl.TextChanged += (sender1, e1) => { tabPage.Text = clientControl.Text; };
            tabPage.Controls.Add(clientControl);
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectTab(tabPage);
        }


        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab!=null)
            tabControl.TabPages.Remove(tabControl.SelectedTab);
        }

        private void OptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingForm().ShowDialog(this);
        }

        private void PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PassengersForm().ShowDialog(this);
        }

    }
}
