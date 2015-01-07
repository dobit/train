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
