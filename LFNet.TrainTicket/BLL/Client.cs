using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LFNet.TrainTicket.Config;
using LFNet.TrainTicket.RqEntity;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// 代表一个模拟的客户端
    /// </summary>
    public class Client
    {
        #region Events

        public event EventHandler<ClientEventArgs> ClientChanged;

        /// <summary>
        /// 乘客信息发生变化
        /// </summary>
        public event EventHandler PassengersChanged;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnClientChanged(string message)
        {
            EventHandler<ClientEventArgs> handler = ClientChanged;
            if (handler != null) handler(this, new ClientEventArgs(message));
        }

        #endregion

        #region Fields
        /// <summary>
        /// 停止状态
        /// </summary>
        private bool _stop = false;

        /// <summary>
        /// 查询页面访问时间
        /// </summary>
        private DateTime _queryPageTime = DateTime.MinValue;
        /// <summary>
        /// 查询页面的动态js检测结果
        /// </summary>
        private DynamicJsResult _queryPageDynamicJsResult = null;
        #endregion

        #region  Properties
        /// <summary>
        /// 用户账户信息
        /// </summary>
        public AccountInfo Account { get; set; }

        ///乘客信息
        public List<Object> PassengerInfos { get; set; } 

        
        /// <summary>
        /// Cookie信息
        /// </summary>
        internal CookieContainer Cookie { get;private set; }

        #endregion

        #region Ctor
        public Client(AccountInfo account)
        {
            Account = account;
            Cookie=new CookieContainer();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 执行
        /// </summary>
        public async void Run()
        {
            this._stop = false;
            //Run


            this._stop = true;
        }

        /// <summary>
        /// 停止执行
        /// </summary>
        public async void Stop()
        {
            this._stop = true;
            

        }


        #region login
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns>登录时否成功</returns>
        public async Task<bool> Login()
        {

            //打开登陆页面
            var loginPageResult = await this.GetLoginPageResult();

            var randCode = await GetRandCode();

            Thread.Sleep(5000); //单击等待
            Response<LoginAysnSuggestResponse> response =
                await
                    this.LoginAsynSuggest(this.Account.Username, this.Account.Password, randCode, loginPageResult.DynamicJsResult.Key, loginPageResult.DynamicJsResult.Value);
            if (response.data != null && response.data.loginCheck == "Y")
            {
               // IsLogin = true;
                Info("登录成功");
                return true;
            }
            else
            {
                Info(response.messages[0].ToString());
                return false;
            }
        }

        #endregion

        #region Passengers

        public async Task<bool> RefreshPassengers()
        {
            OpenQueryPage();
            Response<GetPassengerDTOs> response = await  this.GetPassengers();
            if (response.data != null)
            {
                response.data.normal_passengers
            }
            else
            {
                
            }
        }

        #endregion


        /// <summary>
        /// 打开查询页面
        /// </summary>
        private async void OpenQueryPage()
        {
            if (_queryPageDynamicJsResult == null || (DateTime.Now - _queryPageTime).TotalMinutes > 20)
            {
                var queryPageResult = await this.GetQueryPageResult();
                //查询准备 打开查询页 获取页面动态js结果
                _queryPageDynamicJsResult = queryPageResult.DynamicJsResult;
                _queryPageTime = DateTime.Now;
            }
        }


        #endregion


        #region Helpers
        /// <summary>
        /// 获取一个有效的验证码
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetRandCode(int type = 0)
        {
            string vcode = "";
            do
            {
                Image image = await InterfaceProvider.GetRandCode(this, type);
                var codeByForm = image.GetVCodeByForm();
                Thread.Sleep(5000); //等待5s 输入时间
                vcode = await codeByForm;
                if (_stop) return vcode;
                var checkRandCodeAnsynResponse = await this.CheckRandCodeAnsyn(vcode, type, "");

                if (checkRandCodeAnsynResponse.data != null && checkRandCodeAnsynResponse.data.result == "1") break;
                else
                {
                    Info(checkRandCodeAnsynResponse.messages[0].ToString());
                }
            } while (true);
            return vcode;
        }

        private async void Info(string message)
        {
            OnClientChanged(message);
        }

        #endregion
    }

    /// <summary>
    /// 用户账户信息
    /// </summary>
    public class AccountInfo
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
