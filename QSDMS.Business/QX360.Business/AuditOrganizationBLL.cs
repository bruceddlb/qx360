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

    public class AuditOrganizationBLL : BaseBLL<IAuditOrganizationService<AuditOrganizationEntity, AuditOrganizationEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditOrganizationBLL m_Instance = new AuditOrganizationBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditOrganizationBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AuditOrganizationCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AuditOrganizationEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AuditOrganizationEntity> GetPageList(AuditOrganizationEntity para, ref Pagination pagination)
        {
            List<AuditOrganizationEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AuditOrganizationEntity> GetList(AuditOrganizationEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AuditOrganizationEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AuditOrganizationEntity entity)
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
        public AuditOrganizationEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

      
    }
}
