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

    public class MemberBLL : BaseBLL<IMemberService<MemberEntity, MemberEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MemberBLL m_Instance = new MemberBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MemberBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "MemberCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(MemberEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<MemberEntity> GetPageList(MemberEntity para, ref Pagination pagination)
        {
            try
            {
                List<MemberEntity> list = InstanceDAL.GetPageList(para, ref pagination);

                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }

        public List<MemberEntity> GetList(MemberEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(MemberEntity entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.MemberName))
            {
                entity.SimpleSpelling = Str.PinYin(entity.MemberName);
            }
            return InstanceDAL.Add(entity);
        }

        public bool Update(MemberEntity entity)
        {
            if (!string.IsNullOrWhiteSpace(entity.MemberName))
            {
                entity.SimpleSpelling = Str.PinYin(entity.MemberName);
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
        public MemberEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        /// <summary>
        /// 检测登陆
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public MemberEntity CheckLogin(string username, string pwd)
        {
            return InstanceDAL.CheckLogin(username, pwd);
        }

        /// <summary>
        /// 根据openid查询对应信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public MemberEntity GetEntityForOpenId(string openid)
        {
            return InstanceDAL.GetEntityForOpenId(openid);
        }
    }
}
