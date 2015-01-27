using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using LFNet.Configuration;
using LFNet.TrainTicket.Config;

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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            ConfigFileManager.GetConfig<SystemConfig>(true);
            MainForm = new MainForm();//Form1();
            Application.Run(MainForm);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is ThreadAbortException))
            {
                HandleException((Exception)e.ExceptionObject);
            }
        }

        private static void HandleException(Exception e)
        {

            MessageBox.Show(e.ToString(), "Unhandled exception", MessageBoxButtons.OK, MessageBoxIcon.Hand);

        }

        public static Form MainForm { get; set; }
    }
}
