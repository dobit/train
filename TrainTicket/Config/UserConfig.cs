using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LFNet.Configuration;

namespace LFNet.TrainTicket.Config
{
   public class UserConfig:BaseConfig<UserConfig>
    {
       private List<UserAccount> _userAccounts=new List<UserAccount>();
       public List<UserAccount> UserAccounts
       {
           get { return _userAccounts; }
           set { _userAccounts = value; }
       }
    }
}
