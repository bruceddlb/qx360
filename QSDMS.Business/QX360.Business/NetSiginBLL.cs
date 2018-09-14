using QSDMS.Util.WebControl;
using QX360.Data.IServices;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{

    public class NetSiginBLL : BaseBLL<INetSiginService<NetSiginEntity, NetSiginEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static NetSiginBLL m_Instance = new NetSiginBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static NetSiginBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "NetSiginCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(NetSiginEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<NetSiginEntity> GetPageList(NetSiginEntity para, ref Pagination pagination)
        {
            List<NetSiginEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<NetSiginEntity> GetList(NetSiginEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(NetSiginEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(NetSiginEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public NetSiginEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
