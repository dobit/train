using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JScript;

namespace LFNet.TrainTicket.Config
{
    /// <summary>
    /// 系统设置文件
    /// </summary>
   public class SystemConfig
    {
       private int _randCodeWaitSeconds=5;//5秒
       private int _clickWaitDelay=5*1000;
        private int _queryWaitDelay=5*1000;
        private int _randCodeCheckDelay=5*1000;
        private int _inputUserNamePasswordDelay=5*1000;//5秒
        private int _loginBtnClickDelay=5*1000;//5miao

        /// <summary>
       /// 验证码输入等待秒数
       /// </summary>
       public int RandCodeWaitSeconds
       {
           get { return _randCodeWaitSeconds; }
           set { _randCodeWaitSeconds = value; }
       }

       /// <summary>
       /// 鼠标点击等待秒数
       /// </summary>
       public int ClickWaitDelay
       {
           get { return _clickWaitDelay; }
           set { _clickWaitDelay = value; }
       }

        public int QueryWaitDelay
        {
            get { return _queryWaitDelay; }
            set { _queryWaitDelay = value; }
        }

        /// <summary>
        /// 验证码获取到验证码验证的间隔
        /// </summary>
        public int RandCodeCheckDelay
        {
            get { return _randCodeCheckDelay; }
            set { _randCodeCheckDelay = value; }
        }

        /// <summary>
        /// 输入用户名密码时间延迟
        /// </summary>
        public int InputUserNamePasswordDelay
        {
            get { return _inputUserNamePasswordDelay; }
            set { _inputUserNamePasswordDelay = value; }
        }

       /// <summary>
        /// 单击登陆按钮前的延时
       /// </summary>
        public int LoginBtnClickDelay
        {
            get { return _loginBtnClickDelay; }
            set { _loginBtnClickDelay = value; }
        }
    }
}
