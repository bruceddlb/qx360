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

    public class WithDringCommitteeBLL : BaseBLL<IWithDringCommitteeService<WithDringCommitteeEntity, WithDringCommitteeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDringCommitteeBLL m_Instance = new WithDringCommitteeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDringCommitteeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WithDringCommitteeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WithDringCommitteeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WithDringCommitteeEntity> GetPageList(WithDringCommitteeEntity para, ref Pagination pagination)
        {
            List<WithDringCommitteeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WithDringCommitteeEntity> GetList(WithDringCommitteeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WithDringCommitteeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WithDringCommitteeEntity entity)
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
        public WithDringCommitteeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
