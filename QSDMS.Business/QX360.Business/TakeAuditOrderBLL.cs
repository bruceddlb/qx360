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

    public class TakeAuditOrderBLL : BaseBLL<ITakeAuditOrderService<TakeAuditOrderEntity, TakeAuditOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TakeAuditOrderBLL m_Instance = new TakeAuditOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TakeAuditOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TakeAuditOrderSCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TakeAuditOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TakeAuditOrderEntity> GetPageList(TakeAuditOrderEntity para, ref Pagination pagination)
        {
            List<TakeAuditOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TakeAuditOrderEntity> GetList(TakeAuditOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TakeAuditOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TakeAuditOrderEntity entity)
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
        public TakeAuditOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }
    }
}
