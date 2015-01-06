using System.Windows.Forms;

namespace LFNet.TrainTicket.Controls
{
    public partial class TrainclassCtrl : UserControl
    {
        public TrainclassCtrl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        public string Value
        {
            get { 
                string ret = "";
                if(cbQB.Checked)
                {
                    return "";
                }
                foreach (Control control in this.panel2.Controls)
                {
                    var checkBox = control as CheckBox;
                    if (checkBox != null&&checkBox.Checked)
                    {
                        ret += control.Name.Replace("cb", "")+",";
                    }
                }
                return ret;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    foreach (Control control in this.panel2.Controls)
                    {
                        var checkBox = control as CheckBox;
                        if (checkBox != null)
                        {
                            checkBox.Checked = true;
                        }
                    }
                }
                else
                {
                    foreach (Control control in this.panel2.Controls)
                    {
                        var checkBox = control as CheckBox;
                        if (checkBox != null)
                        {
                            checkBox.Checked = value.Contains(checkBox.Name.Replace("cb", "") + ",");
                        }
                    }
                }

            }
        }

        private void cbQB_CheckedChanged(object sender, System.EventArgs e)
        {
            var v = cbQB.Checked;
            foreach (Control control in this.panel2.Controls)
            {
                var checkBox = control as CheckBox;
                if (checkBox != null)
                {
                    checkBox.Checked = v;
                }
            }
        }
    }
}
