using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFNet.TrainTicket.Controls
{
    public partial class CheckBoxList : UserControl
    {
        private IList<object> _selectedObjects;
        private IList<object> _dataSource;

        public CheckBoxList()
        {
           
            InitializeComponent();
            _selectedObjects=new List<object>();
        }

        public IList<Object> DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                Init();
            }
        }


        private void Init()
        {
            this.Controls.Clear();
            if (DataSource == null) return;
            foreach (var obj in DataSource)
            {
                CheckBox checkBox = new CheckBox()
                {
                    Text = obj.ToString(),
                    Dock = DockStyle.Left,
                    Location = new Point(5, 5),
                    //Margin = new Padding(5),
                    AutoSize = true,
                    Checked = false,
                    Tag=obj

                };
                 checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                //checkBox.Checked = true;
                this.Controls.Add(checkBox);

            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.Checked)
            {
                SelectedObjects.Remove(checkBox.Tag);
                SelectedObjects.Add(checkBox.Tag);
            }
            else
            {
                SelectedObjects.Remove(checkBox.Tag);
            }
        }

        public IList<Object> SelectedObjects
        {
            get { return _selectedObjects; }
            set
            {
                _selectedObjects = value;
                foreach (Control control in this.Controls)
                {
                    CheckBox checkBox = control as CheckBox;
                    if (checkBox != null) checkBox.Checked = _selectedObjects.Contains(checkBox.Tag);
                }
            }
        }
    }
}
