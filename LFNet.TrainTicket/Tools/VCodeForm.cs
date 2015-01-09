using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using LFNet.TrainTicket.Common;

namespace LFNet.TrainTicket.Tools
{
    public class VCodeForm : Form
    {
        private PictureBox pictureBox1;
        private TextBox tbCode;
        private Button button1;
        private Button button2;
        private bool stop = false;
        public Image Image { get; set; }
        public VCodeForm()
        {
            InitializeComponent();
        }
        public VCodeForm(Image image)
        {
            Image = image;
            InitializeComponent();
            this.pictureBox1.Image = image;
            tbCode.Text = new Cracker().Read(new Bitmap(image));
           new System.Threading.Thread(PlaySound){IsBackground=true}.Start();
        }

        private SoundPlayer soundPlayer;
        private Mp3 mp3;
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

                    mp3=new Mp3();
                   mp3.FileName = files[p];
                   mp3.play();
                   
                   // soundPlayer = new SoundPlayer(files[p]);
                   //soundPlayer.PlayLooping();
                  
               }
              
          
       }


        public string Value
        {
            get
            {
                if(soundPlayer!=null)
                {
                    soundPlayer.Stop();
                    soundPlayer.Dispose();
                }
                if(mp3!=null)
                {
                    mp3.StopT();
                    mp3 = null;
                }
                return tbCode.Text;
            }
        }


        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(199, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(25, 90);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(143, 21);
            this.tbCode.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(12, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "确定(&O)";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(103, 124);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "停止(&C)";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // VCodeForm
            // 
            this.AcceptButton = this.button1;
            this.ClientSize = new System.Drawing.Size(199, 162);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.pictureBox1);
            this.Name = "VCodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请输入验证码";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}