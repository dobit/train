using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxWMPLib;
using LFNet.Common;
using LFNet.Common.Reflection;
using LFNet.Configuration;
using LFNet.TrainTicket.BLL;
using LFNet.TrainTicket.Common;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket
{
    public partial class ClientControl : UserControl
    {
       
        public Client client;
        public AccountInfo Account;
        private Thread t;
        public ClientControl()
        {
            InitializeComponent();
            Account = ConfigFileManager.GetConfig<AccountInfo>(true);
            this.client = new Client();
            this.accountInfoBindingSource.DataSource = Account;
            BindEvents();
            foreach (Passenger passenger in Global.GetPassengers())
            {
                if (passenger.Checked)
                {
                    //if (this.client.Passengers.Find(p=>p.Name==passenger.Name)==null)
                        this.client.Passengers.Add(passenger);
                    //this.passengersCheckedBoxList.Items.Add(passenger.Name,true);
                }
            }
            psControl.Passengers = this.client.Passengers;
            axWindowsMediaPlayer1.PlayStateChange += axWindowsMediaPlayer1_PlayStateChange;
            webBrowser1.Navigate(System.AppDomain.CurrentDomain.BaseDirectory + "music/payme.html"); 
            
        }
       
        private void PlayMusic()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "music/";
                if (!System.IO.Directory.Exists(path))
                {
                    return;
                }
                string[] files = System.IO.Directory.GetFiles(path, "*.mp3");
                if (files.Length > 0)
                {
                    System.Random random = new Random();
                    int p = random.Next(0, files.Length - 1);

                    axWindowsMediaPlayer1.URL = files[p];
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }
            catch (Exception ex)
            {
                PlaySound();
            }
        }

        void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                PlayMusic();
            }
        }


        private void seatTypesCtrl1_ValueChanged(object sender, EventArgs e)
        {
            lbSeattypes.Text = seatTypesCtrl1.Value;
        }


        private void btnQuery_Click(object sender, EventArgs e)
        {
            this.client.Account = ObjectCopier.BinaryClone(Account) as AccountInfo;
            Account.Save();
            Account.SaveConfig();
            if (string.IsNullOrEmpty(this.client.Account.Username))
            {
                MessageBox.Show("用户名不能空");
                tbUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(this.client.Account.Password))
            {
                MessageBox.Show("密码不能空");
                tbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(this.client.Account.FromStationTeleCode) || string.IsNullOrEmpty(this.client.Account.ToStationTeleCode))
            {
                MessageBox.Show("出发站和目的站设置不正确");
                return;
            }
            if (string.IsNullOrEmpty(this.client.Account.StartTimeStr))
            {
                MessageBox.Show("请选择出发时间");
                tbTime.Focus();
                return;
            }
            if (this.client.Passengers.Count(p=>p.Checked) == 0)
            {
                MessageBox.Show("请选择乘客");
                return;
            }
            if (string.IsNullOrEmpty(this.client.Account.SeatOrder))
            {
                MessageBox.Show("请选择购买车票的席别顺序");
                return;
            }
            string passengerNames = string.Join(",",
                                                this.client.Passengers.Where(p => p.Checked).Select(
                                                    p => p.Name).ToArray());
            string confirmText = string.Format("乘车人为：{0}\r\n车票优先顺序为:{1}", passengerNames, this.client.Account.SeatOrder);
            
            if (MessageBox.Show(confirmText, "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               
                btnQuery.Enabled = false;
                btnStop.Enabled = true;
               
                t = new Thread(Run) { IsBackground = true };
                t.Start();
            }
        }

        public void Run(object accountObject)
        {
            client.Run();//客户端执行
        }
        private Mp3 soundPlayer;
        private void PlaySound()
        {

            string path = System.AppDomain.CurrentDomain.BaseDirectory + "music/";
            if (!System.IO.Directory.Exists(path))
            {
                return;
            }
            string[] files = System.IO.Directory.GetFiles(path, "*.mp3");
            if (files.Length > 0)
            {
                System.Random random = new Random();
                int p = random.Next(0, files.Length - 1);
                if (soundPlayer!=null)
                {
                    soundPlayer.StopT();
                   
                }
                soundPlayer = new Mp3();
                soundPlayer.FileName = files[p];
                soundPlayer.play();
            }
        }


        private void Log( Exception ex)
        {
            Log( ex.Message + "\r\n\t" + ex.StackTrace);

        }

        public void Log(string message)
        {
            if (lbPassengers.InvokeRequired)
            {
                lbPassengers.Invoke(new Action(() =>
                    {
                        var l = lbPassengers.Text.Length - 10000;
                        lbPassengers.Text = lbPassengers.Text.Substring(l > 0 ? l : 0) + client.Account.Username + "," + System.DateTime.Now.ToString("hh:mm:ss:") + message + "\r\n";
                        lbPassengers.SelectionStart = lbPassengers.TextLength;
                        lbPassengers.ScrollToCaret();

                    }));
            }
            else
            {
                var l = lbPassengers.Text.Length - 10000;
                lbPassengers.Text = lbPassengers.Text.Substring(l > 0 ? l : 0) + client.Account.Username + "," + System.DateTime.Now.ToString("hh:mm:ss:") + message + "\r\n";
                lbPassengers.SelectionStart = lbPassengers.TextLength;
                lbPassengers.ScrollToCaret();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            new Thread(StopAll) { IsBackground = true }.Start();

        }

        public void StopAll()
        {
           // this.Stop = true;
            this.client.Stop();
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                        {
                            axWindowsMediaPlayer1.Ctlcontrols.stop();

                        }));
                }
                else
                {
                    axWindowsMediaPlayer1.Ctlcontrols.stop();

                }
            }
            if (soundPlayer != null)
            {
                soundPlayer.StopT();
                //soundPlayer.Dispose();
                soundPlayer = null;
            }

            if (t!=null&&System.Threading.Thread.CurrentThread != t)
            {
                int i = 0;
                while (i < 5000 && t.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(i += 100);
                }
                if (t.IsAlive)
                {
                    t.Abort();
                }
            }
            this.Invoke(new Action(() =>
                {
                    btnQuery.Enabled = true;
                    btnStop.Enabled = false;
                }));
        }

        private void btnOpenExplorer_Click(object sender, EventArgs e)
        {
            string url = "https://kyfw.12306.cn/otn/index/initMy12306";
            string addtional = "Cookie: " + client.Cookie.GetCookieHeader(new Uri(url)) + Environment.NewLine;

            WebBrowser webBrowser = new WebBrowser();

            CookieCollection cookieCollection = client.Cookie.GetCookies(new Uri("https://kyfw.12306.cn/otn/"));
            foreach (Cookie cookie in cookieCollection)
            {
                InternetSetCookie("https://kyfw.12306.cn/otn/", cookie.Name, cookie.Value + ";expires=Sun,22-Feb-2099 00:00:00 GMT");
            }
            webBrowser.Navigate(url, true);
        }


        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.client.Account = ObjectCopier.BinaryClone(Account) as AccountInfo;
            Account.Save();
            Account.SaveConfig();
           // ConfigFileManager.SaveConfig<AccountInfo>();
            this.client.Login();
           
        }

        private void BindEvents()
        {
            this.client.Alarm += Client_Alarm;
            this.client.ClientChanged += Client_ClientChanged;
            this.client.Error += Client_Error;
            this.client.ExcuteStateChanged += Client_ExcuteStateChanged;
            this.client.LoginStateChanged += Client_LoginStateChanged;
            this.client.PassengersChanged += client_PassengersChanged;

        }

        private void client_PassengersChanged(object sender, ClientEventArgs<List<Passenger>> e)
        {
            psControl.Invoke(new Action(() =>
            {
                psControl.Passengers = this.client.Passengers;
            }));


        }

        private void Client_ExcuteStateChanged(object sender, ClientEventArgs<ExcuteState> e)
        {
            btnQuery.Invoke(new Action(() =>
            {
                btnQuery.Enabled = (e.State == ExcuteState.Stopped);
            }));
            
            this.Log(e.State.ToString());
        }

        private void Client_Error(object sender, ClientEventArgs<Exception> e)
        {
            this.Log(e.State);
        }

        private void Client_ClientChanged(object sender, ClientEventArgs<string> e)
        {
            this.Log(e.State);
        }

        public void Client_Alarm(object obj,ClientEventArgs e )
        {
            this.PlayMusic();
        }


        private void Client_LoginStateChanged(object sender, ClientEventArgs<LoginState> e)
        {
            btnLogin.Invoke(new Action(() =>
            {
                btnLogin.Enabled = e.State != LoginState.Login;
            }));
            
            this.Log(e.State.ToString());
        }

        private void tbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbPassword.Focus();
            }
        }

        private void tbUsername_Leave(object sender, EventArgs e)
        {
            if (tbUsername.Text.Trim().Length > 4)
            {
                AccountInfo accountInfo =ObjectCopier.BinaryClone(AccountManager.GetAccountInfo(tbUsername.Text.Trim())) as  AccountInfo;
                if (accountInfo != null)
                {
                   // this.client.Account = accountInfo;
                    this.accountInfoBindingSource.DataSource = accountInfo;// this.client.Account;
                    Account = accountInfo;
                }
            }
        }

        private void tbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.accountInfoBindingSource.SuspendBinding();
            var temp = FromStationCtrl.SelectedValue;// this.Account.FromStationInfo;
            this.Account.FromStationInfo = ToStationCtrl.SelectedValue;
            this.Account.ToStationInfo = temp;
            FromStationCtrl.SelectedValue = ToStationCtrl.SelectedValue;
            ToStationCtrl.SelectedValue = temp;
            this.accountInfoBindingSource.ResumeBinding();

        }

        private void tbUsername_TextChanged(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                Parent.Text = tbUsername.Text;
            }
        }

        }
}
