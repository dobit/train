using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LFNet.Configuration;
using LFNet.TrainTicket.Config;

namespace LFNet.TrainTicket
{
    public partial class SettingForm : Form
    {
        private readonly SystemConfig systemConfig=new SystemConfig();
        public SettingForm()
        {
            InitializeComponent();
            systemConfig=ConfigFileManager.GetConfig<SystemConfig>(true);
            bindingSource.DataSource = systemConfig;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            systemConfig.SaveConfig();
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
