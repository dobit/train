using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using LFNet.Net.Http;
using LFNet.TrainTicket;
using LFNet.TrainTicket.Config;
using System.Net.Http;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ;

            Console.WriteLine("bin216:"+Utils.Bin216("123456"));
            Console.WriteLine("Encode32:" + Utils.Encode32("123456"));
            var data = new Base32().stringToLongArray("1234567",true);
            Console.WriteLine("stringToLongArray:" + string.Join(",", data));
            Console.WriteLine("longArrayToString:" + new Base32().longArrayToString(data, true));
            Console.WriteLine("Base32:" + new Base32().encrypt("0000", "ABC123"));


            Console.WriteLine("encode:" + Utils.Encode32(Utils.Bin216(new Base32().encrypt("0000", "NjY1NjIy"))));
            Console.ReadLine();



            System.Net.ServicePointManager.ServerCertificateValidationCallback =(sender, certificate, chain, errors) => true;

            //IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses("dynamic.12306.cn");
          

            OrderRequest orderRequest = new OrderRequest()
                                            {
                                                FromStationTelecode = "BJP",
                                                //FromStationTelecodeName="北京",
                                                ToStationTelecode = "NCG",
                                                //ToStationTelecodeName = "南昌",
                                                IncludeStudent = "00",
                                                StartTimeStr = "00:00--24:00",
                                                TrainClass = "QB#D#Z#T#K#QT#",
                                                TrainDate = DateTime.Parse("2012-10-11"),
                                                TrainPassType = "QB"
                                            };
            BuyTicketConfig.Instance.OrderRequest = orderRequest;
            var Passengers=new Passenger[]
                                       {
                                           new Passenger(){Name="林利",CardNo = "362201198409101614",Checked=true,MobileNo="15910675179"}, 
                                           new Passenger(){Name="林艳",CardNo = "362201198305131667",Checked=true,MobileNo="18610037900"}, 
                                       };
            Account account = new Account("mydobit", "03265791", null);
            bool auto = false;
            account.Login(ref auto);
            Console.WriteLine(account.IsLogin);
            var list = account.QueryTrainInfos(new List<TrainItemInfo>());
            //string ret = account.OrderTicket(list[0], Passengers);
            //if(string.IsNullOrEmpty(ret))
            //{
            //  Console.WriteLine("OrderId="+account.GetOderId());
            //}
            //else
            //{
            //    Console.WriteLine(ret);
            //}
            Console.ReadLine();


        }


        public class  TestClass
        {
            public string Name { get; set; }
            public string Type { get; set; }

            public int Method { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> Data { get; set; } 
        }
    }
}
