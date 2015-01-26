using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.Controls
{
    public partial class PassengersControl : UserControl
    {
        private IList<Passenger> _passengers;

        public PassengersControl()
        {
            InitializeComponent();
           
        }


        public IList<Passenger> Passengers
        {
            get { return _passengers; }
            set
            {
                _passengers = value;
                if (value != null)
                {
                    passengersCheckedBoxList.Items.Clear();
                    foreach (Passenger passenger in _passengers)
                    {
                        passengersCheckedBoxList.Items.Add(passenger.Name, passenger.Checked);

                    }
                    
                }
            }
        }

        private void passengersCheckedBoxList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _passengers[e.Index].Checked = e.NewValue == CheckState.Checked;
        }

        
    }
}
