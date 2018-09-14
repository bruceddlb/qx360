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

    public class StudyFreeDateBLL : BaseBLL<IStudyFreeDateService<StudyFreeDateEntity, StudyFreeDateEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyFreeDateBLL m_Instance = new StudyFreeDateBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyFreeDateBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "StudyFreeDateCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(StudyFreeDateEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<StudyFreeDateEntity> GetPageList(StudyFreeDateEntity para, ref Pagination pagination)
        {
            List<StudyFreeDateEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<StudyFreeDateEntity> GetList(StudyFreeDateEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(StudyFreeDateEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(StudyFreeDateEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            StudyFreeTimeBLL.Instance.DeleteByObjectId(keyValue);
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new StudyFreeDateEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {

                    this.Delete(item.StudyFreeDateId);
                }
            }
        }

        /// <summary>
        /// 根据时间删除操作
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void DeleteByTime(string startTime, string endTime)
        {
            var list = this.GetList(new StudyFreeDateEntity() { StartTime = startTime, EndTime = endTime });
            if (list != null)
            {
                foreach (var freedateitem in list)
                {
                    var freetimelist = StudyFreeTimeBLL.Instance.GetList(new StudyFreeTimeEntity() { StudyFreeDateId = freedateitem.StudyFreeDateId });
                    foreach (var freetimeitem in freetimelist)
                    {
                        StudyFreeTimeBLL.Instance.Delete(freetimeitem.StudyFreeTimeId);
                    }
                    this.Delete(freedateitem.StudyFreeDateId);
                }
            }
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public StudyFreeDateEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public void ClearData()
        {
            InstanceDAL.ClearData();
        }
    }
}
