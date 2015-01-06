namespace LFNet.TrainTicket
{
    /// <summary>
    /// 操作的URL
    /// </summary>
    public static class ActionUrls
    {
        /// <summary>
        /// 获取站点信息的URL
        /// </summary>
        public const string StationsUrl =
            "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.8253";


        /// <summary>
        /// 登陆提交地址
        /// </summary>
        public const string LoginAysnSuggestUrl = "https://kyfw.12306.cn/otn/login/loginAysnSuggest";


        /// <summary>
        /// 获取随机码
        /// </summary>
        public const string CheckRandCodeAnsynUrl = "https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn";

        /// <summary>
        /// 查询页地址
        /// </summary>
        public const string QueryPageUrl = "https://kyfw.12306.cn/otn/leftTicket/init";

        /// <summary>
        /// 登陆页地址
        /// </summary>
        public const string LoginPageUrl = "https://kyfw.12306.cn/otn/login/init";

        /// <summary>
        /// 订单提交页面
        /// </summary>
        public const string OrderPageUrl = "https://kyfw.12306.cn/otn/confirmPassenger/initDc";

        /// <summary>
        /// 首页
        /// </summary>
        public const string HomePage = "http://www.12306.cn";

        /// <summary>
        /// 订票首页
        /// </summary>
        public const string TicketHomePage = "https://kyfw.12306.cn/otn/";

        /// <summary>
        /// 我的12306页面
        /// </summary>
        public const string InitMy12306PageUrl = "https://kyfw.12306.cn/otn/index/initMy12306";

        /// <summary>
        /// 余票查询页面
        /// </summary>
        public const string LeftTicketUrl = "https://kyfw.12306.cn/otn/leftTicket/init";
    }
}