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

    public class TrainingTimeTableBLL : BaseBLL<ITrainingTimeTableService<TrainingTimeTableEntity, TrainingTimeTableEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingTimeTableBLL m_Instance = new TrainingTimeTableBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingTimeTableBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TrainingTimeTableCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingTimeTableEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingTimeTableEntity> GetPageList(TrainingTimeTableEntity para, ref Pagination pagination)
        {
            List<TrainingTimeTableEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingTimeTableEntity> GetList(TrainingTimeTableEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingTimeTableEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingTimeTableEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new TrainingTimeTableEntity() { SchoolId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.TrainingTimeTableId);
                }
            }
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public TrainingTimeTableEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

    }
}
