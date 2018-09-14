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

    public class BannerBLL : BaseBLL<IBannerService<BannerEntity, BannerEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static BannerBLL m_Instance = new BannerBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static BannerBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "BannerCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(BannerEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<BannerEntity> GetPageList(BannerEntity para, ref Pagination pagination)
        {
            List<BannerEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<BannerEntity> GetList(BannerEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(BannerEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(BannerEntity entity)
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
        public BannerEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
