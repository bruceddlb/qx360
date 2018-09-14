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

    public class AuditFreeDateBLL : BaseBLL<IAuditFreeDateService<AuditFreeDateEntity, AuditFreeDateEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditFreeDateBLL m_Instance = new AuditFreeDateBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditFreeDateBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AuditFreeDateCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AuditFreeDateEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AuditFreeDateEntity> GetPageList(AuditFreeDateEntity para, ref Pagination pagination)
        {
            List<AuditFreeDateEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AuditFreeDateEntity> GetList(AuditFreeDateEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AuditFreeDateEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AuditFreeDateEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            AuditFreeTimeBLL.Instance.DeleteByObjectId(keyValue);
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new AuditFreeDateEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.AuditFreeDateId);
                }
            }
        }

        /// <summary>
        /// 根据时间删除操作
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void DeleteByTime(string startTime, string endTime)
        {
            var list = this.GetList(new AuditFreeDateEntity() { StartTime = startTime, EndTime = endTime });
            if (list != null)
            {
                foreach (var freedateitem in list)
                {
                    var freetimelist = AuditFreeTimeBLL.Instance.GetList(new AuditFreeTimeEntity() { AuditFreeDateId = freedateitem.AuditFreeDateId });
                    foreach (var freetimeitem in freetimelist)
                    {
                        AuditFreeTimeBLL.Instance.Delete(freetimeitem.AuditFreeTimeId);
                    }
                    this.Delete(freedateitem.AuditFreeDateId);
                }
            }
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public AuditFreeDateEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
