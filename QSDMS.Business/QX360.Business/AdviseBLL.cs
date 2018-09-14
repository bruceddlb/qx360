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

    public class AdviseBLL : BaseBLL<IAdviseService<AdviseEntity, AdviseEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AdviseBLL m_Instance = new AdviseBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AdviseBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AdviseCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AdviseEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AdviseEntity> GetPageList(AdviseEntity para, ref Pagination pagination)
        {
            List<AdviseEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AdviseEntity> GetList(AdviseEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AdviseEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AdviseEntity entity)
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
        public AdviseEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
