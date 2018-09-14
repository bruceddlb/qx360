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

    public class CourseItemBLL : BaseBLL<ICourseItemService<CourseItemEntity, CourseItemEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static CourseItemBLL m_Instance = new CourseItemBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static CourseItemBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "CourseItemCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(CourseItemEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<CourseItemEntity> GetPageList(CourseItemEntity para, ref Pagination pagination)
        {
            List<CourseItemEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<CourseItemEntity> GetList(CourseItemEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(CourseItemEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(CourseItemEntity entity)
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
        public CourseItemEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
