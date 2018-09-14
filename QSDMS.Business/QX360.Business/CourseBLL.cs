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

    public class CourseBLL : BaseBLL<ICourseService<CourseEntity, CourseEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static CourseBLL m_Instance = new CourseBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static CourseBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "CourseCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(CourseEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<CourseEntity> GetPageList(CourseEntity para, ref Pagination pagination)
        {
            List<CourseEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<CourseEntity> GetList(CourseEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(CourseEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(CourseEntity entity)
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
        public CourseEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
