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

    public class RuleBLL : BaseBLL<IRuleService<RuleEntity, RuleEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static RuleBLL m_Instance = new RuleBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static RuleBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "RuleCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(RuleEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<RuleEntity> GetPageList(RuleEntity para, ref Pagination pagination)
        {
            List<RuleEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<RuleEntity> GetList(RuleEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(RuleEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(RuleEntity entity)
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
        public RuleEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
