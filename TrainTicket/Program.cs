using System;
using System.IO;
using System.Windows.Forms;

namespace LFNet.TrainTicket
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
            System.Net.ServicePointManager.Expect100Continue = false;
            System.IO.Stream stream = typeof(Program).Assembly.GetManifestResourceStream("LFNet.TrainTicket.music.PayMe.html");

            var fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + "music/payme.html", FileMode.Create);
            stream.CopyTo(fs);
            stream.Close();
            fs.Close();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            MainForm = new Form1();
            Application.Run(MainForm);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            
        }

        public static Form1 MainForm { get; set; }
    }
}
