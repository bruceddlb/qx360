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

    public class TeacherCommitteeBLL : BaseBLL<ITeacherCommitteeService<TeacherCommitteeEntity, TeacherCommitteeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TeacherCommitteeBLL m_Instance = new TeacherCommitteeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TeacherCommitteeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TeacherCommitteeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TeacherCommitteeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TeacherCommitteeEntity> GetPageList(TeacherCommitteeEntity para, ref Pagination pagination)
        {
            List<TeacherCommitteeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TeacherCommitteeEntity> GetList(TeacherCommitteeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TeacherCommitteeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TeacherCommitteeEntity entity)
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
        public TeacherCommitteeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
