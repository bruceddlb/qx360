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

    public class TrainingCarBLL : BaseBLL<ITrainingCarService<TrainingCarEntity, TrainingCarEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingCarBLL m_Instance = new TrainingCarBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingCarBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TrainingCarCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingCarEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingCarEntity> GetPageList(TrainingCarEntity para, ref Pagination pagination)
        {
            List<TrainingCarEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingCarEntity> GetList(TrainingCarEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingCarEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingCarEntity entity)
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
        public TrainingCarEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
