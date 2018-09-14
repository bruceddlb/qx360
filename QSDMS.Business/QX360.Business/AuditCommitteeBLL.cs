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

    public class AuditCommitteeBLL : BaseBLL<IAuditCommitteeService<AuditCommitteeEntity, AuditCommitteeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditCommitteeBLL m_Instance = new AuditCommitteeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditCommitteeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AuditCommitteeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AuditCommitteeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AuditCommitteeEntity> GetPageList(AuditCommitteeEntity para, ref Pagination pagination)
        {
            List<AuditCommitteeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AuditCommitteeEntity> GetList(AuditCommitteeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AuditCommitteeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AuditCommitteeEntity entity)
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
        public AuditCommitteeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
