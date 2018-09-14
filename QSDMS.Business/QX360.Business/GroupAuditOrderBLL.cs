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

    public class GroupAuditOrderBLL : BaseBLL<IGroupAuditOrderService<GroupAuditOrderEntity, GroupAuditOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static GroupAuditOrderBLL m_Instance = new GroupAuditOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static GroupAuditOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "GroupAuditOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(GroupAuditOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<GroupAuditOrderEntity> GetPageList(GroupAuditOrderEntity para, ref Pagination pagination)
        {
            List<GroupAuditOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<GroupAuditOrderEntity> GetList(GroupAuditOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(GroupAuditOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(GroupAuditOrderEntity entity)
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
        public GroupAuditOrderEntity GetEntity(string keyValue)
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
    }
}
