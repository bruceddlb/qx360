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

    public class MessageSetUpBLL : BaseBLL<IMessageSetUpService<MessageSetUpEntity, MessageSetUpEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MessageSetUpBLL m_Instance = new MessageSetUpBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MessageSetUpBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "MessageSetUpCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(MessageSetUpEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<MessageSetUpEntity> GetPageList(MessageSetUpEntity para, ref Pagination pagination)
        {
            List<MessageSetUpEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<MessageSetUpEntity> GetList(MessageSetUpEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(MessageSetUpEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(MessageSetUpEntity entity)
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
        public MessageSetUpEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
