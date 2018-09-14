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

    public class StudyCustomFreeTimeBLL : BaseBLL<IStudyCustomFreeTimeService<StudyCustomFreeTimeEntity, StudyCustomFreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyCustomFreeTimeBLL m_Instance = new StudyCustomFreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyCustomFreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "StudyCustomFreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(StudyCustomFreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<StudyCustomFreeTimeEntity> GetPageList(StudyCustomFreeTimeEntity para, ref Pagination pagination)
        {
            List<StudyCustomFreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<StudyCustomFreeTimeEntity> GetList(StudyCustomFreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(StudyCustomFreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(StudyCustomFreeTimeEntity entity)
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
        public StudyCustomFreeTimeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
