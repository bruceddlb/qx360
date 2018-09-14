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

    /// <summary>
    /// 财务报表
    /// </summary>
    public class FinaceBLL : BaseBLL<IFinaceService<FinaceEntity, FinaceEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static FinaceBLL m_Instance = new FinaceBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static FinaceBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "FinaceCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(FinaceEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<FinaceEntity> GetPageList(FinaceEntity para, ref Pagination pagination)
        {
            List<FinaceEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<FinaceEntity> GetList(FinaceEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(FinaceEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(FinaceEntity entity)
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
        public FinaceEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
