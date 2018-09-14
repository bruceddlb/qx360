
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Data.Common;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>
   
    /// 描 述：注册账户
    /// </summary>
    public class AccountService :IAccountService
    {

        public AccountEntity CheckLogin(string mobileCode, string password)
        {
            throw new NotImplementedException();
        }

        public string GetSecurityCode(string mobileCode)
        {
            throw new NotImplementedException();
        }

        public void Register(AccountEntity accountEntity)
        {
            throw new NotImplementedException();
        }

        public void LoginLimit(string platform, string account, string iPAddress, string iPAddressName)
        {
            throw new NotImplementedException();
        }
    }
}
