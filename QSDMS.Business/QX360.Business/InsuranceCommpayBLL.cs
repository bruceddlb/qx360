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

    public class InsuranceCommpayBLL : BaseBLL<IInsuranceCommpayService<InsuranceCommpayEntity, InsuranceCommpayEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static InsuranceCommpayBLL m_Instance = new InsuranceCommpayBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static InsuranceCommpayBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "InsuranceCommpayCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(InsuranceCommpayEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<InsuranceCommpayEntity> GetPageList(InsuranceCommpayEntity para, ref Pagination pagination)
        {
            List<InsuranceCommpayEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<InsuranceCommpayEntity> GetList(InsuranceCommpayEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(InsuranceCommpayEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(InsuranceCommpayEntity entity)
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
        public InsuranceCommpayEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
