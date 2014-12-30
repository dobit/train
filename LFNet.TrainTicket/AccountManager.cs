using System.Collections.Generic;
using LFNet.TrainTicket.Config;

namespace LFNet.TrainTicket
{
    public static class AccountManager
    {
        static AccountManager()
        {
            foreach (AccountInfo accountInfo in Config.BuyTicketConfig.Instance.AccountInfos)
            {
                _accountList.Add(new Account(accountInfo));
            }
        }

        private static List<Account> _accountList = new List<Account>();

        public static List<Account> AccountList
        {
            get { return _accountList; }
            set { _accountList = value; }
        }
        /// <summary>
        /// 该账号是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckExist(string name)
        {
            return _accountList.Find(p => p.AccountInfo.Username.Equals(name)) != null;
        }

        /// <summary>
        /// 创建一个新账号
        /// </summary>
        /// <returns></returns>
        public static Account CreateNewAccount()
        {
            string name = "NewAccount" + _accountList.Count + 1;
            AccountInfo accountInfo = new AccountInfo() {Username = name};
            Config.BuyTicketConfig.Instance.AccountInfos.Add(accountInfo);
            Config.BuyTicketConfig.Save();
            Account account = new Account(accountInfo);
            _accountList.Add(account);

            return account;
        }

        public static void Remove(string name)
        {
            Account account = _accountList.Find(p => p.AccountInfo.Username.Equals(name));
            if(account!=null)
            {
                _accountList.Remove(account);
                
            }
            AccountInfo accountInfo = BuyTicketConfig.Instance.AccountInfos.Find(p => p.Username.Equals(name));
            if (accountInfo != null){ Config.BuyTicketConfig.Instance.AccountInfos.Remove(accountInfo);
            Config.BuyTicketConfig.Save();}
        }
    }
}