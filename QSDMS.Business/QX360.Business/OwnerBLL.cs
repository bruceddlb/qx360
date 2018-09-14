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

    public class OwnerBLL : BaseBLL<IOwnerService<OwnerEntity, OwnerEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static OwnerBLL m_Instance = new OwnerBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static OwnerBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "OwnerCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(OwnerEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<OwnerEntity> GetPageList(OwnerEntity para, ref Pagination pagination)
        {
            List<OwnerEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<OwnerEntity> GetList(OwnerEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(OwnerEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(OwnerEntity entity)
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
        public OwnerEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
