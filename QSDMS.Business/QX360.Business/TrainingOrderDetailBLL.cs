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

    public class TrainingOrderDetailBLL : BaseBLL<ITrainingOrderDetailService<TrainingOrderDetailEntity, TrainingOrderDetailEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingOrderDetailBLL m_Instance = new TrainingOrderDetailBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingOrderDetailBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TrainingOrderDetailCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingOrderDetailEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingOrderDetailEntity> GetPageList(TrainingOrderDetailEntity para, ref Pagination pagination)
        {
            List<TrainingOrderDetailEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingOrderDetailEntity> GetList(TrainingOrderDetailEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingOrderDetailEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingOrderDetailEntity entity)
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
        public TrainingOrderDetailEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
