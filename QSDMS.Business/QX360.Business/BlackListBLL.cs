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

    public class BlackListBLL : BaseBLL<IBlackListService<BlackListEntity, BlackListEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static BlackListBLL m_Instance = new BlackListBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static BlackListBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "BlackListCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(BlackListEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<BlackListEntity> GetPageList(BlackListEntity para, ref Pagination pagination)
        {
            List<BlackListEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<BlackListEntity> GetList(BlackListEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(BlackListEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(BlackListEntity entity)
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
        public BlackListEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
