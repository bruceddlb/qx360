using QSDMS.Util;
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

    public class TeacherBLL : BaseBLL<ITeacherService<TeacherEntity, TeacherEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TeacherBLL m_Instance = new TeacherBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TeacherBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TeacherCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TeacherEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TeacherEntity> GetPageList(TeacherEntity para, ref Pagination pagination)
        {
            List<TeacherEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TeacherEntity> GetList(TeacherEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TeacherEntity entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.SimpleSpelling = Str.PinYin(entity.Name);
            }
            return InstanceDAL.Add(entity);
        }

        public bool Update(TeacherEntity entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.SimpleSpelling = Str.PinYin(entity.Name);
            }
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
        public TeacherEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public TeacherEntity CheckLogin(string username, string pwd)
        {
            return InstanceDAL.CheckLogin(username, pwd);
        }

        public TeacherEntity GetEntityByOpenId(string openid)
        {
            return InstanceDAL.GetEntityByOpenId(openid);
        }
    }
}
