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
       private int _clickWaitSeconds=5;
        private int _queryWaitSeconds=5;

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
       public int ClickWaitSeconds
       {
           get { return _clickWaitSeconds; }
           set { _clickWaitSeconds = value; }
       }

        public int QueryWaitSeconds
        {
            get { return _queryWaitSeconds; }
            set { _queryWaitSeconds = value; }
        }
    }
}
