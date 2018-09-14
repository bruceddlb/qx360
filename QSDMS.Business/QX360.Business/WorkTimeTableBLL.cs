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

    public class WorkTimeTableBLL : BaseBLL<IWorkTimeTableService<WorkTimeTableEntity, WorkTimeTableEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WorkTimeTableBLL m_Instance = new WorkTimeTableBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WorkTimeTableBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WorkTimeTableCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WorkTimeTableEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WorkTimeTableEntity> GetPageList(WorkTimeTableEntity para, ref Pagination pagination)
        {
            List<WorkTimeTableEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WorkTimeTableEntity> GetList(WorkTimeTableEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WorkTimeTableEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WorkTimeTableEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new WorkTimeTableEntity() { SchoolId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.WorkTimeTableId);
                }
            }
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public WorkTimeTableEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
