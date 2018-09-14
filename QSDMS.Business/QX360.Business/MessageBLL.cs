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

    public class MessageBLL : BaseBLL<IMessageService<MessageEntity, MessageEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MessageBLL m_Instance = new MessageBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MessageBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "MessageCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(MessageEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<MessageEntity> GetPageList(MessageEntity para, ref Pagination pagination)
        {
            List<MessageEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<MessageEntity> GetList(MessageEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(MessageEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(MessageEntity entity)
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
        public MessageEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
