using System.Windows.Forms;

namespace LFNet.TrainTicket
{
    public partial class OptionForm :  Form
    {
        public OptionForm()
        {
            InitializeComponent();
            this.buyTicketConfigBindingSource.DataSource = Config.BuyTicketConfig.Instance;
            
        }

        private void OptionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.BuyTicketConfig.Save();
        }
    }
}
