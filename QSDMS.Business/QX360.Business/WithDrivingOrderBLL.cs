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

    public class WithDrivingOrderBLL : BaseBLL<IWithDrivingOrderService<WithDrivingOrderEntity, WithDrivingOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDrivingOrderBLL m_Instance = new WithDrivingOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDrivingOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WithDrivingOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WithDrivingOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WithDrivingOrderEntity> GetPageList(WithDrivingOrderEntity para, ref Pagination pagination)
        {
            List<WithDrivingOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WithDrivingOrderEntity> GetList(WithDrivingOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WithDrivingOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WithDrivingOrderEntity entity)
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
        public WithDrivingOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }
    }
}
