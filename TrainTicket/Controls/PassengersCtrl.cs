using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LFNet.TrainTicket.Config;
using System.Linq;
namespace LFNet.TrainTicket.Controls
{
    public partial class PassengersCtrl : UserControl
    {
        public PassengersCtrl()
        {
            InitializeComponent();
            Reset();
        }

        public void Reset()
        {
            this.Controls.Clear();
            IEnumerable<Passenger> passengers = Config.BuyTicketConfig.Instance.Passengers;
            foreach (Passenger passenger in passengers.Reverse())
            {
                CheckBox checkBox = new CheckBox()
                    {
                        Text = passenger.Name, 
                        Dock = DockStyle.Left, 
                        Tag = passenger
                    };
                checkBox.Checked = passenger.Checked;
                checkBox.CheckStateChanged += checkBox_CheckStateChanged;
                this.Controls.Add(checkBox);
            }

        }

        void checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox) sender;
            (checkBox.Tag as Passenger).Checked = checkBox.Checked;
            Config.BuyTicketConfig.Save();
        }

        
    }
}
