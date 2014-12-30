using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
namespace LFNet.TrainTicket.Controls
{
    public partial class SeatTypesCtrl : UserControl
    {
        public const string Seats = "商务座 特等座 一等座 二等座 高级软卧 软卧 硬卧 软座 硬座 无座 其他";

        public SeatTypesCtrl()
        {
           
            
            foreach (var seat in Seats.Split(' ').Reverse())
            {
                CheckBox checkBox = new CheckBox()
                {
                    Text = seat,
                    Dock = DockStyle.Left,
                    Location =new Point(5,5),
                    //Margin = new Padding(5),
                    AutoSize = true,
                    Checked=false

                };
                checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                //checkBox.Checked = true;
                this.Controls.Add(checkBox);
                
            }
            InitializeComponent();
        }

        public event EventHandler ValueChanged;
        public void Reset()
        {
            
            
           
        }

        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if(checkBox.Checked )
            {
                _value += "," + checkBox.Text;
            }
            else
            {
                
                _value = _value.Replace("," + checkBox.Text, "");
            }

            if(ValueChanged!=null)
            {
                ValueChanged(this, null);
            }
        }

        private string _value="";
        public string Value
        {
            get
            {
                return _value.TrimStart(',');
            }
            set
            {
                if(value==null) return;
                if (!value.StartsWith(",")) value = "," + value;
               
                    foreach (var control in this.Controls)
                    {
                        if (control is CheckBox)
                        {
                            CheckBox checkBox = ((CheckBox)control);
                            checkBox.Checked = value.Contains("," + checkBox.Text);

                        }
                    }

               
            }
        }

        private void tbSeatTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
