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

    public class ShopBLL : BaseBLL<IShopService<ShopEntity, ShopEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ShopBLL m_Instance = new ShopBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ShopBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ShopCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ShopEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ShopEntity> GetPageList(ShopEntity para, ref Pagination pagination)
        {
            List<ShopEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ShopEntity> GetList(ShopEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ShopEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ShopEntity entity)
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
        public ShopEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
