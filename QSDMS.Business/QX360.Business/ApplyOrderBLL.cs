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

    public class ApplyOrderBLL : BaseBLL<IApplyOrderService<ApplyOrderEntity, ApplyOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ApplyOrderBLL m_Instance = new ApplyOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ApplyOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ApplyOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ApplyOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ApplyOrderEntity> GetPageList(ApplyOrderEntity para, ref Pagination pagination)
        {
            List<ApplyOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ApplyOrderEntity> GetList(ApplyOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ApplyOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ApplyOrderEntity entity)
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
        public ApplyOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取业务单号
        /// </summary>
        /// <returns></returns>
        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }

        /// <summary>
        /// 判断是否报名
        /// </summary>
        /// <returns></returns>
        public int CheckHasApplay(string memberid)
        {
            int count = this.GetList(new ApplyOrderEntity() { MemberId = memberid }).Count();
            if (count > 0)
            {
                return 0;
            }
            return 1;
        }
    }
}
