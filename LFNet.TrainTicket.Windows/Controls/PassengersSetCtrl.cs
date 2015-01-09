using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.Controls
{
    public partial class PassengersSetCtrl : UserControl
    {
        private bool IsAddMode;
        public PassengersSetCtrl()
        {
            InitializeComponent();

            Reset();
            listView1.ItemChecked += listView1_ItemChecked;
        }

        public void Reset()
        {
            try
            {
                listView1.Items.Clear();
                foreach (Passenger passenger in Config.BuyTicketConfig.Instance.Passengers)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Tag = passenger;
                    listViewItem.Checked = passenger.Checked;

                    listViewItem.SubItems.AddRange(new string[]
                    {
                        passenger.Name, passenger.CardType.ToString(), passenger.CardNo, passenger.MobileNo.ToString(),
                        passenger.SeatDetailType.ToString()
                    });
                    listView1.Items.Add(listViewItem);
                }
            }
            catch (Exception exception)
            {
                
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            (e.Item.Tag as Passenger).Checked = e.Item.Checked;
           Save();
        }


        public void Save()
        {
            List<Passenger> passengers = new List<Passenger>();
            foreach (ListViewItem item in listView1.Items)
            {
                passengers.Add(item.Tag as Passenger);
            }
            Config.BuyTicketConfig.Instance.Passengers = passengers;

            Config.BuyTicketConfig.Save();
        }
       

     

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Passenger passenger = new Passenger();
            PassengerForm passengerForm=new PassengerForm();
            passengerForm.Value = passenger;
            if(passengerForm.ShowDialog(this)==DialogResult.OK)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Tag = passenger;
                listViewItem.Checked = passenger.Checked;
                listViewItem.SubItems.AddRange(new string[] { passenger.Name, passenger.CardType.ToString(), passenger.CardNo, passenger.MobileNo.ToString(),passenger.SeatDetailType.ToString() });
                listView1.Items.Add(listViewItem);
                Save();
            }
            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count==0)
            {
                MessageBox.Show("请先选择一位乘客");
                return;
            }
            Passenger passenger = listView1.SelectedItems[0].Tag as Passenger;

            PassengerForm passengerForm = new PassengerForm();
            passengerForm.Value = passenger;
            if (passengerForm.ShowDialog(this) == DialogResult.OK)
            {

                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Tag == passenger)
                    {
                       
                        item.SubItems.Clear();

                        item.SubItems.AddRange(new string[]
                                                   {
                                                       passenger.Name,
                                                       passenger.CardType.ToString(), passenger.CardNo,
                                                       passenger.MobileNo.ToString(),passenger.SeatDetailType.ToString()
                                                   });
                    }
                }
                Save();
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    listView1.Items.Remove(selectedItem);
                }
                Save();
            }
        }
    }
}
