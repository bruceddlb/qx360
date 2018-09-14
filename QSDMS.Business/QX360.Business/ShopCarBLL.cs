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

    public class ShopCarBLL : BaseBLL<IShopCarService<ShopCarEntity, ShopCarEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ShopCarBLL m_Instance = new ShopCarBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ShopCarBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ShopCarCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ShopCarEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ShopCarEntity> GetPageList(ShopCarEntity para, ref Pagination pagination)
        {
            List<ShopCarEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ShopCarEntity> GetList(ShopCarEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ShopCarEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ShopCarEntity entity)
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
        public ShopCarEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
