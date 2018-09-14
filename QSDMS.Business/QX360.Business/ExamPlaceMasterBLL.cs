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

    public class ExamPlaceMasterBLL : BaseBLL<IExamPlaceMasterService<ExamPlaceMasterEntity, ExamPlaceMasterEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ExamPlaceMasterBLL m_Instance = new ExamPlaceMasterBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ExamPlaceMasterBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ExamPlaceMasterCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ExamPlaceMasterEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ExamPlaceMasterEntity> GetPageList(ExamPlaceMasterEntity para, ref Pagination pagination)
        {
            List<ExamPlaceMasterEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ExamPlaceMasterEntity> GetList(ExamPlaceMasterEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ExamPlaceMasterEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ExamPlaceMasterEntity entity)
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
        public ExamPlaceMasterEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        /// <summary>
        ///  登陆
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public ExamPlaceMasterEntity CheckLogin(string name, string pwd)
        {
            return InstanceDAL.CheckLogin(name, pwd);
        }
    }
}
