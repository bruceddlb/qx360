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

    public class NoticeBLL : BaseBLL<INoticeService<NoticeEntity, NoticeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static NoticeBLL m_Instance = new NoticeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static NoticeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "NoticeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(NoticeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<NoticeEntity> GetPageList(NoticeEntity para, ref Pagination pagination)
        {
            List<NoticeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<NoticeEntity> GetList(NoticeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(NoticeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(NoticeEntity entity)
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
        public NoticeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
