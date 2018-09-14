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

    public class SmsLogBLL : BaseBLL<ISmsLogService<SmsLogEntity, SmsLogEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SmsLogBLL m_Instance = new SmsLogBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SmsLogBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "SmsLogCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(SmsLogEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<SmsLogEntity> GetPageList(SmsLogEntity para, ref Pagination pagination)
        {
            List<SmsLogEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<SmsLogEntity> GetList(SmsLogEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(SmsLogEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(SmsLogEntity entity)
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
        public SmsLogEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
