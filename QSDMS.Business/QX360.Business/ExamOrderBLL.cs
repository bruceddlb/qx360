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

    public class ExamOrderBLL : BaseBLL<IExamOrderService<ExamOrderEntity, ExamOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ExamOrderBLL m_Instance = new ExamOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ExamOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ExamCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ExamOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ExamOrderEntity> GetPageList(ExamOrderEntity para, ref Pagination pagination)
        {
            List<ExamOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ExamOrderEntity> GetList(ExamOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ExamOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ExamOrderEntity entity)
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
        public ExamOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
