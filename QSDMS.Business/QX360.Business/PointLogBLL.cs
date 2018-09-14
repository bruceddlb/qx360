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

    public class PointLogBLL : BaseBLL<IPointLogService<PointLogEntity, PointLogEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static PointLogBLL m_Instance = new PointLogBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static PointLogBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "PointLogCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(PointLogEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<PointLogEntity> GetPageList(PointLogEntity para, ref Pagination pagination)
        {
            List<PointLogEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<PointLogEntity> GetList(PointLogEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(PointLogEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(PointLogEntity entity)
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
        public PointLogEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
