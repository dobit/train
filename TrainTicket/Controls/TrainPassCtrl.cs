using System.Windows.Forms;

namespace LFNet.TrainTicket.Controls
{
    public partial class TrainPassCtrl : UserControl
    {
        public TrainPassCtrl()
        {
            InitializeComponent();
        }

        public string Value
        {
            get
            {
                foreach (var control in Controls)
                {
                    if (control is RadioButton)
                    {
                        RadioButton radioButton = (RadioButton) control;
                        if (radioButton.Checked)
                        {

                            return radioButton.Tag.ToString();
                        }
                    }
                }
                return "";
            }
            set
            {
                foreach (var control in Controls)
                {
                    if (control is RadioButton)
                    {
                        RadioButton radioButton = (RadioButton) control;
                        if (radioButton.Tag.ToString() == value)
                        {
                            radioButton.Checked = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}
