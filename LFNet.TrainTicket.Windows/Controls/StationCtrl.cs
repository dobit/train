using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.Controls
{
    public partial class StationCtrl : UserControl
    {

        List<StationInfo> stationInfos= new List<StationInfo>();
        public StationCtrl()
        {
           
            InitializeComponent();
            if (!DesignMode)
            {
                stationInfos = Global.GetStations();
            }
                comboBox1.DataSource = stationInfos;
                    //.Where(p => p.PY.StartsWith(comboBox1.Text,StringComparison.OrdinalIgnoreCase) || p.Name.Contains(comboBox1.Text)).ToList();
            
            comboBox1.ValueMember = "Code";
            comboBox1.DisplayMember = "Name";
            
            listBox1.DataSource = stationInfos;
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {

            listBox1.Visible = true;
            if (e.KeyCode == Keys.Down)
            {
                if (listBox1.SelectedIndex < listBox1.Items.Count)
                listBox1.SelectedIndex++;
                comboBox1.SelectedText = listBox1.SelectedItem.ToString();
                
            }
            else if( e.KeyCode == Keys.Up)
            {
                if(listBox1.SelectedIndex>0)
                    listBox1.SelectedIndex--;
                comboBox1.SelectedText = listBox1.SelectedItem.ToString();
            }
            else
            {
                listBox1.DataSource = stationInfos.Where(
                    p =>
                    p.PY.StartsWith(comboBox1.Text, StringComparison.OrdinalIgnoreCase)||p.Name.StartsWith(comboBox1.Text, StringComparison.OrdinalIgnoreCase))
                    .Union(stationInfos.Where(p => p.Name.Contains(comboBox1.Text))).Select(p => p.Name).ToArray();
            }
         
           
        }

        public StationInfo SelectedValue
        {
            get
            {
                if (comboBox1.SelectedItem != null)
                {
                    return comboBox1.SelectedItem as StationInfo;
                    //return new StationInfo(){Code=comboBox1.SelectedValue.ToString(),Name=comboBox1.SelectedItem};
                }
                return new StationInfo();
            }
            set
            {
                if (value != null)
                    comboBox1.SelectedItem = value;
            }
        }

        //public string Value
        //{
        //    get
        //    {
        //        if (comboBox1.SelectedValue != null) return comboBox1.SelectedValue.ToString();
        //        return "";
        //    }
        //    set
        //    {
        //        if(value!=null)
        //        comboBox1.SelectedValue = value;
        //    }
        //}

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            listBox1.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            //comboBox1.Text = "";
            comboBox1.Text = listBox1.SelectedItem.ToString();
            listBox1.Visible = false;
        }
    }
}
