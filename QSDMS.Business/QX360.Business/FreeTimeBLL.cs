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

    public class FreeTimeBLL : BaseBLL<IFreeTimeService<FreeTimeEntity, FreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static FreeTimeBLL m_Instance = new FreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static FreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "FreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(FreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<FreeTimeEntity> GetPageList(FreeTimeEntity para, ref Pagination pagination)
        {
            List<FreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<FreeTimeEntity> GetList(FreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(FreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(FreeTimeEntity entity)
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
        public FreeTimeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public int ClearData()
        {
            return InstanceDAL.ClearData();
        }
    }
}
