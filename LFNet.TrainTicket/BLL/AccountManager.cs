using System;
using System.IO;
using LFNet.Configuration;
using LFNet.TrainTicket.Entity;

namespace LFNet.TrainTicket.BLL
{
    public static class AccountManager
    {

        public static AccountInfo GetAccountInfo(string username)
        {
            try
            {
                return ConfigFileManager.GetConfig<AccountInfo>(GetAccountFilename(username), false);
            }
            catch (DirectoryNotFoundException ex)
            {
                return null;
            }
            catch (FileNotFoundException ex2)
            {
                return null;
            }

        }

        private static string GetAccountFilename(string username)
        {
            if (string.IsNullOrEmpty(username)) username = "default";
            return System.IO.Path.Combine(ConfigFileManager.ConfigPath, "accounts\\" + username + ".config");
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="accountInfo"></param>
        public static void Save(this AccountInfo accountInfo)
        {
            accountInfo.SaveConfig(GetAccountFilename(accountInfo.Username));
        }
    }
}