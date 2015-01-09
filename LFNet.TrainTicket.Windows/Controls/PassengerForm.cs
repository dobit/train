using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket
{
    public partial class PassengerForm : Form
    {
        private Passenger _value;

        public PassengerForm()
        {
            InitializeComponent();
            this.tbCardType.Items.AddRange(Enum.GetNames(typeof(CardType)));
            this.tbSeatDetailType.Items.AddRange(Enum.GetNames(typeof(SeatDetailType)));
        }

        public Passenger Value
        {
            get { return _value; }
            set { _value = value; passengerBindingSource.DataSource = value; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tbUserName.Text.Length == 0 || tbCardNo.Text.Length == 0)
            {
                MessageBox.Show("姓名和证号不能空");
                return;
            }
            _value.Name = tbUserName.Text;
            _value.CardNo = tbCardNo.Text;
            _value.MobileNo = tbMobileNo.Text;
            _value.CardType = (CardType)Enum.Parse(typeof(CardType), tbCardType.SelectedItem.ToString());
            _value.SeatDetailType =
                (SeatDetailType) Enum.Parse(typeof (SeatDetailType), tbSeatDetailType.SelectedItem.ToString());
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
