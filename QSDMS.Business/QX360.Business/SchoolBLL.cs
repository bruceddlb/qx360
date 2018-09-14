using QSDMS.Util;
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

    public class SchoolBLL : BaseBLL<ISchoolService<SchoolEntity, SchoolEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SchoolBLL m_Instance = new SchoolBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SchoolBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "SchoolCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(SchoolEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<SchoolEntity> GetPageList(SchoolEntity para, ref Pagination pagination)
        {
            List<SchoolEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<SchoolEntity> GetList(SchoolEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(SchoolEntity entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.SimpleSpelling = Str.PinYin(entity.Name);
            }
            return InstanceDAL.Add(entity);
        }

        public bool Update(SchoolEntity entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.SimpleSpelling = Str.PinYin(entity.Name);
            }
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
        public SchoolEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public SchoolEntity CheckLogin(string username, string pwd)
        {
            return InstanceDAL.CheckLogin(username, pwd);
        }
    }
}
