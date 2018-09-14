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

    public class WxUserInfoBLL : BaseBLL<IWxUserInfoService<WxUserInfoEntity, WxUserInfoEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WxUserInfoBLL m_Instance = new WxUserInfoBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WxUserInfoBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WxUserInfoCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WxUserInfoEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WxUserInfoEntity> GetPageList(WxUserInfoEntity para, ref Pagination pagination)
        {
            List<WxUserInfoEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WxUserInfoEntity> GetList(WxUserInfoEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WxUserInfoEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WxUserInfoEntity entity)
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
        public WxUserInfoEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
