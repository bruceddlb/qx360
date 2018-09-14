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

    public class SeeCarOrderBLL : BaseBLL<ISeeCarOrderService<SeeCarOrderEntity, SeeCarOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SeeCarOrderBLL m_Instance = new SeeCarOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SeeCarOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "SeeCarOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(SeeCarOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<SeeCarOrderEntity> GetPageList(SeeCarOrderEntity para, ref Pagination pagination)
        {
            List<SeeCarOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<SeeCarOrderEntity> GetList(SeeCarOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(SeeCarOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(SeeCarOrderEntity entity)
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
        public SeeCarOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }
    }
}
