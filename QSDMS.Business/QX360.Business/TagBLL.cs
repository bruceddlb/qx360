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

    public class TagBLL : BaseBLL<ITagService<TagEntity, TagEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TagBLL m_Instance = new TagBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TagBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TagCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TagEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TagEntity> GetPageList(TagEntity para, ref Pagination pagination)
        {
            List<TagEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TagEntity> GetList(TagEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TagEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TagEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new TagEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.TagId);
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
        public TagEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
