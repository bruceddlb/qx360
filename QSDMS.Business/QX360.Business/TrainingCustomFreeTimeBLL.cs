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

    public class TrainingCustomFreeTimeBLL : BaseBLL<ITrainingCustomFreeTimeService<TrainingCustomFreeTimeEntity, TrainingCustomFreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingCustomFreeTimeBLL m_Instance = new TrainingCustomFreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingCustomFreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TrainingCustomFreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingCustomFreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingCustomFreeTimeEntity> GetPageList(TrainingCustomFreeTimeEntity para, ref Pagination pagination)
        {
            List<TrainingCustomFreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingCustomFreeTimeEntity> GetList(TrainingCustomFreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingCustomFreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingCustomFreeTimeEntity entity)
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
        public TrainingCustomFreeTimeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
