using System.Drawing;

namespace LFNet.TrainTicket.DAL
{
    public class LoginPageResult : PageResult
    {
        /// <summary>
        /// 登陆页面的验证码
        /// </summary>
        public Image RandCodeImage { get; set; }
    }
}