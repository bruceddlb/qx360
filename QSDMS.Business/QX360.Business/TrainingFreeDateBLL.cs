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

    public class TrainingFreeDateBLL : BaseBLL<ITrainingFreeDateService<TrainingFreeDateEntity, TrainingFreeDateEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingFreeDateBLL m_Instance = new TrainingFreeDateBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingFreeDateBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TrainingFreeDateCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingFreeDateEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingFreeDateEntity> GetPageList(TrainingFreeDateEntity para, ref Pagination pagination)
        {
            List<TrainingFreeDateEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingFreeDateEntity> GetList(TrainingFreeDateEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingFreeDateEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingFreeDateEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            TrainingFreeTimeBLL.Instance.DeleteByObjectId(keyValue);
            return InstanceDAL.Delete(keyValue);
        }

        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new TrainingFreeDateEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.TrainingFreeDateId);
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
            var list = this.GetList(new TrainingFreeDateEntity() { StartTime = startTime, EndTime = endTime });
            if (list != null)
            {
                foreach (var freedateitem in list)
                {
                    var freetimelist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity() { TrainingFreeDateId = freedateitem.TrainingFreeDateId });
                    foreach (var freetimeitem in freetimelist)
                    {
                        TrainingFreeTimeBLL.Instance.Delete(freetimeitem.TrainingFreeTimeId);
                    }
                    this.Delete(freedateitem.TrainingFreeDateId);
                }
            }
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public TrainingFreeDateEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public void ClearData()
        {
            InstanceDAL.ClearData();
        }
    }
}
