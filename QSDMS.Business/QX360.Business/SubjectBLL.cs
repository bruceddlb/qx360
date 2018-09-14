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

    public class SubjectBLL : BaseBLL<ISubjectService<SubjectEntity, SubjectEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SubjectBLL m_Instance = new SubjectBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SubjectBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "SubjectCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(SubjectEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<SubjectEntity> GetPageList(SubjectEntity para, ref Pagination pagination)
        {
            List<SubjectEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<SubjectEntity> GetList(SubjectEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(SubjectEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(SubjectEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public void DeleteByschoolId(string schoolid)
        {
            var list = this.GetList(new SubjectEntity() { SchoolId = schoolid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.SubjectId);
                }
            }
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
        public SubjectEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
