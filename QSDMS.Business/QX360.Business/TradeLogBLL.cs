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

    public class TradeLogBLL : BaseBLL<ITradeLogService<TradeLogEntity, TradeLogEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TradeLogBLL m_Instance = new TradeLogBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TradeLogBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TradeLogCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TradeLogEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TradeLogEntity> GetPageList(TradeLogEntity para, ref Pagination pagination)
        {
            List<TradeLogEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TradeLogEntity> GetList(TradeLogEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TradeLogEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TradeLogEntity entity)
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
        public TradeLogEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
