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
        public const string StationsUrl = "https://kyfw.12306.cn/otn/resources/js/framework/station_name.js?station_version=1.8253";




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
    }
}