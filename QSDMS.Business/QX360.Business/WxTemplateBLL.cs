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

    public class WxTemplateBLL : BaseBLL<IWxTemplateService<WxTemplateEntity, WxTemplateEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WxTemplateBLL m_Instance = new WxTemplateBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WxTemplateBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WxTemplateCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WxTemplateEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WxTemplateEntity> GetPageList(WxTemplateEntity para, ref Pagination pagination)
        {
            List<WxTemplateEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WxTemplateEntity> GetList(WxTemplateEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WxTemplateEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WxTemplateEntity entity)
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
        public WxTemplateEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
