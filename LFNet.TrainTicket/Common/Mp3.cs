using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LFNet.TrainTicket.Common
{
    /// <summary>
    /// ��Win32Apiʵ�ֵ�ý�岥����.֧��Mp3,wma,wmv
    /// </summary>
    public class Mp3
    {
        public Mp3()
        {
        }
        //����API����ʹ�õ��ַ������� 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        private string Name = "";
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        private string durLength = "";
        [MarshalAs(UnmanagedType.LPTStr, SizeConst = 128)]
        private string TemStr = "";
        int ilong;
        //���岥��״̬ö�ٱ���
        public enum State
        {
            mPlaying = 1,
            mPuase = 2,
            mStop = 3
        };
        //�ṹ����
        public struct structMCI
        {
            public bool bMut;
            public int iDur;
            public int iPos;
            public int iVol;
            public int iBal;
            public string iName;
            public State state;
        };
 
        public structMCI mc = new structMCI();
 
        //ȡ�ò����ļ�����
        public string FileName
        {
            get
            {
                return mc.iName;
            }
            set
            {
                //ASCIIEncoding asc = new ASCIIEncoding(); 
                try
                {
                    TemStr = "";
                    TemStr = TemStr.PadLeft(127, Convert.ToChar(" "));
                    Name = Name.PadLeft(260, Convert.ToChar(" "));
                    mc.iName = value;
                    ilong = ApiClass.GetShortPathName(mc.iName, Name, Name.Length);
                    Name = GetCurrPath(Name);
                    //Name = "open " + Convert.ToChar(34) + Name + Convert.ToChar(34) + " alias media";
                    Name = "open " + Convert.ToChar(34) + Name + Convert.ToChar(34) + " alias media";
                    ilong = ApiClass.mciSendString("close all", TemStr, TemStr.Length, 0);
                    ilong = ApiClass.mciSendString(Name, TemStr, TemStr.Length, 0);
                    ilong = ApiClass.mciSendString("set media time format milliseconds", TemStr, TemStr.Length, 0);
                    mc.state = State.mStop;
                }
                catch
                {
                    MessageBox.Show("��������!");
                }
            }
        }
        //����
        public void play()
        {
            TemStr = "";
            TemStr = TemStr.PadLeft(127, Convert.ToChar(" "));
            ApiClass.mciSendString("play media", TemStr, TemStr.Length, 0);
            mc.state = State.mPlaying;
        }
        //ֹͣ
        public void StopT()
        {
            TemStr = "";
            TemStr = TemStr.PadLeft(128, Convert.ToChar(" "));
            ilong = ApiClass.mciSendString("close media", TemStr, 128, 0);
            ilong = ApiClass.mciSendString("close all", TemStr, 128, 0);
            mc.state = State.mStop;
        }
 
        public void Puase()
        {
            TemStr = "";
            TemStr = TemStr.PadLeft(128, Convert.ToChar(" "));
            ilong = ApiClass.mciSendString("pause media", TemStr, TemStr.Length, 0);
            mc.state = State.mPuase;
        }
        private string GetCurrPath(string name)
        {
            if (name.Length < 1) return "";
            name = name.Trim();
            name = name.Substring(0, name.Length - 1);
            return name;
        }
        //��ʱ��
        public int Duration
        {
            get
            {
                durLength = "";
                durLength = durLength.PadLeft(128, Convert.ToChar(" "));
                ApiClass.mciSendString("status media length", durLength, durLength.Length, 0);
                durLength = durLength.Trim();
                if (durLength == "") return 0;
                return (int)(Convert.ToDouble(durLength) / 1000f);
            }
        }
 
        //��ǰʱ��
        public int CurrentPosition
        {
            get
            {
                durLength = "";
                durLength = durLength.PadLeft(128, Convert.ToChar(" "));
                ApiClass.mciSendString("status media position", durLength, durLength.Length, 0);
                mc.iPos = (int)(Convert.ToDouble(durLength) / 1000f);
                return mc.iPos;
            }
        }
    }

    public class ApiClass
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
            string lpszLongPath,
            string shortFile,
            int cchBuffer
            );

        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int mciSendString(
            string lpstrCommand,
            string lpstrReturnString,
            int uReturnLength,
            int hwndCallback
            );
    }
}