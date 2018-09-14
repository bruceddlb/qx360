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

    public class AttachmentPicBLL : BaseBLL<IAttachmentPicService<AttachmentPicEntity, AttachmentPicEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AttachmentPicBLL m_Instance = new AttachmentPicBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AttachmentPicBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AttachmentPicCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AttachmentPicEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AttachmentPicEntity> GetPageList(AttachmentPicEntity para, ref Pagination pagination)
        {
            List<AttachmentPicEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AttachmentPicEntity> GetList(AttachmentPicEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AttachmentPicEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AttachmentPicEntity entity)
        {
            return InstanceDAL.Update(entity);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new AttachmentPicEntity() { ObjectId=objectid});
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.PicId);
                }
            }
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
        public AttachmentPicEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
