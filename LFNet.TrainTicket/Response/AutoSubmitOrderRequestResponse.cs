namespace LFNet.TrainTicket.Response
{
    /// <summary>
    /// {"validateMessagesShowId":"_validatorMessage","status":true,"httpstatus":200,"data":{"result":"G1#3056A35BD7DA7800C571B67497FB680B0AC997E2261FBC8DAED43AB8#1003253365401245000010032500003008350076#1","submitStatus":true},"messages":[],"validateMessages":{}}
    /// </summary>
    public class AutoSubmitOrderRequestResponse
    {
        /// <summary>
        /// "G1#3056A35BD7DA7800C571B67497FB680B0AC997E2261FBC8DAED43AB8#1003253365401245000010032500003008350076#1"
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// true
        /// </summary>
        public bool submitStatus { get; set; }
    }
}