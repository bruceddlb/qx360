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

    public class ClassBLL : BaseBLL<IClassService<ClassEntity, ClassEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ClassBLL m_Instance = new ClassBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ClassBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ClassCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ClassEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ClassEntity> GetPageList(ClassEntity para, ref Pagination pagination)
        {
            List<ClassEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ClassEntity> GetList(ClassEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ClassEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ClassEntity entity)
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
        public ClassEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
