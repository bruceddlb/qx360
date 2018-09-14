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

    public class AuditTimeTableBLL : BaseBLL<IAuditTimeTableService<AuditTimeTableEntity, AuditTimeTableEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditTimeTableBLL m_Instance = new AuditTimeTableBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditTimeTableBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AuditTimeTableCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AuditTimeTableEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AuditTimeTableEntity> GetPageList(AuditTimeTableEntity para, ref Pagination pagination)
        {
            List<AuditTimeTableEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AuditTimeTableEntity> GetList(AuditTimeTableEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AuditTimeTableEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AuditTimeTableEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new AuditTimeTableEntity() { AuditId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.AuditTimeTableId);
                }
            }
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public AuditTimeTableEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
