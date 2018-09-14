using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.IServices
{
    public interface IMemberService<T, Q, P> : IDAL<T, Q, P>
    {
        /// <summary>
        /// 检测登陆
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        T CheckLogin(string username,string pwd);

        /// <summary>
        /// 根据openid获取信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        T GetEntityForOpenId(string openid);
    }
}
