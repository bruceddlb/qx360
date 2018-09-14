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

    public class ArticleBLL : BaseBLL<IArticleService<ArticleEntity, ArticleEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ArticleBLL m_Instance = new ArticleBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ArticleBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ArticleCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(ArticleEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<ArticleEntity> GetPageList(ArticleEntity para, ref Pagination pagination)
        {
            List<ArticleEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<ArticleEntity> GetList(ArticleEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(ArticleEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(ArticleEntity entity)
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
        public ArticleEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
