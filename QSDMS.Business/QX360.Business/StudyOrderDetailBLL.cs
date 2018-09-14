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

    public class StudyOrderDetailBLL : BaseBLL<IStudyOrderDetailService<StudyOrderDetailEntity, StudyOrderDetailEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyOrderDetailBLL m_Instance = new StudyOrderDetailBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyOrderDetailBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "StudyOrderDetailCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(StudyOrderDetailEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<StudyOrderDetailEntity> GetPageList(StudyOrderDetailEntity para, ref Pagination pagination)
        {
            List<StudyOrderDetailEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<StudyOrderDetailEntity> GetList(StudyOrderDetailEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(StudyOrderDetailEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(StudyOrderDetailEntity entity)
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
        public StudyOrderDetailEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
