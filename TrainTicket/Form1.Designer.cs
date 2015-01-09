namespace LFNet.TrainTicket
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
       
        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.adPanel = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnOpenExplorer = new System.Windows.Forms.Button();
            this.cbForce = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbPassengers = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAutoVCode = new System.Windows.Forms.CheckBox();
            this.cbShowRealYp = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.nudDelay = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.tbToStation = new LFNet.TrainTicket.Controls.StationCtrl();
            this.tbFromStation = new LFNet.TrainTicket.Controls.StationCtrl();
            this.passengersSetCtrl3 = new LFNet.TrainTicket.Controls.PassengersSetCtrl();
            this.label5 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbSeattypes = new System.Windows.Forms.Label();
            this.seatTypesCtrl1 = new LFNet.TrainTicket.Controls.SeatTypesCtrl();
            this.btnQuery = new System.Windows.Forms.Button();
            this.tbTrainPass = new LFNet.TrainTicket.Controls.TrainPassCtrl();
            this.tbTrainclass = new LFNet.TrainTicket.Controls.TrainclassCtrl();
            this.tbTrainNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTime = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.accountInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.adPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelay)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // adPanel
            // 
            this.adPanel.Controls.Add(this.webBrowser1);
            this.adPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.adPanel.Location = new System.Drawing.Point(0, 588);
            this.adPanel.Name = "adPanel";
            this.adPanel.Size = new System.Drawing.Size(784, 120);
            this.adPanel.TabIndex = 9;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(784, 120);
            this.webBrowser1.TabIndex = 2;
            // 
            // tbUsername
            // 
            this.tbUsername.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.accountInfoBindingSource, "Username", true));
            this.tbUsername.Location = new System.Drawing.Point(80, 21);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(191, 21);
            this.tbUsername.TabIndex = 9;
            this.toolTip1.SetToolTip(this.tbUsername, "登陆用的用户名");
            // 
            // tbPassword
            // 
            this.tbPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.accountInfoBindingSource, "Password", true));
            this.tbPassword.Location = new System.Drawing.Point(342, 20);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(148, 21);
            this.tbPassword.TabIndex = 11;
            this.toolTip1.SetToolTip(this.tbPassword, "登陆用的密码");
            // 
            // btnOpenExplorer
            // 
            this.btnOpenExplorer.Location = new System.Drawing.Point(632, 313);
            this.btnOpenExplorer.Name = "btnOpenExplorer";
            this.btnOpenExplorer.Size = new System.Drawing.Size(93, 23);
            this.btnOpenExplorer.TabIndex = 38;
            this.btnOpenExplorer.Text = "打开浏览器";
            this.toolTip1.SetToolTip(this.btnOpenExplorer, "单击将打开浏览器，如果已经登录则不需要再登录");
            this.btnOpenExplorer.UseVisualStyleBackColor = true;
            this.btnOpenExplorer.Click += new System.EventHandler(this.btnOpenExplorer_Click);
            // 
            // cbForce
            // 
            this.cbForce.AutoSize = true;
            this.cbForce.Location = new System.Drawing.Point(164, 313);
            this.cbForce.Name = "cbForce";
            this.cbForce.Size = new System.Drawing.Size(72, 16);
            this.cbForce.TabIndex = 43;
            this.cbForce.Text = "强制下单";
            this.toolTip1.SetToolTip(this.cbForce, "勾选该勾后，找到下单串就会下单");
            this.cbForce.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 708);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 28;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1Collapsed = true;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(784, 563);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 29;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(200, 100);
            this.splitContainer2.SplitterDistance = 32;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbPassengers);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 391);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(784, 172);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "日志";
            // 
            // lbPassengers
            // 
            this.lbPassengers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPassengers.Location = new System.Drawing.Point(3, 17);
            this.lbPassengers.Name = "lbPassengers";
            this.lbPassengers.Size = new System.Drawing.Size(778, 152);
            this.lbPassengers.TabIndex = 0;
            this.lbPassengers.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAutoVCode);
            this.groupBox1.Controls.Add(this.cbShowRealYp);
            this.groupBox1.Controls.Add(this.cbForce);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.nudDelay);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tbToStation);
            this.groupBox1.Controls.Add(this.tbFromStation);
            this.groupBox1.Controls.Add(this.passengersSetCtrl3);
            this.groupBox1.Controls.Add(this.btnOpenExplorer);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lbSeattypes);
            this.groupBox1.Controls.Add(this.seatTypesCtrl1);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.tbTrainPass);
            this.groupBox1.Controls.Add(this.tbTrainclass);
            this.groupBox1.Controls.Add(this.tbTrainNo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbTime);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbDate);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(784, 342);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "购票设置";
            // 
            // cbAutoVCode
            // 
            this.cbAutoVCode.AutoSize = true;
            this.cbAutoVCode.Location = new System.Drawing.Point(320, 315);
            this.cbAutoVCode.Name = "cbAutoVCode";
            this.cbAutoVCode.Size = new System.Drawing.Size(108, 16);
            this.cbAutoVCode.TabIndex = 46;
            this.cbAutoVCode.Text = "自动识别验证码";
            this.cbAutoVCode.UseVisualStyleBackColor = true;
            // 
            // cbShowRealYp
            // 
            this.cbShowRealYp.AutoSize = true;
            this.cbShowRealYp.Location = new System.Drawing.Point(242, 313);
            this.cbShowRealYp.Name = "cbShowRealYp";
            this.cbShowRealYp.Size = new System.Drawing.Size(72, 16);
            this.cbShowRealYp.TabIndex = 44;
            this.cbShowRealYp.Text = "余票检查";
            this.cbShowRealYp.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(139, 313);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 42;
            this.label11.Text = "秒";
            // 
            // nudDelay
            // 
            this.nudDelay.Location = new System.Drawing.Point(84, 310);
            this.nudDelay.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDelay.Name = "nudDelay";
            this.nudDelay.Size = new System.Drawing.Size(49, 21);
            this.nudDelay.TabIndex = 41;
            this.nudDelay.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 313);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 40;
            this.label10.Text = "刷新间隔:";
            // 
            // tbToStation
            // 
            this.tbToStation.AutoSize = true;
            this.tbToStation.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.accountInfoBindingSource, "ToStationTeleCode", true));
            this.tbToStation.Location = new System.Drawing.Point(286, 24);
            this.tbToStation.Margin = new System.Windows.Forms.Padding(4);
            this.tbToStation.Name = "tbToStation";
            this.tbToStation.Size = new System.Drawing.Size(124, 23);
            this.tbToStation.TabIndex = 32;
            this.tbToStation.Value = "";
            // 
            // tbFromStation
            // 
            this.tbFromStation.AutoSize = true;
            this.tbFromStation.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.accountInfoBindingSource, "FromStationTeleCode", true));
            this.tbFromStation.Location = new System.Drawing.Point(82, 24);
            this.tbFromStation.Margin = new System.Windows.Forms.Padding(4);
            this.tbFromStation.Name = "tbFromStation";
            this.tbFromStation.Size = new System.Drawing.Size(124, 23);
            this.tbFromStation.TabIndex = 31;
            this.tbFromStation.Value = "";
            // 
            // passengersSetCtrl3
            // 
            this.passengersSetCtrl3.Location = new System.Drawing.Point(84, 91);
            this.passengersSetCtrl3.Name = "passengersSetCtrl3";
            this.passengersSetCtrl3.Size = new System.Drawing.Size(561, 143);
            this.passengersSetCtrl3.TabIndex = 45;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 94);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 36;
            this.label5.Text = "乘客选择：";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(547, 313);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 35;
            this.btnStop.Text = "停止(&S)";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 34;
            this.label1.Text = "购票顺序:";
            // 
            // lbSeattypes
            // 
            this.lbSeattypes.AutoSize = true;
            this.lbSeattypes.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.accountInfoBindingSource, "SeatOrder", true));
            this.lbSeattypes.Location = new System.Drawing.Point(80, 284);
            this.lbSeattypes.Name = "lbSeattypes";
            this.lbSeattypes.Size = new System.Drawing.Size(29, 12);
            this.lbSeattypes.TabIndex = 33;
            this.lbSeattypes.Text = "硬卧";
            // 
            // seatTypesCtrl1
            // 
            this.seatTypesCtrl1.AutoSize = true;
            this.seatTypesCtrl1.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.seatTypesCtrl1.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.accountInfoBindingSource, "SeatOrder", true));
            this.seatTypesCtrl1.Location = new System.Drawing.Point(82, 241);
            this.seatTypesCtrl1.Margin = new System.Windows.Forms.Padding(4);
            this.seatTypesCtrl1.Name = "seatTypesCtrl1";
            this.seatTypesCtrl1.Size = new System.Drawing.Size(600, 31);
            this.seatTypesCtrl1.TabIndex = 24;
            this.seatTypesCtrl1.Value = "其他,无座,硬座,软座,硬卧,软卧,高级软卧,二等座,一等座,特等座,商务座";
            this.seatTypesCtrl1.ValueChanged += new System.EventHandler(this.seatTypesCtrl1_ValueChanged);
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(432, 313);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(88, 23);
            this.btnQuery.TabIndex = 7;
            this.btnQuery.Text = "开始执行(&R)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // tbTrainPass
            // 
            this.tbTrainPass.Location = new System.Drawing.Point(638, 49);
            this.tbTrainPass.Margin = new System.Windows.Forms.Padding(4);
            this.tbTrainPass.Name = "tbTrainPass";
            this.tbTrainPass.Size = new System.Drawing.Size(142, 26);
            this.tbTrainPass.TabIndex = 25;
            this.tbTrainPass.Value = "QB";
            // 
            // tbTrainclass
            // 
           
            this.tbTrainclass.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.accountInfoBindingSource, "TrainClass", true));
            this.tbTrainclass.Location = new System.Drawing.Point(243, 49);
            this.tbTrainclass.Margin = new System.Windows.Forms.Padding(4);
            this.tbTrainclass.Name = "tbTrainclass";
            this.tbTrainclass.Size = new System.Drawing.Size(402, 28);
            this.tbTrainclass.TabIndex = 23;
            this.tbTrainclass.Value = "";
            // 
            // tbTrainNo
            // 
            this.tbTrainNo.Location = new System.Drawing.Point(82, 53);
            this.tbTrainNo.Name = "tbTrainNo";
            this.tbTrainNo.Size = new System.Drawing.Size(156, 21);
            this.tbTrainNo.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "目的地：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "出发车次:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "出发地:";
            // 
            // tbTime
            // 
            this.tbTime.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.accountInfoBindingSource, "StartTimeStr", true));
            this.tbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tbTime.FormattingEnabled = true;
            this.tbTime.Items.AddRange(new object[] {
            "00:00--23:59",
            "00:00--06:00",
            "06:00--12:00",
            "12:00--18:00",
            "18:00--23:59"});
            this.tbTime.Location = new System.Drawing.Point(665, 24);
            this.tbTime.Name = "tbTime";
            this.tbTime.Size = new System.Drawing.Size(97, 20);
            this.tbTime.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(601, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "出发时间:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(425, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "出发日期:";
            // 
            // tbDate
            // 
            this.tbDate.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.accountInfoBindingSource, "TrainDate", true));
            this.tbDate.Location = new System.Drawing.Point(488, 24);
            this.tbDate.MaxDate = new System.DateTime(2020, 10, 9, 0, 0, 0, 0);
            this.tbDate.MinDate = new System.DateTime(2012, 10, 9, 0, 0, 0, 0);
            this.tbDate.Name = "tbDate";
            this.tbDate.Size = new System.Drawing.Size(106, 21);
            this.tbDate.TabIndex = 9;
            this.tbDate.Value = new System.DateTime(2012, 10, 9, 0, 0, 0, 0);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnLogin);
            this.groupBox2.Controls.Add(this.tbUsername);
            this.groupBox2.Controls.Add(this.tbPassword);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(784, 49);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "账户信息";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(519, 19);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 12;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 8;
            this.label6.Text = "用户名:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(305, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 10;
            this.label9.Text = "密码：";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于AToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 25);
            this.menuStrip1.TabIndex = 30;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            this.关于AToolStripMenuItem.Size = new System.Drawing.Size(60, 21);
            this.关于AToolStripMenuItem.Text = "关于(&A)";
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(75, 23);
            this.axWindowsMediaPlayer1.TabIndex = 31;
            this.axWindowsMediaPlayer1.Visible = false;
            // 
            // accountInfoBindingSource
            // 
            this.accountInfoBindingSource.DataSource = typeof(LFNet.TrainTicket.Entity.AccountInfo);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 730);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.adPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "自动购票程序                  作者：dobit QQ:156798087";
            this.adPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelay)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountInfoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel adPanel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbSeattypes;
        private Controls.StationCtrl tbToStation;
        private Controls.StationCtrl tbFromStation;
        private Controls.SeatTypesCtrl seatTypesCtrl1;
        private System.Windows.Forms.Button btnQuery;
        private Controls.TrainPassCtrl tbTrainPass;
        private Controls.TrainclassCtrl tbTrainclass;
        private System.Windows.Forms.TextBox tbTrainNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tbTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker tbDate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox lbPassengers;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 关于AToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOpenExplorer;
       
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudDelay;
        private System.Windows.Forms.Label label10;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.CheckBox cbForce;
        private System.Windows.Forms.CheckBox cbShowRealYp;
        private Controls.PassengersSetCtrl passengersSetCtrl3;
        private System.Windows.Forms.CheckBox cbAutoVCode;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.BindingSource accountInfoBindingSource;
    }
}

