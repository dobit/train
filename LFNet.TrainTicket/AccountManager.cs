using System.Collections.Generic;
using LFNet.TrainTicket.Config;

namespace LFNet.TrainTicket
{
    /// <summary>
    /// �˻�������
    /// </summary>
    public class AccountManager
    {
        static Dictionary<string, Account> accounts = new Dictionary<string, Account>();
        /// <summary>
        /// ���˺��������ip�仯ʱ�ᴴ���µ��˺ţ���������ԭ�����˺�
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="proxyIp"></param>
        /// <returns></returns>
        public static Account GetAccount(string userName, string password, string proxyIp)
        {
            string key = string.Format("{0},{1},{2}", userName, password, proxyIp).ToLower();
            if (accounts.ContainsKey(key))
            {
                return accounts[key];
            }
            else
            {
                Account account = new Account(userName, password, proxyIp);
                accounts.Add(key, account);
                return account;
            }
        }


    }

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
        /// ���˺��Ƿ����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckExist(string name)
        {
            return _accountList.Find(p => p.AccountInfo.Username.Equals(name)) != null;
        }

        /// <summary>
        /// ����һ�����˺�
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