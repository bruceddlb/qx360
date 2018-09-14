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

    public class StudyCommitteeBLL : BaseBLL<IStudyCommitteeService<StudyCommitteeEntity, StudyCommitteeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyCommitteeBLL m_Instance = new StudyCommitteeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyCommitteeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "StudyCommitteeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(StudyCommitteeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<StudyCommitteeEntity> GetPageList(StudyCommitteeEntity para, ref Pagination pagination)
        {
            List<StudyCommitteeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<StudyCommitteeEntity> GetList(StudyCommitteeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(StudyCommitteeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(StudyCommitteeEntity entity)
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
        public StudyCommitteeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
