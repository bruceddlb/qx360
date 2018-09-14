using iFramework.Business;
using QSDMS.Data.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Business
{
    public class AccountBLL : BaseBLL<IAccountService>
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        private static AccountBLL m_Instance = new AccountBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AccountBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        private AccountBLL() { }

        public void aa()
        {
            InstanceDAL.CheckLogin("", "");
        }

    }
}
