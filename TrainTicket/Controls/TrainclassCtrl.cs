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
                    ret += "QB#";
                }
                if (cbD.Checked)
                {
                    ret += "D#";
                }
                if(cbZ.Checked)
                {
                    ret += "Z#";
                }
                if (cbT.Checked)
                {
                    ret += "T#";
                }
                if (cbK.Checked)
                {
                    ret += "K#";
                }
                if (cbQT.Checked)
                {
                    ret += "QT#";
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
                            checkBox.Checked = value.Contains(checkBox.Name.Replace("cb", "") + "#");
                        }
                    }
                }

            }
        }
    }
}
